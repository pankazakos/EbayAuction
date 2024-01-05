import { Component, ViewChild } from '@angular/core';
import {
  BasicItemResponse,
  ExtendedItemInfo,
} from '../shared/contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { MatTabChangeEvent, MatTabGroup } from '@angular/material/tabs';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { MatDialog } from '@angular/material/dialog';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { AuthService } from '../shared/services/auth-service.service';
import { ItemComponent } from '../shared/components/item/item.component';
import { environment } from 'src/environments/environment';
import { AlertService } from '../shared/services/alert.service';
import {
  EditItemDialogComponent,
  EditItemDialogOutputData,
} from './edit-item-dialog/edit-item-dialog.component';
import { ConfirmComponent } from '../shared/components/confirm/confirm.component';
import { MyItemService } from './services/my-item.service';
import { PublishItemRequest } from '../shared/contracts/requests/item';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { BasicBidResponse } from '../shared/contracts/responses/bid';
import { BidEndpoints } from '../shared/contracts/endpoints/BidEndpoints';
import { DateTimeFormatService } from '../shared/services/date-time-format.service';
import { CategoryService } from '../shared/services/category.service';

interface MutlipleItemsWithImage {
  items: ExtendedItemInfo[];
  loading: boolean;
}

@Component({
  selector: 'app-my-items',
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss'],
})
export class MyItemsComponent {
  inactiveItems: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  publishedItems: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  publishedWithBids: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  publishedNotExpired: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  publishedNotExpiredWithBids: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  displayedItems: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  myBids: { data: BasicBidResponse[]; loading: boolean } = {
    data: [],
    loading: true,
  };
  itemsWithMyBids: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };

  toggleOnlyWithBids = false;
  toggleOnlyNotExpired = true;

  constructor(
    private http: HttpClient,
    private myItemService: MyItemService,
    private addItemDialog: MatDialog,
    private editItemDialog: MatDialog,
    private authService: AuthService,
    private itemDialog: MatDialog,
    private confirmDialog: MatDialog,
    private alertService: AlertService,
    private dateTimeFormatter: DateTimeFormatService,
    private categoryService: CategoryService
  ) {}

  @ViewChild(MatTabGroup) tabGroup!: MatTabGroup;

  ngOnInit(): void {
    // default tab is only for saved items
    this.setInactiveItems();
  }

  onTabChanged(event: MatTabChangeEvent): void {
    const selectedIndex = event.index;

    switch (selectedIndex) {
      case 0:
        this.setInactiveItems();
        break;
      case 1:
        this.setPublishedItems();
        break;
      case 2:
        this.setMyBids();
        break;
      default:
        break;
    }
  }

  private copyResponseItems(response: BasicItemResponse[]): ExtendedItemInfo[] {
    return response.map((item: BasicItemResponse) => ({
      ...item,
      image: { src: '', isLoading: true },
      auctionStarted: '',
      auctionEnds: '',
    }));
  }

  private setInactiveItems(): void {
    if (!this.inactiveItems.loading) return;

    this.http
      .get(ItemEndpoints.inactive, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.inactiveItems.items = this.copyResponseItems(response);
          this.inactiveItems.loading = false;
          if (this.inactiveItems.items.length > 0)
            this.fetchImagesForInactiveItems();
        },
        error: (error: any) => {
          console.log(error);
        },
      });
  }

  private setPublishedItems(): void {
    if (!this.publishedItems.loading) return;

    this.http
      .get(ItemEndpoints.active, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.publishedItems.items = this.copyResponseItems(response);
          this.publishedItems.loading = false;
          if (this.publishedItems.items.length > 0)
            this.fetchImagesForPublishedItems();
        },
        error: (error: any) => console.error(error),
      });
  }

  private setPublishedWithBids(): void {
    this.publishedWithBids.items = this.publishedItems.items.filter(
      (item) => item.numBids > 0
    );
    this.publishedWithBids.loading = false;
  }

  private setMyBids(): void {
    if (!this.myBids.loading) return;

    this.http
      .get(BidEndpoints.myBids, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicBidResponse[] | any) => {
          console.log(response);

          this.myBids.data = response;
          this.myBids.loading = false;

          this.setItemsWithMyBids();
        },
        error: (error: any) => console.error(error),
      });
  }

  private setItemsWithMyBids(): void {
    if (!this.itemsWithMyBids.loading) return;

    const uniqueItemIds = new Set<number>();
    this.myBids.data.forEach((bid) => {
      uniqueItemIds.add(bid.itemId);
    });

    const requests = Array.from(uniqueItemIds).map((itemId) =>
      this.http.get(ItemEndpoints.getById(itemId))
    );

    forkJoin(requests).subscribe({
      next: (responses: BasicItemResponse[] | any) => {
        console.log(responses);

        this.itemsWithMyBids.items = this.copyResponseItems(responses);
        this.itemsWithMyBids.loading = false;
        if (this.itemsWithMyBids.items.length > 0)
          this.fetchImagesForItemsWithMyBids();
      },
      error: (error: any) => console.error(error),
    });
  }

  private setPublishedNotExpired(): void {
    this.publishedNotExpired.items = this.publishedItems.items.filter(
      (item) => !this.dateTimeFormatter.isExpired(item.ends)
    );
    this.publishedNotExpired.loading = false;
  }

  private setPublishedNotExpiredWithBids(): void {
    this.publishedNotExpiredWithBids = {
      items: this.publishedNotExpired.items.filter((item) =>
        this.publishedWithBids.items.includes(item)
      ),
      loading: false,
    };
  }

  private fetchImagesForInactiveItems(): void {
    this.inactiveItems.items.map((item) => {
      this.http
        .get(`${ItemEndpoints.getImage(item.imageGuid)}`, {
          responseType: 'blob',
        })
        .subscribe({
          next: (imageData: Blob) => {
            setTimeout(() => {
              const blob = new Blob([imageData], {
                type: 'image/jpeg',
              });
              const imageUrl = URL.createObjectURL(blob);
              item.image = {
                src: imageUrl,
                isLoading: false,
              };
            }, environment.timeout);
          },
          error: (error) => {
            console.error(error);
          },
        });
    });
  }

  private fetchImagesForPublishedItems(): void {
    const imageRequests = this.publishedItems.items.map((item) => {
      item.auctionStarted = this.dateTimeFormatter.convertOnlyToDate(
        item.started
      );
      item.auctionEnds = this.dateTimeFormatter.convertOnlyToDate(item.ends);

      return this.http
        .get(`${ItemEndpoints.getImage(item.imageGuid)}`, {
          responseType: 'blob',
        })
        .pipe(
          map((imageData: Blob) => {
            setTimeout(() => {
              const blob = new Blob([imageData], {
                type: 'image/jpeg',
              });
              const imageUrl = URL.createObjectURL(blob);
              item.image = {
                src: imageUrl,
                isLoading: false,
              };
            }, environment.timeout);
          })
        );
    });

    forkJoin(imageRequests).subscribe({
      next: () => {
        this.setPublishedWithBids();
        this.setPublishedNotExpired();
        this.setPublishedNotExpiredWithBids();
        this.displayedItems = this.publishedNotExpired;
      },
      error: (error) => {
        console.error(error);
      },
    });
  }

  private fetchImagesForItemsWithMyBids(): void {
    this.itemsWithMyBids.items.map((item) => {
      item.auctionStarted = this.dateTimeFormatter.convertOnlyToDate(
        item.started
      );
      item.auctionEnds = this.dateTimeFormatter.convertOnlyToDate(item.ends);

      this.http
        .get(`${ItemEndpoints.getImage(item.imageGuid)}`, {
          responseType: 'blob',
        })
        .subscribe({
          next: (imageData: Blob) => {
            setTimeout(() => {
              const blob = new Blob([imageData], {
                type: 'image/jpeg',
              });
              const imageUrl = URL.createObjectURL(blob);
              item.image = {
                src: imageUrl,
                isLoading: false,
              };
            }, environment.timeout);
          },
          error: (error) => {
            console.error(error);
          },
        });
    });
  }

  addItem(): void {
    const addItemDialogRef = this.addItemDialog.open(AddItemDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
    });

    addItemDialogRef.afterClosed().subscribe((result) => {
      if (result == 'success') {
        this.inactiveItems.loading = true;
        this.setInactiveItems();
      }
    });
  }

  confirmDelete(item: ExtendedItemInfo): void {
    const confirmDeleteDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      disableClose: true,
      data: {
        question: `Are you sure you want to delete your saved item ${item.name}?`,
      },
    });

    confirmDeleteDialogRef.afterClosed().subscribe((result) => {
      if (result === 'confirm') {
        this.deleteItem(item.itemId);
      }
    });
  }

  deleteItem(itemId: number): void {
    this.http
      .delete(ItemEndpoints.delete(itemId), {
        headers: this.authService.getHeaders(),
      })
      .subscribe({
        next: (response: any) => {
          console.log(response);
          this.alertService.success('Item successfully deleted', 'Ok');
          this.inactiveItems.loading = true;
          this.setInactiveItems();
        },
        error: (error: any) => console.error(error),
      });
  }

  editItem(item: ExtendedItemInfo): void {
    const editItemDialogRef = this.editItemDialog.open(
      EditItemDialogComponent,
      {
        autoFocus: false,
        restoreFocus: false,
        data: {
          item: item,
        },
      }
    );

    editItemDialogRef
      .afterClosed()
      .subscribe((result: EditItemDialogOutputData) => {
        this.categoryService.clear();

        if (!result) return;

        if (result.status == 'edited') {
          if (result.item) {
            item = result.item;
          }
          this.alertService.success('Item successfully saved', 'Ok');
          this.inactiveItems.loading = true;
          this.setInactiveItems();
        }
      });
  }

  publishItem(item: ExtendedItemInfo): void {
    const publishItemRequestData: PublishItemRequest = {
      expiration: this.myItemService.expiryDatetime,
    };

    this.http
      .put<PublishItemRequest>(
        ItemEndpoints.activate(item.itemId),
        publishItemRequestData,
        { headers: this.authService.getHeaders() }
      )
      .subscribe({
        next: (response: string | any) => {
          console.log(response);
          this.alertService.success('Item successfully published', 'Ok');
          this.inactiveItems.loading = true;
          this.setInactiveItems();
          this.publishedItems.loading = true;
          this.setPublishedItems();
          if (this.tabGroup) this.tabGroup.selectedIndex = 1;
        },
        error: (error: any) => console.error(error),
      });
  }

  showItemDialog(
    item: ExtendedItemInfo,
    itemStatus: string,
    openCalendar: boolean
  ): void {
    let image = null;
    let isItemInactive = false;

    if (itemStatus == 'inactive') {
      item = item;
      image = item.image;
      isItemInactive = true;
    } else if (itemStatus == 'published') {
      item = item;
      image = item.image;
      isItemInactive = false;
    } else {
      return;
    }

    const itemDialogRef = this.itemDialog.open(ItemComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        item: item,
        image: image,
        isItemInactive: isItemInactive,
        openCalendar: openCalendar,
      },
    });

    itemDialogRef.afterClosed().subscribe((result) => {
      if (result == 'publish') {
        this.publishItem(item);
      }
    });
  }

  private handleToggles(): void {
    console.log('handle toggles');
    console.log(this.toggleOnlyNotExpired);
    console.log(this.toggleOnlyWithBids);

    if (this.toggleOnlyNotExpired && this.toggleOnlyWithBids) {
      console.log('both toggles are true');

      this.displayedItems = this.publishedNotExpiredWithBids;
      return;
    }

    if (this.toggleOnlyNotExpired) {
      console.log('only not expired is true');

      this.displayedItems = this.publishedNotExpired;
      return;
    }

    if (this.toggleOnlyWithBids) {
      console.log('only with bids is true');

      this.displayedItems = this.publishedWithBids;
      return;
    }

    console.log('both toggles are false');

    this.displayedItems = this.publishedItems;
  }

  showOnlyNotExpired(): void {
    console.log('show only not expired');

    this.toggleOnlyNotExpired = !this.toggleOnlyNotExpired;

    this.handleToggles();
  }

  showOnlyWithBids(): void {
    console.log('show only with bids');

    this.toggleOnlyWithBids = !this.toggleOnlyWithBids;

    this.handleToggles();
  }
}

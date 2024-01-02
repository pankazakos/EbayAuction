import { Component } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { MatTabChangeEvent } from '@angular/material/tabs';
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

interface SingleItemWithImage {
  data: BasicItemResponse;
  image: { src: string; isLoading: boolean };
}

interface MutlipleItemsWithImage {
  items: SingleItemWithImage[];
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
  itemsWithBids: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };
  displayedItems: MutlipleItemsWithImage = {
    items: [],
    loading: true,
  };

  toggleChecked = false;

  constructor(
    private http: HttpClient,
    private myItemService: MyItemService,
    private addItemDialog: MatDialog,
    private editItemDialog: MatDialog,
    private authService: AuthService,
    private itemDialog: MatDialog,
    private confirmDialog: MatDialog,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {
    // default tab is for my items
    this.setInactiveItems();
    this.setPublishedItems();
  }

  onTabChanged(event: MatTabChangeEvent): void {
    const selectedIndex = event.index;

    switch (selectedIndex) {
      case 0:
        this.setInactiveItems();
        this.setPublishedItems();
        break;
      case 1:
        break;
      default:
        break;
    }
  }

  private copyResponseItems(
    response: BasicItemResponse[]
  ): SingleItemWithImage[] {
    return response.map((item: BasicItemResponse) => ({
      data: item,
      image: { src: '', isLoading: true },
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
          if (this.inactiveItems.items.length > 0) this.fetchImages();
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
          if (this.publishedItems.items.length > 0) this.fetchPublishedImages();
        },
        error: (error: any) => console.error(error),
      });
  }

  private setItemsWithBids(): void {
    this.itemsWithBids.items = this.publishedItems.items.filter(
      (item) => item.data.numBids > 0
    );
    this.itemsWithBids.loading = false;
  }

  private fetchImages(): void {
    this.inactiveItems.items.map((item) => {
      this.http
        .get(`${ItemEndpoints.getImage(item.data.imageGuid)}`, {
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

  private fetchPublishedImages(): void {
    const imageRequests = this.publishedItems.items.map((item) => {
      return this.http
        .get(`${ItemEndpoints.getImage(item.data.imageGuid)}`, {
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
        this.displayedItems = this.publishedItems;
        this.setItemsWithBids();
      },
      error: (error) => {
        console.error(error);
      },
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

  confirmDelete(index: number): void {
    const confirmDeleteDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      disableClose: true,
      data: {
        question: `Are you sure you want to delete your saved item ${this.inactiveItems.items[index].data.name}?`,
      },
    });

    confirmDeleteDialogRef.afterClosed().subscribe((result) => {
      if (result === 'confirm') {
        this.deleteItem(this.inactiveItems.items[index].data.itemId);
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

  editItem(item: BasicItemResponse, idx: number): void {
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
        if (result.status == 'edited') {
          if (result.item) {
            this.inactiveItems.items[idx].data = result.item;
          }
          this.alertService.success('Item successfully saved', 'Ok');
          this.inactiveItems.loading = true;
          this.setInactiveItems();
        }
      });
  }

  publishItem(itemIdx: number): void {
    const publishItemRequestData: PublishItemRequest = {
      expiration: this.myItemService.expiryDatetime,
    };

    this.http
      .put<PublishItemRequest>(
        ItemEndpoints.activate(this.inactiveItems.items[itemIdx].data.itemId),
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
        },
        error: (error: any) => console.error(error),
      });
  }

  showItemDialog(
    itemIdx: number,
    itemStatus: string,
    openCalendar: boolean
  ): void {
    let item = null;
    let image = null;
    let isItemInactive = false;

    if (itemStatus == 'inactive') {
      item = this.inactiveItems.items[itemIdx].data;
      image = this.inactiveItems.items[itemIdx].image;
      isItemInactive = true;
    } else if (itemStatus == 'published') {
      item = this.displayedItems.items[itemIdx].data;
      image = this.displayedItems.items[itemIdx].image;
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
        this.publishItem(itemIdx);
      }
    });
  }

  showOnlyWithBids(): void {
    this.toggleChecked = !this.toggleChecked;

    if (!this.toggleChecked) {
      this.displayedItems = this.publishedItems;
      return;
    }

    this.displayedItems = this.itemsWithBids;
  }
}

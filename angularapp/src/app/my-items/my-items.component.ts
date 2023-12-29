import { Component } from '@angular/core';
import { BasicItemResponse } from '../shared/contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { MatDialog } from '@angular/material/dialog';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { AuthService } from '../shared/services/auth-service.service';
import { ItemComponent } from '../search/item/item.component';
import { environment } from 'src/environments/environment';
import { AlertService } from '../shared/services/alert.service';
import {
  EditItemDialogComponent,
  EditItemDialogOutputData,
} from './edit-item-dialog/edit-item-dialog.component';
import { ConfirmComponent } from '../shared/components/confirm/confirm.component';
import { MyItemService } from './my-item.service';
import { PublishItemRequest } from '../shared/contracts/requests/item';

@Component({
  selector: 'app-my-items',
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss'],
})
export class MyItemsComponent {
  inactiveItems: {
    data: BasicItemResponse[];
    loading: boolean;
    images: { src: string; isLoading: boolean; itemId: number }[];
    empty: boolean;
  } = {
    data: [],
    loading: true,
    images: [],
    empty: false,
  };
  publishedItems: {
    data: BasicItemResponse[];
    loading: boolean;
    images: { src: string; isLoading: boolean; itemId: number }[];
    empty: boolean;
  } = {
    data: [],
    loading: true,
    images: [],
    empty: false,
  };
  itemsWithBids: { data: BasicItemResponse[]; loading: boolean } = {
    data: [],
    loading: true,
  };

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
        this.setItemsWithBids();
        break;
      default:
        break;
    }
  }

  setInactiveItems(): void {
    if (!this.inactiveItems.loading) return;

    this.http
      .get(ItemEndpoints.inactive, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.inactiveItems.data = response;
          this.inactiveItems.loading = false;
          this.inactiveItems.empty = false;
          if (this.inactiveItems.data.length == 0)
            this.inactiveItems.empty = true;
          else this.fetchImages();
        },
        error: (error: any) => {
          console.log(error);
        },
      });
  }

  setPublishedItems(): void {
    if (!this.publishedItems.loading) return;

    this.http
      .get(ItemEndpoints.active, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.publishedItems.data = response;
          this.publishedItems.loading = false;
          this.publishedItems.empty = false;
          if (this.publishedItems.data.length == 0)
            this.publishedItems.empty = true;
          else this.fetchPublishedImages();
        },
        error: (error: any) => console.error(error),
      });
  }

  setItemsWithBids(): void {
    if (!this.itemsWithBids.loading) return;

    this.http
      .get(ItemEndpoints.bidden, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.itemsWithBids.data = response;
          this.itemsWithBids.loading = false;
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
      item = this.inactiveItems.data[itemIdx];
      image = this.inactiveItems.images[itemIdx];
      isItemInactive = true;
    } else if (itemStatus == 'published') {
      item = this.publishedItems.data[itemIdx];
      image = this.publishedItems.images[itemIdx];
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

  private fetchImages(): void {
    this.inactiveItems.images = new Array(this.inactiveItems.data.length).fill({
      src: '',
      isLoading: true,
      itemId: -1,
    });

    this.inactiveItems.data.map((item, i) => {
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
              this.inactiveItems.images[i] = {
                src: imageUrl,
                isLoading: false,
                itemId: item.itemId,
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
    this.publishedItems.images = new Array(
      this.publishedItems.data.length
    ).fill({
      src: '',
      isLoading: true,
      itemId: -1,
    });

    this.publishedItems.data.map((item, i) => {
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
              this.publishedItems.images[i] = {
                src: imageUrl,
                isLoading: false,
                itemId: item.itemId,
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

  confirmDelete(index: number): void {
    const confirmDeleteDialogRef = this.confirmDialog.open(ConfirmComponent, {
      autoFocus: false,
      disableClose: true,
      data: {
        question: `Are you sure you want to delete your saved item ${this.inactiveItems.data[index].name}?`,
      },
    });

    confirmDeleteDialogRef.afterClosed().subscribe((result) => {
      if (result === 'confirm') {
        this.deleteItem(this.inactiveItems.data[index].itemId);
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
            this.inactiveItems.data[idx] = result.item;
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
        ItemEndpoints.activate(this.inactiveItems.data[itemIdx].itemId),
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
}

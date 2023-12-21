import { Component } from '@angular/core';
import {
  BasicItemResponse,
  PublishedItemResponse,
} from '../shared/contracts/responses/item';
import { HttpClient } from '@angular/common/http';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { MatDialog } from '@angular/material/dialog';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { AuthService } from '../shared/services/auth-service.service';
import { ItemComponent } from '../search/item/item.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-my-items',
  templateUrl: './my-items.component.html',
  styleUrls: ['./my-items.component.scss'],
})
export class MyItemsComponent {
  inactiveItems: BasicItemResponse[] = [];
  publishedItems: PublishedItemResponse[] = [];
  itemsWithBids: BasicItemResponse[] = [];
  images: { src: string; isLoading: boolean; itemId: number }[] = [];

  updatedInactive: boolean = false;
  updatedPublished: boolean = false;
  updatedBidden: boolean = false;

  constructor(
    private http: HttpClient,
    private addItemDialog: MatDialog,
    private authService: AuthService,
    private itemDialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.setInactiveItems(); // default tab is for inactive items
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
        this.setItemsWithBids();
        break;
      default:
        break;
    }
  }

  setInactiveItems(): void {
    if (this.updatedInactive) return;

    this.http
      .get(ItemEndpoints.inactive, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.inactiveItems = response;
          this.updatedInactive = true;
          this.fetchImages();
        },
        error: (error: any) => {
          console.log(error);
        },
      });
  }

  setPublishedItems(): void {
    if (this.updatedPublished) return;

    this.http
      .get(ItemEndpoints.active, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: PublishedItemResponse[] | any) => {
          console.log(response);

          this.publishedItems = response;
          this.updatedPublished = true;
        },
        error: (error: any) => console.error(error),
      });
  }

  setItemsWithBids(): void {
    if (this.updatedBidden) return;

    this.http
      .get(ItemEndpoints.bidden, { headers: this.authService.getHeaders() })
      .subscribe({
        next: (response: BasicItemResponse[] | any) => {
          console.log(response);

          this.itemsWithBids = response;
          this.updatedBidden = true;
        },
        error: (error: any) => console.error(error),
      });
  }

  showItemDialog(itemIdx: number): void {
    this.itemDialog.open(ItemComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        item: { ...this.inactiveItems[itemIdx] },
        image: this.images[itemIdx],
      },
    });
  }

  private fetchImages(): void {
    this.inactiveItems.map((item) => {
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
              this.images.push({
                src: imageUrl,
                isLoading: false,
                itemId: item.itemId,
              });
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
        this.updatedInactive = false;
        this.setInactiveItems();
      }
    });
  }
}

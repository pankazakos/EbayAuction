import { AfterViewInit, Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CategoryService } from 'src/app/shared/services/category.service';
import { MyItemService } from '../services/my-item.service';
import { AuthService } from 'src/app/shared/services/auth-service.service';
import { HttpClient } from '@angular/common/http';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';
import {
  BasicItemResponse,
  ExtendedItemInfo,
} from 'src/app/shared/contracts/responses/item';
import { AlertService } from 'src/app/shared/services/alert.service';
import { baseUrl } from 'src/app/shared/types';

export interface EditItemDialogOutputData {
  status: 'edited' | 'cancelled';
  item?: ExtendedItemInfo;
}

@Component({
  selector: 'app-edit-item-dialog',
  templateUrl: './edit-item-dialog.component.html',
  styleUrls: ['./edit-item-dialog.component.scss'],
})
export class EditItemDialogComponent implements AfterViewInit {
  item: BasicItemResponse;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    public myItemService: MyItemService,
    public categoryService: CategoryService,
    private selfDialogRef: MatDialogRef<EditItemDialogComponent>,
    private alertService: AlertService,
    @Inject(MAT_DIALOG_DATA) public dialogInputData: any
  ) {
    this.item = dialogInputData.item;
    this.myItemService.editItemForm.name = this.item.name;
    this.myItemService.editItemForm.firstBid = this.item.firstBid;
    this.myItemService.editItemForm.buyPrice = this.item.buyPrice;
    this.myItemService.editItemForm.description = this.item.description;
  }

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categoryService.setCategories(response);
      },
      error: (error) => console.log(error),
    });

    this.http.get(ItemEndpoints.categories(this.item.itemId)).subscribe({
      next: (response: BasicCategoryResponse | any) => {
        console.log(response);
        this.categoryService.setSelectedCategories(response);
      },
      error: (error) => console.log(error),
    });
  }

  ngAfterViewInit(): void {
    console.log('after view init');

    // this.categoryService.formControl.valueChanges.subscribe((value) => {
    //   this.categoryService.filter(value);
    // });
  }

  onSaveItem(): void {
    console.log('save item');
    this.myItemService.editItemForm.categoryIds =
      this.categoryService.getCategoryIds();

    console.log(this.myItemService.editItemForm);

    const formData = new FormData();

    formData.append(
      'itemJson',
      JSON.stringify(this.myItemService.editItemForm)
      // this.myItemService.editItemForm
    );

    if (this.myItemService.editItemImageFile) {
      formData.append('image', this.myItemService.editItemImageFile);
    }

    this.http
      .put(ItemEndpoints.edit(this.item.itemId), formData, {
        headers: this.authService.getHeaders(),
      })
      .subscribe({
        next: (response: BasicItemResponse | any) => {
          console.log(response);
          this.selfDialogRef.close({
            status: 'edited',
            item: response,
          } as EditItemDialogOutputData);
        },
        error: (error) => {
          console.error(error);
          this.alertService.error('Invalid data to update item', 'Close');
        },
      });
  }

  onFileSelected(event: Event) {
    const element = event.target as HTMLInputElement;

    if (!element.files || element.files.length <= 0) {
      console.error('No files selected');
      return;
    }

    const file = element.files[0];

    if (file) {
      this.myItemService.editItemImageFilename = file.name;
      this.myItemService.editItemImageFile = file;
    }
  }

  onRemoveFile(): void {
    this.myItemService.editItemImageFilename = '';
    this.myItemService.editItemImageFile = null;
  }
}

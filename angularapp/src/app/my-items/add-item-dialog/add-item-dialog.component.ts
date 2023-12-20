import { Component } from '@angular/core';
import { MyItemService } from '../my-item.service';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { baseUrl } from 'src/app/shared/types';
import { AuthService } from 'src/app/shared/services/auth-service.service';
import { CategoryService } from 'src/app/shared/services/category.service';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';

@Component({
  selector: 'app-add-item-dialog',
  templateUrl: './add-item-dialog.component.html',
  styleUrls: ['./add-item-dialog.component.scss'],
})
export class AddItemDialogComponent {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
    public myItemService: MyItemService,
    public categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categoryService.setCategories(response);
      },
      error: (error) => console.log(error),
    });

    this.categoryService.formControl.valueChanges.subscribe((value) => {
      this.categoryService.filter(value);
    });
  }

  onCreateItem(): void {
    this.myItemService.addItemForm.categoryIds =
      this.categoryService.getCategoryIds();

    const formData = new FormData();

    formData.append('itemJson', JSON.stringify(this.myItemService.addItemForm));

    if (this.myItemService.addItemImageFile) {
      formData.append('image', this.myItemService.addItemImageFile);
    }

    this.http
      .post(`${ItemEndpoints.create}`, formData, {
        headers: this.authService.getHeaders(),
      })
      .subscribe({
        next: (response: any) => {
          console.log(response);
        },
        error: (error) => console.log(error),
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
      this.myItemService.addItemImageFilename = file.name;
      this.myItemService.addItemImageFile = file;
    }
  }

  onRemoveFile(): void {
    this.myItemService.addItemImageFilename = '';
    this.myItemService.addItemImageFile = null;
  }
}

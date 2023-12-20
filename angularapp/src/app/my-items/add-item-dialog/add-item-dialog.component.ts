import { Component } from '@angular/core';
import { MyItemService } from '../my-item.service';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { HttpClient } from '@angular/common/http';
import { baseUrl } from 'src/app/shared/types';
import { AuthService } from 'src/app/shared/services/auth-service.service';
import { CategoryService } from 'src/app/shared/services/category.service';

@Component({
  selector: 'app-add-item-dialog',
  templateUrl: './add-item-dialog.component.html',
  styleUrls: ['./add-item-dialog.component.scss'],
})
export class AddItemDialogComponent {
  fileName: string = '';
  selectedFile: File | null = null;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    public myItemService: MyItemService,
    public categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categoryService.categories = response;
        this.categoryService.filteredNames =
          this.categoryService.categories.map((category) => category.name);
      },
      error: (error) => console.log(error),
    });

    this.categoryService.formControl.valueChanges.subscribe((value) => {
      this.categoryService.filter(value);
    });
  }

  // onCategorySelected(event: MatAutocompleteSelectedEvent): void {
  //   const selectedCategory = event.option.value;
  //   this.categoryService.selectedNames.push(selectedCategory);

  //   this.categoryService.setValue(''); // remove from form and display in chip
  // }

  // onAutocompleteEnterKeyPress(): void {
  //   if (this.myItemService.filteredCategoryNames.length > 0) {
  //     const firstMatchingOption = this.myItemService.filteredCategoryNames[0];
  //     this.categoryFormControl.setValue(firstMatchingOption);
  //     this.onCategorySelected({
  //       option: { value: firstMatchingOption },
  //     } as MatAutocompleteSelectedEvent);
  //   }
  // }

  // filterCategories(value: string) {
  //   const filterValue = value.toLowerCase();
  //   this.myItemService.filteredCategoryNames = this.categories
  //     .filter(
  //       (category) =>
  //         !this.myItemService.selectedCategoryNames.includes(category.name)
  //     )
  //     .map((category) => category.name)
  //     .filter((category) => category.toLowerCase().includes(filterValue));
  // }

  // removeCategory(category: string): void {
  //   const index = this.myItemService.selectedCategoryNames.indexOf(category);
  //   if (index >= 0) {
  //     this.myItemService.selectedCategoryNames.splice(index, 1);
  //   }

  //   const indexToAddback = this.categories.findIndex(
  //     (cat) => cat.name == category
  //   );

  //   this.myItemService.filteredCategoryNames.splice(
  //     indexToAddback,
  //     0,
  //     category
  //   );

  //   console.log('filtered: ' + this.myItemService.filteredCategoryNames);
  // }

  onCreateItem(): void {
    console.log(this.myItemService.addItemForm);

    console.log(this.categoryService.selectedNames);

    console.log(this.categoryService.getCategoryIds());

    console.log(this.authService.getHeaders());

    // const formData = new FormData();
    // formData.append('itemJson', JSON.stringify(this.myItemService.addItemForm));
    // if (this.selectedFile) {
    //   formData.append('image', this.selectedFile);
    // }

    // this.http.post(`${ItemEndpoints.create}`, formData, {headers: this.headers}).subscribe({
    //   next: (response: any) => {
    //     console.log(response);
    //   },
    //   error: (error) => console.log(error),
    // });
  }

  onFileSelected(event: Event) {
    const element = event.target as HTMLInputElement;

    if (!element.files || element.files.length <= 0) {
      console.error('No files selected');
      return;
    }

    const file = element.files[0];

    if (file) {
      this.selectedFile = file;
      this.fileName = file.name;
    }
  }

  onRemoveFile(): void {
    this.fileName = '';
  }
}

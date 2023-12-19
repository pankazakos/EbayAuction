import { Component } from '@angular/core';
import { MyItemService } from '../my-item.service';
import { FormControl } from '@angular/forms';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { baseUrl } from 'src/app/shared/types';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';

@Component({
  selector: 'app-add-item-dialog',
  templateUrl: './add-item-dialog.component.html',
  styleUrls: ['./add-item-dialog.component.scss'],
})
export class AddItemDialogComponent {
  categoryFormControl = new FormControl();
  categories: BasicCategoryResponse[] = [];
  fileName: string = '';
  selectedFile: File | null = null;
  headers: HttpHeaders = new HttpHeaders().set(
    'Authorization',
    `Bearer ${localStorage.getItem('accessToken')}`
  );

  constructor(private http: HttpClient, public myItemService: MyItemService) {}

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categories = response;
        this.myItemService.filteredCategoryNames = this.categories.map(
          (category) => category.name
        );
      },
      error: (error) => console.log(error),
    });

    this.categoryFormControl.valueChanges.subscribe((value) => {
      this.filterCategories(value);
    });
  }

  onCategorySelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategory = event.option.value;
    this.myItemService.selectedCategoryNames.push(selectedCategory);

    this.categoryFormControl.setValue(''); // remove from form and display in chip
  }

  onAutocompleteEnterKeyPress(): void {
    if (this.myItemService.filteredCategoryNames.length > 0) {
      const firstMatchingOption = this.myItemService.filteredCategoryNames[0];
      this.categoryFormControl.setValue(firstMatchingOption);
      this.onCategorySelected({
        option: { value: firstMatchingOption },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filterCategories(value: string) {
    const filterValue = value.toLowerCase();
    this.myItemService.filteredCategoryNames = this.categories
      .filter(
        (category) =>
          !this.myItemService.selectedCategoryNames.includes(category.name)
      )
      .map((category) => category.name)
      .filter((category) => category.toLowerCase().includes(filterValue));
  }

  removeCategory(category: string): void {
    const index = this.myItemService.selectedCategoryNames.indexOf(category);
    if (index >= 0) {
      this.myItemService.selectedCategoryNames.splice(index, 1);
    }

    const indexToAddback = this.categories.findIndex(
      (cat) => cat.name == category
    );

    this.myItemService.filteredCategoryNames.splice(
      indexToAddback,
      0,
      category
    );

    console.log('filtered: ' + this.myItemService.filteredCategoryNames);
  }

  onCreateItem(): void {
    console.log(this.myItemService.addItemForm);

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

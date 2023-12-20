import { Injectable } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { BasicCategoryResponse } from '../contracts/responses/category';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  formControl = new FormControl();
  categories: BasicCategoryResponse[] = [];
  selectedNames: string[] = [];
  filteredNames: string[] = [];

  constructor() {}

  onSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategory = event.option.value;
    this.selectedNames.push(selectedCategory);

    this.formControl.setValue('');
  }

  onAutocompleteEnterKeyPress(): void {
    if (this.filteredNames.length > 0) {
      const firstMatchingOption = this.filteredNames[0];
      this.formControl.setValue(firstMatchingOption);
      this.onSelected({
        option: { value: firstMatchingOption },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filter(value: string) {
    const filterValue = value.toLowerCase();
    this.filteredNames = this.categories
      .filter((category) => !this.selectedNames.includes(category.name))
      .map((category) => category.name)
      .filter((category) => category.toLowerCase().includes(filterValue));
  }

  remove(category: string): void {
    const index = this.selectedNames.indexOf(category);
    if (index >= 0) {
      this.selectedNames.splice(index, 1);
    }

    const indexToAddback = this.categories.findIndex(
      (cat) => cat.name == category
    );

    this.filteredNames.splice(indexToAddback, 0, category);
  }

  getCategoryIds(): number[] {
    return this.categories
      .filter((category) => this.selectedNames.includes(category.name))
      .map((category) => category.id);
  }
}

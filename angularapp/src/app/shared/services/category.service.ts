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
  selectedCategories: BasicCategoryResponse[] = [];
  filteredCategories: BasicCategoryResponse[] = [];

  constructor() {}

  onSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategoryName = event.option.value;
    const selectedCategory = this.categories.find(
      (category) => category.name === selectedCategoryName
    );

    if (selectedCategory) this.selectedCategories.push(selectedCategory);

    this.formControl.setValue('');
  }

  onAutocompleteEnterKeyPress(): void {
    if (this.filteredCategories.length > 0) {
      const firstMatchingOption = this.filteredCategories[0];
      this.formControl.setValue(firstMatchingOption.name);
      this.onSelected({
        option: { value: firstMatchingOption },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filter(value: string) {
    const filterValue = value.toLowerCase();

    // Filter based on the input value
    this.filteredCategories = this.categories.filter((category) =>
      category.name.toLowerCase().includes(filterValue)
    );

    // Also filter out the selected categories
    this.filteredCategories = this.filteredCategories.filter(
      (category) =>
        !this.selectedCategories.find((selected) => selected.id === category.id)
    );
  }

  remove(category: BasicCategoryResponse): void {
    const index = this.selectedCategories.findIndex(
      (selected) => selected.id === category.id
    );
    if (index >= 0) {
      this.selectedCategories.splice(index, 1);
    }

    const indexToAddback = this.categories.findIndex(
      (cat) => cat.id === category.id
    );
    if (indexToAddback >= 0) {
      this.filteredCategories.push(category);
    }
  }

  getCategoryIds(): number[] {
    return this.selectedCategories.map((category) => category.id);
  }
}

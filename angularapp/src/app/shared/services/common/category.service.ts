import { Injectable } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { BasicCategoryResponse } from '../../contracts/responses/category';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private _formControl = new FormControl();
  private _categories: BasicCategoryResponse[] = [];
  private _selected: BasicCategoryResponse[] = [];
  private _filtered: BasicCategoryResponse[] = [];

  constructor() {}

  get formControl(): FormControl {
    return this._formControl;
  }

  get categories(): BasicCategoryResponse[] {
    return this._categories;
  }

  get selected(): BasicCategoryResponse[] {
    return this._selected;
  }

  get filtered(): BasicCategoryResponse[] {
    return this._filtered;
  }

  clear(): void {
    this._selected = [];
    this._filtered = [];
  }

  setCategories(categories: BasicCategoryResponse[]): void {
    this._categories = categories;

    if (this._selected.length > 0) {
      this._filtered = this.categories.filter(
        (category) =>
          !this._selected.find((selected) => selected.id === category.id)
      );
      return;
    }

    this._filtered = categories;
  }

  setSelectedCategories(selected: BasicCategoryResponse[]): void {
    this._selected = selected;

    if (this._selected.length > 0) {
      this._filtered = this.categories.filter(
        (category) =>
          !this._selected.find((selected) => selected.id === category.id)
      );
    }
  }

  onSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategoryName = event.option.value;
    const selectedCategory = this._categories.find(
      (category) => category.name === selectedCategoryName
    );

    if (selectedCategory) this._selected.push(selectedCategory);

    this._formControl.setValue('');
  }

  onAutocompleteEnterKeyPress(): void {
    if (this._filtered.length > 0) {
      const firstMatchingOption = this._filtered[0];
      this._formControl.setValue(firstMatchingOption.name);
      this.onSelected({
        option: { value: firstMatchingOption.name },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filter(value: string): void {
    const filterValue = value.toLowerCase();

    // Filter based on the input value
    this._filtered = this._categories.filter((category) =>
      category.name.toLowerCase().includes(filterValue)
    );

    // Also filter out the selected categories
    this._filtered = this._filtered.filter(
      (category) =>
        !this._selected.find((selected) => selected.id === category.id)
    );
  }

  remove(category: BasicCategoryResponse): void {
    const index = this._selected.findIndex(
      (selected) => selected.id === category.id
    );
    if (index >= 0) {
      this._selected.splice(index, 1);
    }

    const categoryExists =
      this._categories.findIndex((cat) => cat.id === category.id) >= 0;

    if (categoryExists) {
      this._filtered.push(category);
    }
  }

  getCategoryIds(): number[] {
    return this._selected.map((category) => category.id);
  }
}

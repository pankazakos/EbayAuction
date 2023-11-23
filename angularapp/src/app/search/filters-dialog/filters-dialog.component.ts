import { Component, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatChipListboxChange } from '@angular/material/chips';
import { baseUrl } from 'src/app/shared/types';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-filters-dialog',
  templateUrl: './filters-dialog.component.html',
  styleUrls: ['./filters-dialog.component.scss'],
})
export class FiltersDialogComponent {
  priceRanges = [
    { id: 'up to $50', values: { from: 0, to: 50 } },
    { id: '$50 - $250', values: { from: 50, to: 250 } },
    { id: '$250 - $1000', values: { from: 250, to: 1000 } },
    { id: '$1000 - $5000', values: { from: 1000, to: 5000 } },
    { id: '$5000 and up', values: { from: 5000, to: 100000 } },
    { id: 'custom', values: { from: 0, to: 100000 } },
  ];
  sliderMinPrice = this.priceRanges[0].values.from;
  sliderMaxPrice = this.priceRanges[0].values.to;
  valueMin = this.priceRanges[0].values.from;
  valueMax = this.priceRanges[0].values.to;
  inputMaxPrice = 1000;
  disabledSlider = false;
  categoryFormControl = new FormControl();
  categories: BasicCategoryResponse[] = [];
  selectedCategoryNames: string[] = [];
  filteredCategoryNames: string[] = [];

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private dialogRef: MatDialogRef<FiltersDialogComponent>
  ) {}

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categories = response;
        this.filteredCategoryNames = this.categories.map(
          (category) => category.name
        );
      },
      error: (error) => console.log(error),
    });

    this.categoryFormControl.valueChanges.subscribe((value) => {
      this.filterCategories(value);
    });
  }

  onPriceRangeChange(event: MatChipListboxChange) {
    const selectedOption = event.value;
    this.priceRanges.forEach((range) => {
      if (selectedOption == range.id) {
        if (range.id == 'custom') {
          this.disabledSlider = true;
          this.sliderMinPrice = 0;
          this.sliderMaxPrice = 0;
          this.valueMin = range.values.from;
          this.valueMax = 2000;
          this.inputMaxPrice = range.values.to;
          return;
        }
        this.disabledSlider = false;
        this.sliderMinPrice = range.values.from;
        this.sliderMaxPrice = range.values.to;
        this.inputMaxPrice = range.values.to;
        this.valueMin = range.values.from;
        this.valueMax = range.values.to;
        return;
      }
    });
  }

  onCategorySelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategory = event.option.value;
    this.selectedCategoryNames.push(selectedCategory);

    this.categoryFormControl.setValue(''); // remove from form and display in chip
  }

  onAutocompleteEnterKeyPress(): void {
    if (this.filteredCategoryNames.length > 0) {
      const firstMatchingOption = this.filteredCategoryNames[0];
      this.categoryFormControl.setValue(firstMatchingOption);
      this.onCategorySelected({
        option: { value: firstMatchingOption },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filterCategories(value: string) {
    console.log('filtering method');

    const filterValue = value.toLowerCase();
    this.filteredCategoryNames = this.categories
      .filter((category) => !this.selectedCategoryNames.includes(category.name))
      .map((category) => category.name)
      .filter((category) => category.toLowerCase().includes(filterValue));
  }

  removeCategory(category: string): void {
    const index = this.selectedCategoryNames.indexOf(category);
    if (index >= 0) {
      this.selectedCategoryNames.splice(index, 1);
    }

    const indexToAddback = this.categories.findIndex(
      (cat) => cat.name == category
    );

    console.log('IndexToAddBack: ' + indexToAddback);

    this.filteredCategoryNames.splice(indexToAddback, 0, category);

    console.log('filtered: ' + this.filteredCategoryNames);
  }

  applyFilters(): void {
    this.closeDialog();

    const categoryParams: {
      category: string[];
    } = {
      category: [],
    };

    for (const categoryName of this.selectedCategoryNames) {
      categoryParams.category.push(categoryName);
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        page: 1,
        minPrice: this.valueMin,
        maxPrice: this.valueMax,
        ...categoryParams,
      },
      queryParamsHandling: 'merge',
    });
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
}

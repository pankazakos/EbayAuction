import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatChipListboxChange } from '@angular/material/chips';
import { baseUrl } from 'src/app/shared/types';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { FormControl } from '@angular/forms';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterService } from '../filter.service';

@Component({
  selector: 'app-filters-dialog',
  templateUrl: './filters-dialog.component.html',
  styleUrls: ['./filters-dialog.component.scss'],
})
export class FiltersDialogComponent {
  categoryFormControl = new FormControl();
  categories: BasicCategoryResponse[] = [];

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private dialogRef: MatDialogRef<FiltersDialogComponent>,
    public filterService: FilterService
  ) {}

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categories = response;
        this.filterService.filteredCategoryNames = this.categories.map(
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
    this.filterService.priceRanges.forEach((range) => {
      if (selectedOption == range.id) {
        if (range.id == 'custom') {
          this.filterService.sliderMinPrice = 0;
          this.filterService.sliderMaxPrice = 0;
          this.filterService.minPrice = range.values.from;
          this.filterService.maxPrice = 2000;
          this.filterService.selected = range.id;
          this.filterService.disabledSlider = true;
          return;
        }
        this.filterService.sliderMinPrice = range.values.from;
        this.filterService.sliderMaxPrice = range.values.to;
        this.filterService.minPrice = range.values.from;
        this.filterService.maxPrice = range.values.to;
        this.filterService.selected = range.id;
        this.filterService.disabledSlider = false;

        return;
      }
    });
  }

  onCategorySelected(event: MatAutocompleteSelectedEvent): void {
    const selectedCategory = event.option.value;
    this.filterService.selectedCategoryNames.push(selectedCategory);

    this.categoryFormControl.setValue(''); // remove from form and display in chip
  }

  onAutocompleteEnterKeyPress(): void {
    if (this.filterService.filteredCategoryNames.length > 0) {
      const firstMatchingOption = this.filterService.filteredCategoryNames[0];
      this.categoryFormControl.setValue(firstMatchingOption);
      this.onCategorySelected({
        option: { value: firstMatchingOption },
      } as MatAutocompleteSelectedEvent);
    }
  }

  filterCategories(value: string) {
    console.log('filtering method');

    const filterValue = value.toLowerCase();
    this.filterService.filteredCategoryNames = this.categories
      .filter(
        (category) =>
          !this.filterService.selectedCategoryNames.includes(category.name)
      )
      .map((category) => category.name)
      .filter((category) => category.toLowerCase().includes(filterValue));
  }

  removeCategory(category: string): void {
    const index = this.filterService.selectedCategoryNames.indexOf(category);
    if (index >= 0) {
      this.filterService.selectedCategoryNames.splice(index, 1);
    }

    const indexToAddback = this.categories.findIndex(
      (cat) => cat.name == category
    );

    console.log('IndexToAddBack: ' + indexToAddback);

    this.filterService.filteredCategoryNames.splice(
      indexToAddback,
      0,
      category
    );

    console.log('filtered: ' + this.filterService.filteredCategoryNames);
  }

  applyFilters(): void {
    this.closeDialog();

    const categoryParams: {
      category: string[];
    } = {
      category: [],
    };

    for (const categoryName of this.filterService.selectedCategoryNames) {
      categoryParams.category.push(categoryName);
    }

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {
        page: 1,
        minPrice: this.filterService.minPrice,
        maxPrice: this.filterService.maxPrice,
        ...categoryParams,
      },
      queryParamsHandling: 'merge',
    });
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
}

import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatChipListboxChange } from '@angular/material/chips';
import { baseUrl } from 'src/app/shared/types';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';
import { MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { FilterService } from '../filter.service';
import { CategoryService } from 'src/app/shared/services/category.service';

@Component({
  selector: 'app-filters-dialog',
  templateUrl: './filters-dialog.component.html',
  styleUrls: ['./filters-dialog.component.scss'],
})
export class FiltersDialogComponent {
  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private dialogRef: MatDialogRef<FiltersDialogComponent>,
    public filterService: FilterService,
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

  applyFilters(): void {
    this.closeDialog();

    const categoryParams: {
      category: string[];
    } = {
      category: [],
    };

    for (const category of this.categoryService.selected) {
      categoryParams.category.push(category.name);
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

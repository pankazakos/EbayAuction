import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatChipListboxChange } from '@angular/material/chips';
import { baseUrl } from 'src/app/shared/types';
import { BasicCategoryResponse } from 'src/app/shared/contracts/responses/category';

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
  selectedPriceRange = this.priceRanges[1].id;
  sliderMinPrice = 0;
  sliderMaxPrice = 1000;
  valueMin = 0;
  valueMax = 0;
  inputMaxPrice = 1000;
  disabledSlider = false;
  categories: BasicCategoryResponse[];
  selectedCategoryNames: string[];

  constructor(private http: HttpClient) {
    this.categories = [];
    this.selectedCategoryNames = [];
  }

  ngOnInit(): void {
    this.http.get(`${baseUrl}/category/all`).subscribe({
      next: (response: BasicCategoryResponse[] | any) => {
        this.categories = response;
      },
      error: (error) => console.log(error),
    });
  }

  onPriceRangeChange(event: MatChipListboxChange) {
    const selectedOption = event.value;
    this.priceRanges.forEach((element) => {
      if (selectedOption == element.id) {
        if (element.id == 'custom') {
          this.disabledSlider = true;
          this.sliderMinPrice = 0;
          this.sliderMaxPrice = 0;
          this.valueMin = element.values.from;
          this.valueMax = 1000;
          this.inputMaxPrice = element.values.to;
          return;
        }
        this.disabledSlider = false;
        this.sliderMinPrice = element.values.from;
        this.sliderMaxPrice = element.values.to;
        this.inputMaxPrice = element.values.to;
        this.valueMin = element.values.from;
        this.valueMax = element.values.to;
        return;
      }
    });
  }
}

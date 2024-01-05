import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class FilterService {
  priceRanges: {
    id: string;
    values: {
      from: number;
      to: number;
    };
  }[];
  minPrice: number | null = null;
  maxPrice: number | null = null;
  selected: string | null = null;
  sliderMinPrice: number;
  sliderMaxPrice: number;
  disabledSlider: boolean;

  constructor() {
    this.priceRanges = [
      { id: 'up to $50', values: { from: 0, to: 50 } },
      { id: '$50 - $250', values: { from: 50, to: 250 } },
      { id: '$250 - $1000', values: { from: 250, to: 1000 } },
      { id: '$1000 - $5000', values: { from: 1000, to: 5000 } },
      { id: '$5000 and up', values: { from: 5000, to: 100000 } },
      { id: 'custom', values: { from: 0, to: 100000 } },
    ];
    this.sliderMinPrice = this.priceRanges[0].values.from;
    this.sliderMaxPrice = this.priceRanges[0].values.to;
    this.disabledSlider = true;
  }

  clear(): void {
    this.minPrice = null;
    this.maxPrice = null;
    this.selected = null;
    this.sliderMinPrice = this.priceRanges[0].values.from;
    this.sliderMaxPrice = this.priceRanges[0].values.to;
    this.disabledSlider = true;
  }
}

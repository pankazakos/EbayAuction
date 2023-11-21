import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FiltersService {
  private filteredCategoryNamesSubject = new BehaviorSubject<string[]>([]);
  filteredCategoryNames$ = this.filteredCategoryNamesSubject.asObservable();

  private priceRangeSubject = new BehaviorSubject<{
    valueFrom: number;
    valueTo: number;
  }>({
    valueFrom: 0,
    valueTo: 0,
  });
  priceRange$ = this.priceRangeSubject.asObservable();

  constructor() {}

  updateFilteredCategoryNames(names: string[]) {
    this.filteredCategoryNamesSubject.next(names);
  }

  updatePriceRange(priceRange: { valueFrom: number; valueTo: number }) {
    this.priceRangeSubject.next(priceRange);
  }
}

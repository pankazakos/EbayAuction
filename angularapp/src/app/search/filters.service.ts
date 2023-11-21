import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FiltersService {
  private selectedCategoryNamesSubject = new BehaviorSubject<string[]>([]);
  selectedCategoryNames$ = this.selectedCategoryNamesSubject.asObservable();

  private priceRangeSubject = new BehaviorSubject<{
    valueFrom: number;
    valueTo: number;
  }>({
    valueFrom: 0,
    valueTo: 0,
  });
  priceRange$ = this.priceRangeSubject.asObservable();

  constructor() {}

  updateselectedCategoryNames(names: string[]) {
    this.selectedCategoryNamesSubject.next(names);
  }

  updatePriceRange(priceRange: { valueFrom: number; valueTo: number }) {
    this.priceRangeSubject.next(priceRange);
  }
}

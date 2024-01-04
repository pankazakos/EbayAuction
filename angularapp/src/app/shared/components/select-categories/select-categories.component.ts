import { Component, Input } from '@angular/core';
import { CategoryService } from '../../services/category.service';

@Component({
  selector: 'app-select-categories',
  templateUrl: './select-categories.component.html',
  styleUrls: ['./select-categories.component.scss'],
})
export class SelectCategoriesComponent {
  private _listOutside = false;

  @Input()
  set listOutside(value: any) {
    this._listOutside = value !== null && `${value}` !== 'false';
  }

  get listOutside() {
    return this._listOutside;
  }

  constructor(public categoryService: CategoryService) {}
}

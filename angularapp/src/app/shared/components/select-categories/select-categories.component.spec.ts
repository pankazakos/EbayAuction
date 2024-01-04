import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectCategoriesComponent } from './select-categories.component';

describe('SelectCategoriesComponent', () => {
  let component: SelectCategoriesComponent;
  let fixture: ComponentFixture<SelectCategoriesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SelectCategoriesComponent]
    });
    fixture = TestBed.createComponent(SelectCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

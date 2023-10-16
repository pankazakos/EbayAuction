import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginTabsComponent } from './login-tabs.component';

describe('LoginTabsComponent', () => {
  let component: LoginTabsComponent;
  let fixture: ComponentFixture<LoginTabsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LoginTabsComponent]
    });
    fixture = TestBed.createComponent(LoginTabsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

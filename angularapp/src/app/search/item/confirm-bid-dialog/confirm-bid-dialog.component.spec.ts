import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmBidDialogComponent } from './confirm-bid-dialog.component';

describe('ConfirmBidDialogComponent', () => {
  let component: ConfirmBidDialogComponent;
  let fixture: ComponentFixture<ConfirmBidDialogComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ConfirmBidDialogComponent]
    });
    fixture = TestBed.createComponent(ConfirmBidDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

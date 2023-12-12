import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-bid-dialog',
  templateUrl: './confirm-bid-dialog.component.html',
  styleUrls: ['./confirm-bid-dialog.component.scss'],
})
export class ConfirmBidDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<ConfirmBidDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onConfirmBid(): void {
    this.dialogRef.close('confirm');
  }
}

import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-confirm-bid-dialog',
  templateUrl: './confirm-bid-dialog.component.html',
  styleUrls: ['./confirm-bid-dialog.component.scss'],
})
export class ConfirmBidDialogComponent {
  constructor(
    private successfulBid: MatSnackBar,
    private dialogRef: MatDialogRef<ConfirmBidDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  onConfirmBid(): void {
    this.successfulBid.open('Bid successful!', 'Ok', {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 3000,
      panelClass: ['basic-snackbar'],
    });

    this.dialogRef.close();
  }
}

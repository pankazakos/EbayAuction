import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  constructor(
    private successSnackbar: MatSnackBar,
    private errorSnackbar: MatSnackBar
  ) {}

  success(message: string, action: string) {
    this.successSnackbar.open(message, action, {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 3000,
      panelClass: ['success-snackbar'],
    });
  }

  error(message: string, action: string) {
    this.errorSnackbar.open(message, action, {
      horizontalPosition: 'center',
      verticalPosition: 'top',
      duration: 2500,
      panelClass: ['error-snackbar'],
    });
  }

  internalError() {
    alert('Internal server error. Please try again later.');
  }
}

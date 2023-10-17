import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { LoginModalComponent } from '../login-modal/login-modal.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavBarComponent {
  constructor(public dialog: MatDialog) {}

  openDialog(): void {
    this.dialog.open(LoginModalComponent);
  }
}

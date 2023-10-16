import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { LoginModalComponent } from '../login-modal/login-modal.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
  standalone: true,
  imports: [
    MatToolbarModule,
    MatGridListModule,
    MatButtonModule,
    MatDialogModule,
    LoginModalComponent,
  ],
})
export class NavBarComponent {
  constructor(public dialog: MatDialog) {}

  openDialog(): void {
    this.dialog.open(LoginModalComponent);
  }
}

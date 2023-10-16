import { Component } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css'],
  standalone: true,
  imports: [MatDialogModule],
})
export class LoginModalComponent {}

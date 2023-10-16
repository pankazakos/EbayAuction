import { Component } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { LoginTabsComponent } from './login-tabs/login-tabs.component';

@Component({
  selector: 'app-login-modal',
  templateUrl: './login-modal.component.html',
  styleUrls: ['./login-modal.component.css'],
  standalone: true,
  imports: [MatDialogModule, LoginTabsComponent],
})
export class LoginModalComponent {}

import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';

@Component({
  selector: 'app-login-tabs',
  templateUrl: './login-tabs.component.html',
  styleUrls: ['./login-tabs.component.css'],
  standalone: true,
  imports: [MatTabsModule, SignInComponent, SignUpComponent],
})
export class LoginTabsComponent {}

import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-login-tabs',
  templateUrl: './login-tabs.component.html',
  styleUrls: ['./login-tabs.component.css'],
  standalone: true,
  imports: [MatTabsModule],
})
export class LoginTabsComponent {}

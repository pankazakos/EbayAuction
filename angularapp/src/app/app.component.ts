import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { AuthService } from './shared/services/auth-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  constructor(private authService: AuthService) {}

  title = 'angularapp';

  ngOnInit(): void {
    this.authService.setAuthData();
  }
}

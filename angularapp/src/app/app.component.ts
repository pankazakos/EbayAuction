import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { AuthService } from './shared/auth-service.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  constructor(http: HttpClient, private authService: AuthService) {}

  title = 'angularapp';

  ngOnInit(): void {
    const token = localStorage.getItem('accessToken');
    if (token == null) {
      return;
    }

    console.log('decoding');

    const decodedJWT = JSON.parse(window.atob(token.split('.')[1]));

    const username = decodedJWT.username;
    const isSupeUser = decodedJWT.IsSuperuser;

    this.authService.setAuthData(username, isSupeUser);
  }
}

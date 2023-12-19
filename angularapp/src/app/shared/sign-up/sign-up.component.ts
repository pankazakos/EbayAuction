import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CardWrapperCssProps } from '../card/card.component';
import { RegisterUserResponse } from '../contracts/responses/user';
import { RegisterUserRequest } from '../contracts/requests/user';
import { AuthService } from '../services/auth-service.service';
import { UserEndpoints } from '../contracts/endpoints/UserEndpoints';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.css'],
})
export class SignUpComponent {
  hide = true;

  cardCssProps: CardWrapperCssProps = {
    height: '100%',
    width: '100%',
  };

  @ViewChild('loginForm') registerForm!: NgForm;

  constructor(private http: HttpClient, private authService: AuthService) {}

  onSubmit() {
    const userInfo = this.registerForm.value as RegisterUserRequest;

    if (this.registerForm.valid) {
      this.http
        .post<RegisterUserResponse>(UserEndpoints.create, userInfo)
        .subscribe({
          next: (response) => {
            this.authService.loginUser(userInfo.username, userInfo.password);
          },
          error: (error) => {
            if (error.status == 400) {
              console.error(error);
              alert(error.error);
            } else {
              alert('Unexpected error. Please try again later');
            }
          },
        });
    }
  }
}

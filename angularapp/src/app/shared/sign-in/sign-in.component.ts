import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CardWrapperCssProps } from '../card/card.component';
import { UserCredentialsRequest } from '../contracts/requests/user';
import { AuthService } from '../auth-service.service';

@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.css'],
})
export class SignInComponent {
  hide = true;

  cardCssProps: CardWrapperCssProps = {
    height: '60%',
    width: '100%',
  };

  @ViewChild('loginForm') loginForm!: NgForm;

  constructor(private authService: AuthService) {}

  onSubmit() {
    const credentials = this.loginForm.value as UserCredentialsRequest;
    if (this.loginForm.valid) {
      this.authService.LoginUser(credentials.username, credentials.password);
    }
  }
}

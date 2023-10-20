import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CardWrapperCssProps } from '../card/card.component';

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

  constructor(private http: HttpClient) {}

  onSubmit() {
    const credentials = this.loginForm.value;
    if (this.loginForm.valid) {
      this.http
        .post<{ accessToken: string }>(
          'https://localhost:7068/api/User/login',
          credentials
        )
        .subscribe({
          next: (response) => {
            localStorage.setItem('accessToken', response.accessToken);
            console.log('Login successful and token stored');
          },
          error: (error) => {
            console.error('Login failed', error);
          },
        });
    }
  }
}

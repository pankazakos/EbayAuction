import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CardWrapperCssProps } from '../card/card.component';
import { Router } from '@angular/router';

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

  constructor(private http: HttpClient, private router: Router) {}

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
            this.router.navigate(['/']);
          },
          error: (error) => {
            if (error.status == 404) {
              alert('invalid username');
            } else if (error.status == 400) {
              alert('incorrect password');
            } else {
              alert('unexpected error. Please try again later');
            }
          },
        });
    }
  }
}

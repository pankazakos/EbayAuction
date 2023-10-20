import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CardWrapperCssProps } from '../card/card.component';

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

  constructor(private http: HttpClient) {}

  onSubmit() {
    const userInfo = this.registerForm.value;
    console.log(userInfo);

    if (this.registerForm.valid) {
      this.http.post('https://localhost:7068/api/User', userInfo).subscribe({
        next: (response) => {
          console.log('sign up successful');
        },
        error: (error) => {
          console.error('sign up failed', error);
        },
      });
    }
  }
}

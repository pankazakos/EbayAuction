import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CardWrapperCssProps } from '../card/card.component';
import { Router } from '@angular/router';

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

  constructor(private http: HttpClient, private router: Router) {}

  onSubmit() {
    const userInfo = this.registerForm.value;

    if (this.registerForm.valid) {
      this.http.post('https://localhost:7068/api/User', userInfo).subscribe({
        next: (response) => {
          this.router.navigate(['/']);
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

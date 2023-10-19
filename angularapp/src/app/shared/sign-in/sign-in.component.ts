import { Component } from '@angular/core';
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
}

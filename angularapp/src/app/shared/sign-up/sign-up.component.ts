import { Component } from '@angular/core';
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
}

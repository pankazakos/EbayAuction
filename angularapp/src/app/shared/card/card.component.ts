import { Component, Input } from '@angular/core';

export interface CardWrapperCssProps {
  height?: string;
  width?: string;
}

@Component({
  selector: 'app-card',
  templateUrl: './card.component.html',
  styleUrls: ['./card.component.css'],
})
export class CardComponent {
  @Input() cssProps: CardWrapperCssProps = {
    height: '100%',
    width: '100%',
  };
}

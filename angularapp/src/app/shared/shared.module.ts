import { NgModule } from '@angular/core';
import { NavBarComponent } from './components/navbar/navbar.component';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { RouterModule } from '@angular/router';
import { SignInComponent } from './components/sign-in/sign-in.component';
import { SignUpComponent } from './components/sign-up/sign-up.component';
import { CardComponent } from './components/card/card.component';

@NgModule({
  declarations: [
    NavBarComponent,
    SignInComponent,
    SignUpComponent,
    CardComponent,
  ],
  imports: [CommonImportsModule, RouterModule],
  exports: [NavBarComponent],
})
export class SharedModule {}

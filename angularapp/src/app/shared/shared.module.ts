import { NgModule } from '@angular/core';
import { NavBarComponent } from './navbar/navbar.component';
import { LoginModalComponent } from './login-modal/login-modal.component';
import { LoginTabsComponent } from './login-modal/login-tabs/login-tabs.component';
import { SignInComponent } from './login-modal/login-tabs/sign-in/sign-in.component';
import { SignUpComponent } from './login-modal/login-tabs/sign-up/sign-up.component';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [
    NavBarComponent,
    LoginModalComponent,
    LoginTabsComponent,
    SignInComponent,
    SignUpComponent,
  ],
  imports: [CommonImportsModule, RouterModule],
  exports: [
    NavBarComponent,
    LoginModalComponent,
    LoginTabsComponent,
    SignInComponent,
    SignUpComponent,
  ],
})
export class SharedModule {}

import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

import { AppComponent } from './app.component';
import { NavBarComponent } from './navbar/navbar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginModalComponent } from './login-modal/login-modal.component';
import { LoginTabsComponent } from './login-modal/login-tabs/login-tabs.component';
import { SignInComponent } from './login-modal/login-tabs/sign-in/sign-in.component';
import { SignUpComponent } from './login-modal/login-tabs/sign-up/sign-up.component';

import { MaterialUiModule } from './material-ui/material-ui.module';

@NgModule({
  declarations: [
    AppComponent,
    NavBarComponent,
    LoginModalComponent,
    LoginTabsComponent,
    SignInComponent,
    SignUpComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MaterialUiModule,
    CommonModule,
  ],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

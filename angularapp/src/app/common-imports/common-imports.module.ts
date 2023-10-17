import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { MaterialUiModule } from '../material-ui/material-ui.module';

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MaterialUiModule,
    CommonModule,
  ],
  exports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    MaterialUiModule,
    CommonModule,
  ],
})
export class CommonImportsModule {}

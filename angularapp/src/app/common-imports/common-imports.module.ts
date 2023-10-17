import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MaterialUiModule } from '../material-ui/material-ui.module';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    MaterialUiModule,
    MatInputModule,
    MatFormFieldModule,
  ],
  exports: [
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    CommonModule,
    FormsModule,
    MaterialUiModule,
    MatInputModule,
    MatFormFieldModule,
  ],
})
export class CommonImportsModule {}

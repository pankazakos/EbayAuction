import { NgModule } from '@angular/core';
import { AdminComponent } from './admin.component';
import { RouterModule } from '@angular/router';
import { CommonImportsModule } from '../common-imports/common-imports.module';

@NgModule({
  declarations: [AdminComponent],
  imports: [CommonImportsModule, RouterModule],
  exports: [AdminComponent],
})
export class AdminModule {}

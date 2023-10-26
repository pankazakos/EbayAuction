import { NgModule } from '@angular/core';
import { MyItemsComponent } from './my-items.component';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [MyItemsComponent],
  imports: [CommonImportsModule, RouterModule],
  exports: [MyItemsComponent],
})
export class MyItemsModule {}

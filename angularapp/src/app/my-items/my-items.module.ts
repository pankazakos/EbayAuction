import { NgModule } from '@angular/core';
import { MyItemsComponent } from './my-items.component';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { RouterModule } from '@angular/router';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { EditItemDialogComponent } from './edit-item-dialog/edit-item-dialog.component';
import { DatePipe } from '@angular/common';

@NgModule({
  declarations: [
    MyItemsComponent,
    AddItemDialogComponent,
    EditItemDialogComponent,
  ],
  providers: [DatePipe],
  imports: [CommonImportsModule, RouterModule],
  exports: [MyItemsComponent],
})
export class MyItemsModule {}

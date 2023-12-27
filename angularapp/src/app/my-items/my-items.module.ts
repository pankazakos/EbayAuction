import { NgModule } from '@angular/core';
import { MyItemsComponent } from './my-items.component';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { RouterModule } from '@angular/router';
import { AddItemDialogComponent } from './add-item-dialog/add-item-dialog.component';
import { EditItemDialogComponent } from './edit-item-dialog/edit-item-dialog.component';

@NgModule({
  declarations: [
    MyItemsComponent,
    AddItemDialogComponent,
    EditItemDialogComponent,
  ],
  imports: [CommonImportsModule, RouterModule],
  exports: [MyItemsComponent],
})
export class MyItemsModule {}

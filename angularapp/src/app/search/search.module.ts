import { NgModule } from '@angular/core';
import { SearchComponent } from './search.component';
import { ItemComponent } from './item/item.component';
import { RouterModule } from '@angular/router';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { ConfirmBidDialogComponent } from './item/confirm-bid-dialog/confirm-bid-dialog.component';

@NgModule({
  declarations: [SearchComponent, ItemComponent, FiltersDialogComponent, ConfirmBidDialogComponent],
  imports: [CommonImportsModule, RouterModule],
  exports: [SearchComponent],
})
export class SearchModule {}

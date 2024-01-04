import { NgModule } from '@angular/core';
import { SearchComponent } from './search.component';
import { ItemComponent } from '../shared/components/item/item.component';
import { RouterModule } from '@angular/router';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { SharedModule } from '../shared/shared.module';

@NgModule({
  declarations: [SearchComponent, ItemComponent, FiltersDialogComponent],
  imports: [CommonImportsModule, RouterModule, SharedModule],
  exports: [SearchComponent],
})
export class SearchModule {}

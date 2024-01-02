import { NgModule } from '@angular/core';
import { SearchComponent } from './search.component';
import { ItemComponent } from '../shared/components/item/item.component';
import { RouterModule } from '@angular/router';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';

@NgModule({
  declarations: [SearchComponent, ItemComponent, FiltersDialogComponent],
  imports: [CommonImportsModule, RouterModule],
  exports: [SearchComponent],
})
export class SearchModule {}

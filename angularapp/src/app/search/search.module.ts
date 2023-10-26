import { NgModule } from '@angular/core';
import { SearchComponent } from './search.component';
import { ItemComponent } from './item/item.component';
import { RouterModule } from '@angular/router';
import { CommonImportsModule } from '../common-imports/common-imports.module';

@NgModule({
  declarations: [SearchComponent, ItemComponent],
  imports: [CommonImportsModule, RouterModule],
  exports: [SearchComponent],
})
export class SearchModule {}

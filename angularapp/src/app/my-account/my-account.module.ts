import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MaterialUiModule } from '../material-ui/material-ui.module';
import { CommonImportsModule } from '../common-imports/common-imports.module';
import { MyAccountComponent } from './my-account.component';
import { BidHistoryComponent } from './bid-history/bid-history.component';
import { PersonalInfoComponent } from './personal-info/personal-info.component';

@NgModule({
  declarations: [MyAccountComponent, BidHistoryComponent, PersonalInfoComponent],
  imports: [CommonImportsModule, SharedModule, MaterialUiModule],
  exports: [MyAccountComponent],
})
export class MyAccountModule {}

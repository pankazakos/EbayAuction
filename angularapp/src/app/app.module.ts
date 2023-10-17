import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { CommonImportsModule } from './common-imports/common-imports.module';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [AppComponent],
  imports: [CommonImportsModule, SharedModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

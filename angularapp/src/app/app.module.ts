import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { CommonImportsModule } from './common-imports/common-imports.module';
import { SharedModule } from './shared/shared.module';
import { SearchComponent } from './search/search.component';
import { SignInComponent } from './shared/sign-in/sign-in.component';
import { SignUpComponent } from './shared/sign-up/sign-up.component';

const routes: Routes = [
  { path: 'login', component: SignInComponent },
  {
    path: 'register',
    component: SignUpComponent,
  },
];

@NgModule({
  declarations: [AppComponent, SearchComponent],
  imports: [CommonImportsModule, SharedModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

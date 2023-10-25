import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { CommonImportsModule } from './common-imports/common-imports.module';
import { SharedModule } from './shared/shared.module';
import { SearchComponent } from './search/search.component';
import { SignInComponent } from './shared/sign-in/sign-in.component';
import { SignUpComponent } from './shared/sign-up/sign-up.component';
import { AdminComponent } from './admin/admin.component';
import { AdminGuard } from './admin.guard';

const routes: Routes = [
  { title: 'login', path: 'login', component: SignInComponent },
  { title: 'register', path: 'register', component: SignUpComponent },
  {
    title: 'admin',
    path: 'admin',
    component: AdminComponent,
    canActivate: [AdminGuard],
  },
];

@NgModule({
  declarations: [AppComponent, SearchComponent, AdminComponent],
  imports: [CommonImportsModule, SharedModule, RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

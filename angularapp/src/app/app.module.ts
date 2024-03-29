import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { CommonImportsModule } from './common-imports/common-imports.module';
import { SharedModule } from './shared/shared.module';
import { SearchComponent } from './search/search.component';
import { SignInComponent } from './shared/components/sign-in/sign-in.component';
import { SignUpComponent } from './shared/components/sign-up/sign-up.component';
import { AdminComponent } from './admin/admin.component';
import { AdminGuard } from './guards/admin.guard';
import { SearchModule } from './search/search.module';
import { MyItemsModule } from './my-items/my-items.module';
import { MyItemsComponent } from './my-items/my-items.component';
import { authorizedUserGuard } from './guards/authorized-user.guard';
import { AdminModule } from './admin/admin.module';
import { MyAccountComponent } from './my-account/my-account.component';
import { MyAccountModule } from './my-account/my-account.module';

const routes: Routes = [
  {
    title: 'home',
    path: '',
    component: SearchComponent,
  },
  {
    title: 'my items',
    path: 'my-items',
    component: MyItemsComponent,
    canActivate: [authorizedUserGuard],
  },
  {
    title: 'my account',
    path: 'my-account',
    component: MyAccountComponent,
    canActivate: [authorizedUserGuard],
  },
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
  declarations: [AppComponent],
  imports: [
    CommonImportsModule,
    SharedModule,
    AdminModule,
    SearchModule,
    MyItemsModule,
    MyAccountModule,
    RouterModule.forRoot(routes),
  ],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

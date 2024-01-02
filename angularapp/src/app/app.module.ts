import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { CommonImportsModule } from './common-imports/common-imports.module';
import { SharedModule } from './shared/shared.module';
import { SearchComponent } from './search/search.component';
import { SignInComponent } from './shared/components/sign-in/sign-in.component';
import { SignUpComponent } from './shared/components/sign-up/sign-up.component';
import { AdminComponent } from './admin/admin.component';
import { AdminGuard } from './admin.guard';
import { SearchModule } from './search/search.module';
import { MyItemsModule } from './my-items/my-items.module';
import { MyItemsComponent } from './my-items/my-items.component';
import { authorizedUserGuard } from './authorized-user.guard';

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
  declarations: [AppComponent, AdminComponent],
  imports: [
    CommonImportsModule,
    SharedModule,
    SearchModule,
    MyItemsModule,
    RouterModule.forRoot(routes),
  ],
  exports: [RouterModule],
  providers: [],
  bootstrap: [AppComponent],
})
export class AppModule {}

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
import { SearchModule } from './search/search.module';
import { MyItemsModule } from './my-items/my-items.module';
import { MyItemsComponent } from './my-items/my-items.component';
import { TestComponent } from './test/test.component';

const routes: Routes = [
  {
    title: 'home',
    path: '',
    component: SearchComponent,
  },
  { title: 'my items', path: 'my-items', component: MyItemsComponent },
  { title: 'login', path: 'login', component: SignInComponent },
  { title: 'register', path: 'register', component: SignUpComponent },
  {
    title: 'admin',
    path: 'admin',
    component: AdminComponent,
    canActivate: [AdminGuard],
  },
  {
    title: 'test',
    path: 'test',
    component: TestComponent,
  },
];

@NgModule({
  declarations: [AppComponent, AdminComponent, TestComponent],
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

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductListComponent } from './component/product/product-list/product-list.component';
import { ProductDetailComponent } from './component/product/product-detail/product-detail.component';
import { ProductFormComponent } from './component/product/product-form/product-form.component';
import { LoginComponent } from './component/login/login.component';
import { HomeComponent } from './component/home/home.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './component/user/register/register.component';
import { ProfileComponent } from './component/user/profile/profile.component';
import { AddressListComponent } from './component/address/address-list/address-list.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'product/:id', component: ProductDetailComponent },
  {path : 'login', component: LoginComponent},
  { path: 'register', component: RegisterComponent },
  { path: 'product', component: ProductListComponent, canActivate: [AuthGuard]},
  { path: 'create', component: ProductFormComponent, canActivate: [AuthGuard]},
  { path: 'edit/:id', component: ProductFormComponent, canActivate: [AuthGuard]},
  { path: 'profile', component: ProfileComponent },
  { path: 'address', component: AddressListComponent },
  {path : '**', redirectTo: ''},
];

@NgModule({ 
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

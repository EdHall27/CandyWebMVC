import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';  // Importa o RouterModule
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';  // Importa o módulo de rotas
import { HttpClientModule } from '@angular/common/http';

import { ProductListComponent } from './component/product/product-list/product-list.component';
import { ProductDetailComponent } from './component/product/product-detail/product-detail.component';
import { ProductFormComponent } from './component/product/product-form/product-form.component';
import { HomeComponent } from './component/home/home.component';
import { AuthInterceptor } from './services/auth.interceptor';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { LoginComponent } from './component/login/login.component';
import { RegisterComponent } from './component/user/register/register.component';
import { ProfileComponent } from './component/user/profile/profile.component';
import { AddressListComponent } from './component/address/address-list/address-list.component';
import { AddressFormComponent } from './component/address/address-form/address-form.component';


@NgModule({
  declarations: [
    AppComponent,  // Declara o AppComponent
    ProductListComponent,  // Declara os componentes que você criou
    ProductDetailComponent,
    ProductFormComponent,
    HomeComponent,
    NavbarComponent,
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    AddressListComponent,
    AddressFormComponent,
  ],
  imports: [
    BrowserModule,  // Necessário para qualquer aplicação Angular
    FormsModule,  // Necessário para usar ngModel
    RouterModule,  // Necessário para o roteamento
    HttpClientModule,
    AppRoutingModule,  // Importa o módulo de rotas
    ReactiveFormsModule,  // Necessário para usar formulários reativos
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]  // Indica que o AppComponent é o componente raiz
})
export class AppModule { }

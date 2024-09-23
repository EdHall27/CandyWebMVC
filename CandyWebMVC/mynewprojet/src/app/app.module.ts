import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';  // Importa o RouterModule

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';  // Importa o módulo de rotas

//import { ProductListComponent } from './component/product/product-list/product-list.component';
//import { ProductDetailComponent } from './component/product/product-detail/product-detail.component';
//import { ProductFormComponent } from './component/product/product-form/product-form.component';

@NgModule({
  declarations: [
    AppComponent,  // Declara o AppComponent
    //ProductListComponent,  // Declara os componentes que você criou
    //ProductDetailComponent,
    //ProductFormComponent
  ],
  imports: [
    BrowserModule,  // Necessário para qualquer aplicação Angular
    FormsModule,  // Necessário para usar ngModel
    RouterModule,  // Necessário para o roteamento
    AppRoutingModule  // Importa o módulo de rotas
  ],
  providers: [],
  bootstrap: [AppComponent]  // Indica que o AppComponent é o componente raiz
})
export class AppModule { }

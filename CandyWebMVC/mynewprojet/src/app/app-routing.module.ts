import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
//import { ProductListComponent } from './component/product/product-list/product-list.component';
//import { ProductDetailComponent } from './component/product/product-detail/product-detail.component';
//import { ProductFormComponent } from './component/product/product-form/product-form.component';

const routes: Routes = [
  //{ path: '', component: ProductListComponent },
  //{ path: 'products/:id', component: ProductDetailComponent },
  //{ path: 'create', component: ProductFormComponent },
  //{ path: 'edit/:id', component: ProductFormComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

import { Component, OnInit } from '@angular/core';
import { switchMap, catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { ProductService } from 'D:/Projetos/Projeto CandyWeb/CandyWebMVC/CandyWeb/src/app/services/product.service';
import { Product } from '../product.model';
import { ImageService } from '../../../services/image.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css'],
})
export class ProductListComponent implements OnInit {

  products$: Observable<Product[]>  = of([]);

  constructor(private productService: ProductService,private imageService: ImageService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.products$ = this.productService.getProducts();
  }

  deleteProduct(id: number): void {
    if (confirm('Tem certeza que deseja excluir este produto?')) {
      this.productService.deleteProduct(id).subscribe(
        () => {
          console.log(`Produto com ID ${id} foi deletado.`);
          this.loadProducts(); // Recarrega os produtos após a exclusão
        },
        error => {
          console.error('Erro ao deletar o produto', error);
        }
      );
    }
  }
  getImageUrl(imagePath:string | undefined) : string {
    return this.imageService.getImageUrl(imagePath)
  }
}


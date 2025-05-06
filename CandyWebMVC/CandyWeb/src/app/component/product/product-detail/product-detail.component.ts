import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';
import { ImageService } from '../../../services/image.service';


@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  product: Product | undefined;  // Inicializa com um Observable vazio

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private imageService: ImageService,
  ) {}

  ngOnInit(): void {
    const productId = this.route.snapshot.paramMap.get('id');
    if (productId) {
      this.loadProductDetails(Number(productId));
    }
  }

  loadProductDetails(id: number): void {
    this.productService.getProduct(id).subscribe(
      (product) => {
        this.product = product;
      },
      (error) => {
        console.error('Erro ao carregar detalhes do produto:', error);
      }
    );
  }
  getImageUrl(imagePath:string | undefined) : string {
    return this.imageService.getImageUrl(imagePath)
  }

}

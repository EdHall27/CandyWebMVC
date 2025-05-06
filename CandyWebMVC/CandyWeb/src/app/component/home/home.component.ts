import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { ImageService } from '../../services/image.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

  title = 'CandyWeb';
  products: any[] = [];  // Array para armazenar os produtos

  constructor(
    private productService: ProductService,
    private imageService: ImageService,
    private router : Router,
  ) {}

  ngOnInit(): void {
    this.loadProducts();  // Carregar os produtos quando o componente for inicializado
  }

  // Método para carregar produtos da API
  loadProducts() {
    this.productService.getProducts().subscribe(
      (data) => {
        // Aqui, estamos garantindo que cada produto tenha o caminho correto da imagem usando o ImageService
        this.products = data.map(product => {
          return {
            ...product,
            imageUrl: this.imageService.getImageUrl(product.imagePath)  // Gerar URL da imagem
          };
        });
      },
      (error) => {
        console.error('Erro ao carregar produtos:', error);
      }
    );
  }

  addToCart(product: any) {
    console.log('Produto adicionado ao carrinho:', product);
    
  }

  viewDetails(productId: number) {
    //console.log('Visualizar detalhes do produto com ID:', productId);
    // Navegar para página de detalhes
    this.router.navigate([`/product/${productId}`]);
  }
}

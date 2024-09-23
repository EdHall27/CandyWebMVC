import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductService } from 'D:/Projetos/Projeto CandyWeb/CandyWebMVC/CandyWeb/src/app/services/product.service';
import { Product } from '../product.model';
import { ImageService } from '../../../services/image.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.css']  // Correção: 'styleUrls' ao invés de 'styleUrl'
})
export class ProductFormComponent implements OnInit {
  product: Product = { id: 0, name: '', description: '', price: 0, stock: 0 };
  isEditMode = false;
  selectedFile: File | null = null;

  constructor(
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router,
    private imageService: ImageService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.productService.getProduct(Number(id)).subscribe(
        (data) => {
          this.product = data;
        },
        (error) => console.error('Erro ao carregar produto:', error)
      );
    }
  }

  getImageUrl(imagePath:string | undefined) : string {
    return this.imageService.getImageUrl(imagePath)
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files && input.files.length) {
      this.selectedFile = input.files[0];
    }
  }

  onSubmit(): void {
    const formData = new FormData();

    // Adicione os dados do produto ao FormData
    formData.append('Id', this.product.id ? this.product.id.toString() : '');
    formData.append('Name', this.product.name);
    formData.append('Price', this.product.price.toString());
    formData.append('Description', this.product.description);
    formData.append('Stock', this.product.stock.toString());

    // Adicione o arquivo selecionado ao FormData, se houver
    if (this.selectedFile) {
      formData.append('Image', this.selectedFile, this.selectedFile.name);
    }

    // Log para inspecionar o conteúdo do FormData
    formData.forEach((value, key) => {
      console.log(`FormData field ${key}:`, value);
    });

    const save$ = this.isEditMode
      ? this.productService.updateProductWithImage(this.product.id!, formData)
      : this.productService.createProductWithImage(formData);

    save$.subscribe(
      () => this.router.navigate(['/product']),
      (error:any) => console.error('Erro ao salvar produto:', error)
    );
  }
  
}

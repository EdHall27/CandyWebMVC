// src/app/models/product.model.ts

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  imagePath?: string;
  stock : number;
  // Adicione outros campos conforme necessário
}

import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  constructor() { }

  getImageUrl(imagePath: string | undefined ): string {
    if (imagePath) {
      return `${environment.baseUrl}${imagePath}`;
    } else {
      return 'assets/default-image.jpg'; // Ou o caminho para uma imagem padrão
    }
  }
}

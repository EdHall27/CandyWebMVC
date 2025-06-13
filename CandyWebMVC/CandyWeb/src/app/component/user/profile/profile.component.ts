import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';
import { Router } from '@angular/router';
import { Address } from '../../../services/address.service';

interface UserProfile {
  cpfid: number;
  userName: string;
  userEmail: string;
  userPhone: string;
  isAdmin: boolean;
  defaultAddress: Address | null; // Tipagem correta para o endereço padrão
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
})
export class ProfileComponent {
  apiUrl = environment.apiUrl + '/users';
  user: UserProfile | null = null;
  message = '';

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    // Buscar dados do perfil do utilizador quando o componente é inicializado
    this.http.get<UserProfile>(this.apiUrl + '/profile').subscribe({
      next: (res) => {
        // Atribui a resposta diretamente ao objeto 'user'
        this.user = res;
        console.log('--- Objeto de Perfil Completo Recebido (Frontend) ---', this.user); // Log detalhado
        console.log('--- Endereço Padrão no Objeto de Perfil (Frontend) ---', this.user?.defaultAddress); // Log específico do endereço padrão
      },
      error: (err) => {
        this.message = 'Erro ao carregar perfil. Verifique se está logado. Detalhes: ' + (err.error || err.message);
        console.error('Erro ao buscar perfil (Frontend):', err); // Log de erro
      },
    });
  }

  goToAddresses(): void {
    this.router.navigate(['/address']); 
  }
}

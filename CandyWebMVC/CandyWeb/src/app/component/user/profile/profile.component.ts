import { Component } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environment/environment';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  apiURl = environment.apiUrl + '/users';
  user: any;
  enderecoPadrao: any;
  message = '';

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any>(this.apiURl + '/profile').subscribe({
      next: (res) => {
        this.user = {
          cpfid: res.cpfid,
          userName: res.userName,
          userEmail: res.userEmail,
          userPhone: res.userPhone,
          isAdmin: res.isAdmin
        };
        this.enderecoPadrao = res.enderecoPadrao;
      },
      error: () => this.message = 'Erro ao carregar perfil. Verifique se está logado.'
    });
  }
}

import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environment/environment';
import { AuthResponse } from '../../../models/auth-response.model';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  apiUrl = environment.apiUrl + '/auth/register';
  cpfid = 0;
  userName = '';
  email = '';
  phone = '';
  password = '';
  message = '';

  constructor(private http: HttpClient, private router: Router) {}

  onSubmit(): void {
    const payload = {
      cpfid: this.cpfid,
      userName: this.userName,
      email: this.email,
      phone: this.phone,
      password: this.password
    };
  
    this.http.post<AuthResponse>(this.apiUrl, payload).subscribe({
      next: (res) => {
        localStorage.setItem('access_token', res.accessToken);
        localStorage.setItem('refresh_token', res.refreshToken);
        this.router.navigate(['/']); // Redireciona pra página inicial ou dashboard
      },
      error: (err) => {
        this.message = 'Erro ao cadastrar: ' + (err.error || 'Verifique os dados');
      }
    });
  }
}

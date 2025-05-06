import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment/environment';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
  })
  export class LoginComponent {
  cpfid: number = 0;
  password: string = '';
  error: string = '';

  constructor(private auth: AuthService, private router: Router) {}

  onSubmit(): void {
    if (!this.cpfid || !this.password) {
      this.error = 'Preencha todos os campos.';
      return;
    }

    this.auth.login(this.cpfid, this.password).subscribe({
      next: () => {
        this.error = '';
        this.router.navigate(['/produtos']);
      },
      error: (err) => {
        console.error('Erro no login:', err);
        this.error = 'CPF ou senha inválidos.';
      }
    });
  }
  }
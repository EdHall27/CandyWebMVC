import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  isLoggedIn = false;
  isAdmin = false;

  constructor(private auth: AuthService, private router: Router) {
    this.auth.isLoggedIn$
    .subscribe(value => {this.isLoggedIn = value;this.isAdmin = this.auth.isAdmin();}); // pega do localStorage direto
  }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
    this.isLoggedIn = false;
  }
}

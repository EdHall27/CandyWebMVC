import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'CandyWeb';
  
  constructor(private auth: AuthService) {
    this.auth.checkAuthOnStartup();
  }
}

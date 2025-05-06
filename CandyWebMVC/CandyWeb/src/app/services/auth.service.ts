import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, tap } from 'rxjs';
import { AuthResponse } from '../models/auth-response.model';
import { environment } from '../environment/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private api = environment.apiUrl + '/auth';
  private accessTokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';
  private loggedIn = new BehaviorSubject<boolean>(!!this.getAccessToken());
  public isLoggedIn$ = this.loggedIn.asObservable();

  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  login(cpfid: number, password: string): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.api}/login`, { cpfid, password })
      .pipe(
        tap((res) => {
          this.saveTokens(res.accessToken, res.refreshToken);
          this.loggedIn.next(true);
        })
      );
  }

  refreshToken(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http
      .post<AuthResponse>(`${this.api}/refresh`,  refreshToken )
      .pipe(
        tap((res) => {
          this.saveTokens(res.accessToken, res.refreshToken);
        })
      );
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('access_token');
      localStorage.removeItem('refresh_token');
      this.loggedIn.next(false);
    }
  }

  isAdmin(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;
  
    const payload = JSON.parse(atob(token.split('.')[1]));
    return payload.IsAdmin === 'True'; // ou true, depende de como você envia
  }

  getAccessToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('access_token');
    }
    return null;
  }

  getRefreshToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('refresh_token');
    }
    return null;
  }

  private saveTokens(access: string, refresh: string): void {
    localStorage.setItem(this.accessTokenKey, access);
    localStorage.setItem(this.refreshTokenKey, refresh);
  }

  checkAuthOnStartup(): void {
    const token = this.getAccessToken();
    if (token) {
      this.loggedIn.next(true);
    } else {
      this.loggedIn.next(false);
    }
  }
}

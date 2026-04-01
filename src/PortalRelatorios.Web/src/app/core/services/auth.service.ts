import { HttpClient } from '@angular/common/http';
import { inject, Injectable, computed, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginResponse, UserSummary } from '../models/auth.models';

const TOKEN_KEY = 'portal_token';
const USER_KEY = 'portal_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(null);
  private readonly _user = signal<UserSummary | null>(null);

  readonly token = computed(() => this._token());
  readonly user = computed(() => this._user());
  readonly isAdmin = computed(() => this._user()?.isAdmin ?? false);

  constructor() {
    const t = localStorage.getItem(TOKEN_KEY);
    const u = localStorage.getItem(USER_KEY);
    if (t && u) {
      try {
        this._token.set(t);
        this._user.set(JSON.parse(u) as UserSummary);
      } catch {
        this.clear();
      }
    }
  }

  isLoggedIn(): boolean {
    return !!this._token();
  }

  login(username: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, { username, password })
      .pipe(
        tap((r) => {
          localStorage.setItem(TOKEN_KEY, r.accessToken);
          localStorage.setItem(USER_KEY, JSON.stringify(r.user));
          this._token.set(r.accessToken);
          this._user.set(r.user);
        }),
      );
  }

  logout(): void {
    this.clear();
    void this.router.navigate(['/login']);
  }

  private clear(): void {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    this._token.set(null);
    this._user.set(null);
  }

  getToken(): string | null {
    return this._token();
  }
}

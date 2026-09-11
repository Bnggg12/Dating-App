import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { Login, Register, User } from '../../types/user';
import { tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;
  private refreshIntervalId: any = null;

  currentUser = signal<User | null>(null);

  register(payload: Register) {
    return this.http.post<User>(this.baseUrl + 'account/register', payload, {withCredentials: true}).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startRefreshToken();
        }
      })
    );
  }

  login(payload: Login) {
    return this.http.post<User>(this.baseUrl + 'account/login', payload, {withCredentials: true}).pipe(
      tap(user => {
        if (user) {
          this.setCurrentUser(user);
          this.startRefreshToken();
        }
      })
    );
  }

  refreshToken() {
    return this.http.post<User>(this.baseUrl + 'account/refresh-token', {}, {withCredentials: true})
  }

  startRefreshToken() {
    this.stopRefreshToken();
        
    this.refreshIntervalId = setInterval(() => {
        this.refreshToken().subscribe({
            next: user => {
                this.setCurrentUser(user);
            },
            error: () => {
                this.logout();
            }
        })
    }, 14 * 60 * 1000)
  }

  stopRefreshToken() {
      if (this.refreshIntervalId) {
          clearInterval(this.refreshIntervalId);
          this.refreshIntervalId = null;
      }
  }

  logout() {
    return this.http.post(this.baseUrl + 'account/logout', {}, {withCredentials: true}).subscribe({
      next: () => {
        this.currentUser.set(null);
      }
    })
  }

  setCurrentUser(user: User) {
    user.roles = this.getRolesFromToken(user);
    this.currentUser.set(user);
  }
  
  getMbtis() {
    return this.http.get<string[]>(this.baseUrl + 'account/mbtis');
  }

  getEduLevels() {
    return this.http.get<string[]>(this.baseUrl + 'account/edu-levels');
  }

  getCities() {
    return this.http.get<string[]>(this.baseUrl + 'account/cities');
  }

  private getRolesFromToken(user: User): string[] {
    const payload = user.token.split('.')[1];
    const decoded = atob(payload);
    const jsonPayload = JSON.parse(decoded);
    return Array.isArray(jsonPayload.role) ? jsonPayload.role : [jsonPayload.role];
  }
}

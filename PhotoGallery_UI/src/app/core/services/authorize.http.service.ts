import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environments';

@Injectable({
  providedIn: 'root',
})
export class AuthHttpService {
  private apiUrl = environment.apiUrl + 'auth';

  constructor(private http: HttpClient) {}

  postRegister(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, data);
  }

  postLogin(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, data);
  }

  postVerifyTwoFactorCode(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/verify-2fa`, data);
  }

  postRevokeRefreshToken(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/revoke-token`, data);
  }

  postRefreshToken(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/refresh-token`, data);
  }

  postRequestPasswordReset(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset-password-request`, data);
  }

  postResetPassword(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset-password`, data);
  }
}

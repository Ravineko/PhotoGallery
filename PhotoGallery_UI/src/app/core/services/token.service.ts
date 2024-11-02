import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class TokenService {
    private static readonly ACCESS_TOKEN_KEY = 'accessToken';
    private static readonly REFRESH_TOKEN_KEY = 'refreshToken';
    private roles: string[] = [];
    private userId: string = "";
  
    constructor() {
      const token = this.getToken();
      if (token) {
        this.parseRolesFromToken(token);
        this.parseUserIdFromToken(token);
      }
    }
  
    getToken(): string | null {
      return localStorage.getItem(TokenService.ACCESS_TOKEN_KEY);
    }
  
    getRefreshToken(): string | null {
      return localStorage.getItem(TokenService.REFRESH_TOKEN_KEY);
    }
  
    storeTokens(accessToken: string, refreshToken: string): void {
      localStorage.setItem(TokenService.ACCESS_TOKEN_KEY, accessToken);
      localStorage.setItem(TokenService.REFRESH_TOKEN_KEY, refreshToken);
      this.parseRolesFromToken(accessToken);
      this.parseUserIdFromToken(accessToken);
    }
  
    clearTokens(): void {
      localStorage.removeItem(TokenService.ACCESS_TOKEN_KEY);
      localStorage.removeItem(TokenService.REFRESH_TOKEN_KEY);
      this.roles = []; 
      this.userId = '';
    }
  
    private parseRolesFromToken(token: string): void {
      try {
        const decodedToken: any = jwtDecode(token);
        this.roles = decodedToken.role || [];
      } catch (error) {
        console.error('Error decoding token', error);
        this.roles = [];
      }
    }

    private parseUserIdFromToken(token: string): void {
      try {
        const decodedToken: any = jwtDecode(token);
        this.userId = decodedToken.oid || '';
      } catch (error) {
        console.error('Error decoding token', error);
        this.userId = "";
      }
    }
  
    isLoggedIn(): boolean {
      return !!this.getToken();
    }
  
    hasRole(role: string): boolean {
      return this.roles.includes(role);
    }

    getUserId(): string {
      return this.userId;
    }
  }

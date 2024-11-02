import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { map, catchError } from 'rxjs/operators';
import { Observable, of, throwError } from 'rxjs';
import { TokenService } from './token.service';
import { AuthHttpService } from './authorize.http.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private email: string | null = null;

  constructor(
    private authHttpService: AuthHttpService,
    private tokenService: TokenService,
    private router: Router
  ) {}

  register(
    userName: string,
    email: string,
    password: string,
    phoneNumber: string
  ): Observable<any> {
    return this.authHttpService
      .postRegister({ userName, email, password, phoneNumber })
      .pipe(
        map((response) =>
          response.isSuccess
            ? { status: 'Created' }
            : { status: 'Fail', message: response.errorMessages.join(', ') }
        ),
        catchError((error) =>
          throwError(() => new Error('An unexpected error occurred.' + error))
        )
      );
  }

  login(email: string, password: string): Observable<any> {
    this.email = email;
    return this.authHttpService.postLogin({ email, password }).pipe(
      map((response) => {
        if (response.isSuccess) {
          const result = response.result;
          if (result.twoFactorNeeded) {
            this.router.navigate(['/2fa']);
            return { status: 'TwoFactorRequired' };
          } else {
            const { accessToken, refreshToken } = result;
            if (accessToken && refreshToken) {
              this.tokenService.storeTokens(accessToken, refreshToken);
              return { status: 'Success', accessToken, refreshToken };
            } else {
              return { status: 'Fail', message: 'Unexpected login response.' };
            }
          }
        } else {
          return {
            status: 'Fail',
            message: response.errorMessages?.join(', ') || 'An error occurred',
          };

        }
      }),
      catchError(() =>
        of({ status: 'Error', message: 'An unexpected error occurred.' })
      )
    );
  }

  verifyTwoFactorCode(code: string): Observable<any> {
    return this.authHttpService
      .postVerifyTwoFactorCode({ code, email: this.email })
      .pipe(
        map((response) => {
          if (response.isSuccess) {
            const { accessToken, refreshToken } = response.result;
            this.tokenService.storeTokens(accessToken, refreshToken);
            this.router.navigate(['/']);
            return { status: 'Success', accessToken, refreshToken };
          } else {
            return {
              status: 'Fail',
              message: response.errorMessages?.join(', ') || 'Invalid code',
            };
          }
        }),
        catchError(() =>
          of({ status: 'Error', message: 'An unexpected error occurred.' })
        )
      );
  }

  logout(): void {
    const refreshToken = this.tokenService.getRefreshToken();
    this.authHttpService
      .postRevokeRefreshToken({ refreshToken })
      .pipe(
        map((response) => {
          if (response.isSuccess) {
            this.tokenService.clearTokens();
            this.router.navigate(['/login']);
            return { status: 'Token revoked' };
          } else {
            return {
              status: 'Fail',
              message: response.errorMessages.join(', '),
            };
          }
        }),
        catchError(() =>
          of({ status: 'Error', message: 'An unexpected error occurred.' })
        )
      )
      .subscribe();
  }

  refreshToken(): Observable<boolean> {
    const accessToken = this.tokenService.getToken();
    const refreshToken = this.tokenService.getRefreshToken();

    if (!refreshToken) {
      return of(false);
    }

    return this.authHttpService
      .postRefreshToken({ accessToken, refreshToken })
      .pipe(
        map((response) => {
          if (response.isSuccess && response.result) {
            const { accessToken, refreshToken } = response.result;
            this.tokenService.storeTokens(accessToken, refreshToken);
            return true;
          } else {
            this.tokenService.clearTokens();
            return false;
          }
        }),
        catchError(() => {
          this.tokenService.clearTokens();
          return of(false);
        })
      );
  }

  requestPasswordReset(data: { email: string }): Observable<any> {
    return this.authHttpService.postRequestPasswordReset(data);
  }

  resetPassword(data: { token: string; newPassword: string }): Observable<any> {
    return this.authHttpService.postResetPassword(data);
  }
}

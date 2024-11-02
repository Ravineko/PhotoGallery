import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../services/authorize.service';
import { Router } from '@angular/router';
import { TokenService } from '../services/token.service';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private refreshing = false;

  constructor(private authService: AuthService, 
    private tokenService: TokenService, 
    private router: Router,
    private toastr: ToastrService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.tokenService.getToken();
    if (token) {
      req = this.addToken(req, token);
    }
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 0) {
          return throwError(() => new Error('Cannot connect to the server. Please check your internet connection.'));
        }

        if (error.status === 401) {
          if (error.error?.errorMessages?.includes('Invalid Credentials')) {
            return of(new HttpResponse({
              body: { isSuccess: false, errorMessages: ['Invalid Credentials'], result: null },
              status: 200
            }));
          }

          if (!this.refreshing && this.tokenService.getRefreshToken()) {
            return this.handleAuthError(req, next);
          } else {
            return this.handleFailedRefresh();
          }
        }

        return throwError(() => error);
      })
    );
  }

  private addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  private handleAuthError(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.refreshing = true;

    return this.authService.refreshToken().pipe(
      switchMap(success => {
        if (success) {
          const newToken = this.tokenService.getToken();
          this.refreshing = false;
          return next.handle(this.addToken(req, newToken!));
        } else {
          return this.handleFailedRefresh();
        }
      }),
      catchError(() => this.handleFailedRefresh())
    );
  }

  private handleFailedRefresh(): Observable<never> {
    this.refreshing = false;
    this.tokenService.clearTokens();
    this.router.navigate(['/login']);
    this.toastr.error('Refresh token expired','Unauthorized')
    return throwError(() => new Error('Unauthorized: Refresh token expired'));
  }
}

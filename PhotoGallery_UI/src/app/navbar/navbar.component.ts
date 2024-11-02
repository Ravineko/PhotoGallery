import { Component } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { TokenService } from '../core/services/token.service';
import { AuthService } from '../core/services/authorize.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent {
  isMenuOpen = false;

  constructor(
    private tokenService: TokenService,
    private router: Router,
    private authService: AuthService
  ) {
  }

  toggleMenu() {
    this.isMenuOpen = !this.isMenuOpen;
  }

  isLoggedIn(): boolean {
    return this.tokenService.isLoggedIn();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isUser(): boolean {
    return this.tokenService.hasRole('User');
  }
}

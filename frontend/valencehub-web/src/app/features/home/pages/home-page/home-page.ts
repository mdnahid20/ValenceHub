import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthSessionService } from '../../../auth/services/auth-session.service';
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-home-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
})
export class HomePageComponent {
  private readonly authSessionService = inject(AuthSessionService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  isSubmitting = false;
  isAuthenticated = !!this.authSessionService.getAccessToken();

  logout(): void {
    const refreshToken = this.authSessionService.getRefreshToken();
    if (!refreshToken || this.isSubmitting) {
      return;
    }

    this.isSubmitting = true;
    this.authService
      .logout({ refreshToken })
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.authSessionService.clearSession();
          this.isAuthenticated = false;
          void this.router.navigateByUrl('/auth/login');
        },
        error: () => {
          this.authSessionService.clearSession();
          this.isAuthenticated = false;
        },
      });
  }
}

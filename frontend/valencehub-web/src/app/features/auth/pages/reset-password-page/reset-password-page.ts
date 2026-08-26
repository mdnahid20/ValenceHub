import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import { OtpPurpose } from '../../models/auth.models';
import { AuthSessionService } from '../../services/auth-session.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reset-password-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './reset-password-page.html',
  styleUrl: './reset-password-page.scss',
})
export class ResetPasswordPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly authSessionService = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly pendingContext = this.authSessionService.getPendingVerification();
  readonly resetPasswordForm = this.formBuilder.nonNullable.group(
    {
      newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(255)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: [passwordsMatch] },
  );

  isSubmitting = false;
  errorMessage = '';
  successMessage = '';

  submit(): void {
    if (!this.pendingContext || this.pendingContext.purpose !== OtpPurpose.ForgotPassword) {
      this.errorMessage = 'Password reset context is missing. Start from forgot password.';
      return;
    }

    if (!this.pendingContext.token) {
      this.errorMessage = 'Verification token missing. Verify OTP again.';
      return;
    }

    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService
      .forgotPassword({
        userId: this.pendingContext.userId,
        verificationToken: this.pendingContext.token,
        newPassword: this.resetPasswordForm.controls.newPassword.getRawValue(),
      })
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: () => {
          this.authSessionService.clearPendingVerification();
          this.successMessage = 'Password reset successful. Redirecting to login...';
          setTimeout(() => {
            void this.router.navigateByUrl('/auth/login');
          }, 800);
        },
        error: (error: unknown) => {
          this.errorMessage = this.getErrorMessage(error, 'Unable to reset password.');
        },
      });
  }

  private getErrorMessage(error: unknown, fallback: string): string {
    if (error instanceof HttpErrorResponse) {
      if (typeof error.error?.detail === 'string' && error.error.detail.trim().length > 0) {
        return error.error.detail;
      }

      if (typeof error.error?.title === 'string' && error.error.title.trim().length > 0) {
        return error.error.title;
      }
    }

    if (error instanceof Error && error.message.trim().length > 0) {
      return error.message;
    }

    return fallback;
  }
}

function passwordsMatch(control: AbstractControl): Record<string, true> | null {
  const newPassword = control.get('newPassword')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return newPassword === confirmPassword ? null : { passwordsMismatch: true };
}

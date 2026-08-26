import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, map, Observable, of, switchMap } from 'rxjs';

import {
  OtpPurpose,
  PendingVerificationContext,
  VerifyOtpResponse,
} from '../../models/auth.models';
import { AuthSessionService } from '../../services/auth-session.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-verify-otp-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './verify-otp-page.html',
  styleUrl: './verify-otp-page.scss',
})
export class VerifyOtpPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly authSessionService = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly verifyOtpForm = this.formBuilder.nonNullable.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]],
  });

  readonly pendingContext = this.authSessionService.getPendingVerification();
  isSubmitting = false;
  isResending = false;
  errorMessage = '';
  infoMessage = '';

  submit(): void {
    if (!this.pendingContext) {
      this.errorMessage = 'Verification context not found. Please restart the flow.';
      return;
    }

    if (this.verifyOtpForm.invalid) {
      this.verifyOtpForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';
    this.infoMessage = '';

    const payload = {
      target: this.pendingContext.target,
      channel: this.pendingContext.channel,
      purpose: this.pendingContext.purpose,
      code: this.verifyOtpForm.controls.code.getRawValue(),
    };

    this.authService
      .verifyOtp(payload)
      .pipe(
        switchMap((verificationResponse) =>
          this.handleSuccessfulVerification(verificationResponse, this.pendingContext!),
        ),
        finalize(() => (this.isSubmitting = false)),
      )
      .subscribe({
        next: (route) => {
          void this.router.navigateByUrl(route);
        },
        error: (error: unknown) => {
          this.errorMessage = this.getErrorMessage(error, 'OTP verification failed.');
        },
      });
  }

  resendCode(): void {
    if (!this.pendingContext || this.isResending) {
      return;
    }

    this.isResending = true;
    this.errorMessage = '';
    this.infoMessage = '';

    this.authService
      .resendOtp({
        targetValue: this.pendingContext.target,
        channel: this.pendingContext.channel,
        purpose: this.pendingContext.purpose,
      })
      .pipe(finalize(() => (this.isResending = false)))
      .subscribe({
        next: () => {
          this.infoMessage = 'A new verification code has been sent.';
        },
        error: (error: unknown) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to resend verification code right now.',
          );
        },
      });
  }

  private handleSuccessfulVerification(
    verificationResponse: VerifyOtpResponse,
    context: PendingVerificationContext,
  ): Observable<string> {
    if (context.purpose === OtpPurpose.Register) {
      return this.authService
        .completeRegistration({
          userId: verificationResponse.userId,
          token: verificationResponse.token,
        })
        .pipe(
          map(() => {
            this.authSessionService.clearPendingVerification();
            return '/auth/login';
          }),
        );
    }

    if (context.purpose === OtpPurpose.ForgotPassword) {
      this.authSessionService.setPendingVerification({
        ...context,
        userId: verificationResponse.userId,
        token: verificationResponse.token,
      });
      return of('/auth/reset-password');
    }

    this.authSessionService.clearPendingVerification();
    return of('/auth/login');
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

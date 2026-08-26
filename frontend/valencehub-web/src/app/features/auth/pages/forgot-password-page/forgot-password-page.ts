import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';

import {
  CommunicationChannel,
  OtpPurpose,
  PendingVerificationContext,
} from '../../models/auth.models';
import { AuthSessionService } from '../../services/auth-session.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-forgot-password-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.scss',
})
export class ForgotPasswordPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly authSessionService = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly forgotPasswordForm = this.formBuilder.nonNullable.group({
    target: ['', [Validators.required, Validators.maxLength(320)]],
  });

  isSubmitting = false;
  errorMessage = '';

  submit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      return;
    }

    const target = this.forgotPasswordForm.controls.target.getRawValue().trim();
    const channel = resolveChannelFromTarget(target);

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService
      .sendOtp({
        target,
        channel,
        purpose: OtpPurpose.ForgotPassword,
      })
      .pipe(finalize(() => (this.isSubmitting = false)))
      .subscribe({
        next: (response) => {
          const pendingVerification: PendingVerificationContext = {
            userId: response.userId,
            target,
            channel,
            purpose: OtpPurpose.ForgotPassword,
          };
          this.authSessionService.setPendingVerification(pendingVerification);
          void this.router.navigateByUrl('/auth/verify-otp');
        },
        error: (error: unknown) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to send verification code to that target.',
          );
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

function resolveChannelFromTarget(target: string): CommunicationChannel {
  return target.includes('@') ? CommunicationChannel.Email : CommunicationChannel.Sms;
}

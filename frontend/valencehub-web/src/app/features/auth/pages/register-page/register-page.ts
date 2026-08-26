import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { finalize, map, switchMap } from 'rxjs';

import {
  CommunicationChannel,
  OtpPurpose,
  PendingVerificationContext,
  RegisterRequest,
} from '../../models/auth.models';
import { AuthSessionService } from '../../services/auth-session.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register-page.html',
  styleUrl: './register-page.scss',
})
export class RegisterPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly authSessionService = inject(AuthSessionService);
  private readonly router = inject(Router);

  readonly registerForm = this.formBuilder.nonNullable.group(
    {
      email: ['', [Validators.email, Validators.maxLength(320)]],
      phoneNumber: ['', [Validators.maxLength(20)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(255)]],
    },
    { validators: [requireEmailOrPhone] },
  );

  isSubmitting = false;
  errorMessage = '';

  submit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const rawValue = this.registerForm.getRawValue();
    const email = normalizeOptionalValue(rawValue.email);
    const phoneNumber = normalizeOptionalValue(rawValue.phoneNumber);
    const target = email ?? phoneNumber;

    if (!target) {
      this.errorMessage = 'Provide either an email or phone number.';
      return;
    }

    const channel =
      email !== null ? CommunicationChannel.Email : CommunicationChannel.Sms;

    const registerPayload: RegisterRequest = {
      email,
      phoneNumber,
      password: rawValue.password,
    };

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService
      .register(registerPayload)
      .pipe(
        switchMap((registerResponse) =>
          this.authService.sendOtp({
            target,
            channel,
            purpose: OtpPurpose.Register,
          }).pipe(
            map((otpResponse) => ({
              registerUserId: registerResponse.userId,
              otpUserId: otpResponse.userId,
            })),
          ),
        ),
        finalize(() => (this.isSubmitting = false)),
      )
      .subscribe({
        next: ({ registerUserId, otpUserId }) => {
          const pendingVerification: PendingVerificationContext = {
            userId: otpUserId || registerUserId,
            target,
            channel,
            purpose: OtpPurpose.Register,
          };
          this.authSessionService.setPendingVerification(pendingVerification);
          void this.router.navigateByUrl('/auth/verify-otp');
        },
        error: (error: unknown) => {
          this.errorMessage = this.getErrorMessage(
            error,
            'Unable to create your account right now.',
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

function requireEmailOrPhone(control: AbstractControl): Record<string, true> | null {
  const email = normalizeOptionalValue(control.get('email')?.value);
  const phoneNumber = normalizeOptionalValue(control.get('phoneNumber')?.value);
  return email || phoneNumber ? null : { requireEmailOrPhone: true };
}

function normalizeOptionalValue(value: string | null | undefined): string | null {
  const normalized = value?.trim();
  return normalized ? normalized : null;
}

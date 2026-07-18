import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_BASE_URL } from '../../../core/tokens/api-base-url.token';
import {
  CompleteRegistrationRequest,
  ForgotPasswordRequest,
  LoginRequest,
  LoginResponse,
  LogoutRequest,
  OtpDeliveryResponse,
  RefreshTokenRequest,
  RefreshTokenResponse,
  RegisterRequest,
  RegisterResponse,
  ResendOtpRequest,
  SendOtpRequest,
  VerifyOtpRequest,
  VerifyOtpResponse,
} from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);
  private readonly authBaseUrl = `${this.apiBaseUrl}/auth`;

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.authBaseUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.authBaseUrl}/login`, request);
  }

  sendOtp(request: SendOtpRequest): Observable<OtpDeliveryResponse> {
    return this.http.post<OtpDeliveryResponse>(`${this.authBaseUrl}/send-otp`, request);
  }

  resendOtp(request: ResendOtpRequest): Observable<OtpDeliveryResponse> {
    return this.http.post<OtpDeliveryResponse>(`${this.authBaseUrl}/resend-otp`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<VerifyOtpResponse> {
    return this.http.post<VerifyOtpResponse>(`${this.authBaseUrl}/verify-otp`, request);
  }

  completeRegistration(request: CompleteRegistrationRequest): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/complete-registration`, request);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/forgot-password`, request);
  }

  refreshToken(request: RefreshTokenRequest): Observable<RefreshTokenResponse> {
    return this.http.post<RefreshTokenResponse>(`${this.authBaseUrl}/refresh`, request);
  }

  logout(request: LogoutRequest): Observable<void> {
    return this.http.post<void>(`${this.authBaseUrl}/logout`, request);
  }
}

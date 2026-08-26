import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

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

interface ApiErrorResponse {
  code: string;
  message: string;
  metadata?: unknown;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  error: ApiErrorResponse | null;
  traceId: string;
  timestampUtc: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiBaseUrl = inject(API_BASE_URL);
  private readonly authBaseUrl = `${this.apiBaseUrl}/auth`;

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.unwrapResponse(this.http.post<ApiResponse<RegisterResponse>>(`${this.authBaseUrl}/register`, request));
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.unwrapResponse(this.http.post<ApiResponse<LoginResponse>>(`${this.authBaseUrl}/login`, request));
  }

  sendOtp(request: SendOtpRequest): Observable<OtpDeliveryResponse> {
    return this.unwrapResponse(this.http.post<ApiResponse<OtpDeliveryResponse>>(`${this.authBaseUrl}/send-otp`, request));
  }

  resendOtp(request: ResendOtpRequest): Observable<OtpDeliveryResponse> {
    return this.unwrapResponse(this.http.post<ApiResponse<OtpDeliveryResponse>>(`${this.authBaseUrl}/resend-otp`, request));
  }

  verifyOtp(request: VerifyOtpRequest): Observable<VerifyOtpResponse> {
    return this.unwrapResponse(this.http.post<ApiResponse<VerifyOtpResponse>>(`${this.authBaseUrl}/verify-otp`, request));
  }

  completeRegistration(request: CompleteRegistrationRequest): Observable<void> {
    return this.unwrapVoidResponse(
      this.http.post<ApiResponse<null>>(`${this.authBaseUrl}/complete-registration`, request),
    );
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    return this.unwrapVoidResponse(this.http.post<ApiResponse<null>>(`${this.authBaseUrl}/forgot-password`, request));
  }

  refreshToken(request: RefreshTokenRequest): Observable<RefreshTokenResponse> {
    return this.unwrapResponse(
      this.http.post<ApiResponse<RefreshTokenResponse>>(`${this.authBaseUrl}/refresh`, request),
    );
  }

  logout(request: LogoutRequest): Observable<void> {
    return this.unwrapVoidResponse(this.http.post<ApiResponse<null>>(`${this.authBaseUrl}/logout`, request));
  }

  private unwrapResponse<T>(response$: Observable<ApiResponse<T>>): Observable<T> {
    return response$.pipe(
      map((response) => {
        if (!response.success) {
          throw new Error(response.error?.message ?? 'Request failed.');
        }

        return response.data;
      }),
    );
  }

  private unwrapVoidResponse(response$: Observable<ApiResponse<null>>): Observable<void> {
    return this.unwrapResponse(response$).pipe(map(() => undefined));
  }
}

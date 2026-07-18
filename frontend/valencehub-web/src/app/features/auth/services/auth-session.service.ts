import { Injectable } from '@angular/core';

import { AuthTokens, PendingVerificationContext } from '../models/auth.models';

const AUTH_TOKENS_STORAGE_KEY = 'valencehub.auth.tokens';
const AUTH_PENDING_VERIFICATION_STORAGE_KEY = 'valencehub.auth.pending-verification';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  getTokens(): AuthTokens | null {
    const rawValue = localStorage.getItem(AUTH_TOKENS_STORAGE_KEY);
    return rawValue ? (JSON.parse(rawValue) as AuthTokens) : null;
  }

  setTokens(tokens: AuthTokens): void {
    localStorage.setItem(AUTH_TOKENS_STORAGE_KEY, JSON.stringify(tokens));
  }

  clearTokens(): void {
    localStorage.removeItem(AUTH_TOKENS_STORAGE_KEY);
  }

  getAccessToken(): string | null {
    return this.getTokens()?.accessToken ?? null;
  }

  getRefreshToken(): string | null {
    return this.getTokens()?.refreshToken ?? null;
  }

  getPendingVerification(): PendingVerificationContext | null {
    const rawValue = localStorage.getItem(AUTH_PENDING_VERIFICATION_STORAGE_KEY);
    return rawValue ? (JSON.parse(rawValue) as PendingVerificationContext) : null;
  }

  setPendingVerification(context: PendingVerificationContext): void {
    localStorage.setItem(AUTH_PENDING_VERIFICATION_STORAGE_KEY, JSON.stringify(context));
  }

  clearPendingVerification(): void {
    localStorage.removeItem(AUTH_PENDING_VERIFICATION_STORAGE_KEY);
  }

  clearSession(): void {
    this.clearTokens();
    this.clearPendingVerification();
  }
}

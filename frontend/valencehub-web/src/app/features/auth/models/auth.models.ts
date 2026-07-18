export enum CommunicationChannel {
  Email = 1,
  Sms = 2,
  WhatsApp = 3,
  AuthenticatorApp = 4,
}

export enum OtpPurpose {
  Register = 1,
  Login = 2,
  ForgotPassword = 3,
}

export interface RegisterRequest {
  email: string;
  phoneNumber?: string | null;
  password: string;
}

export interface RegisterResponse {
  userId: string;
}

export interface LoginRequest {
  loginId: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
}

export interface SendOtpRequest {
  target: string;
  channel: CommunicationChannel;
  purpose: OtpPurpose;
}

export interface ResendOtpRequest {
  targetValue: string;
  channel: CommunicationChannel;
  purpose: OtpPurpose;
}

export interface OtpDeliveryResponse {
  success: boolean;
  userId: string;
}

export interface VerifyOtpRequest {
  target: string;
  channel: CommunicationChannel;
  purpose: OtpPurpose;
  code: string;
}

export interface VerifyOtpResponse {
  userId: string;
  token: string;
}

export interface CompleteRegistrationRequest {
  userId: string;
  token: string;
}

export interface ForgotPasswordRequest {
  userId: string;
  verificationToken: string;
  newPassword: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  userId: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
}

export interface LogoutRequest {
  refreshToken: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
}

export interface PendingVerificationContext {
  userId: string;
  target: string;
  channel: CommunicationChannel;
  purpose: OtpPurpose;
  token?: string;
}

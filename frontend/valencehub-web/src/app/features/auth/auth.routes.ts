import { Routes } from '@angular/router';

export const authRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/auth-layout/auth-layout').then((m) => m.AuthLayoutComponent),
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'login',
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/login-page/login-page').then((m) => m.LoginPageComponent),
        title: 'Login | ValenceHub',
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./pages/register-page/register-page').then((m) => m.RegisterPageComponent),
        title: 'Register | ValenceHub',
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./pages/forgot-password-page/forgot-password-page').then(
            (m) => m.ForgotPasswordPageComponent,
          ),
        title: 'Forgot Password | ValenceHub',
      },
      {
        path: 'verify-otp',
        loadComponent: () =>
          import('./pages/verify-otp-page/verify-otp-page').then((m) => m.VerifyOtpPageComponent),
        title: 'Verify OTP | ValenceHub',
      },
      {
        path: 'reset-password',
        loadComponent: () =>
          import('./pages/reset-password-page/reset-password-page').then(
            (m) => m.ResetPasswordPageComponent,
          ),
        title: 'Reset Password | ValenceHub',
      },
    ],
  },
];

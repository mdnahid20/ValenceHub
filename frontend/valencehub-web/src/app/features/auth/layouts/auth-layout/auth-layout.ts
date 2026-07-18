import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.scss',
})
export class AuthLayoutComponent {
  protected readonly links = [
    { label: 'Login', path: '/auth/login' },
    { label: 'Register', path: '/auth/register' },
    { label: 'Forgot Password', path: '/auth/forgot-password' },
  ];
}

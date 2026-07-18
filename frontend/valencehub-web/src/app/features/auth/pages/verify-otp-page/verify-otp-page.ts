import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-verify-otp-page',
  imports: [CommonModule, RouterLink],
  templateUrl: './verify-otp-page.html',
  styleUrl: './verify-otp-page.scss',
})
export class VerifyOtpPageComponent {}

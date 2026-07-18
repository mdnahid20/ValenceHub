import { Routes } from '@angular/router';
import { authRoutes } from './features/auth/auth.routes';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/pages/home-page/home-page').then((m) => m.HomePageComponent),
    title: 'ValenceHub',
  },
  {
    path: 'auth',
    children: authRoutes,
  },
  {
    path: '**',
    redirectTo: '',
  },
];

import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./pages/main-hub/main-hub.page').then(m => m.MainHubPageComponent)
    },
    {
        path: 'analytics-map',
        loadComponent: () => import('./pages/analytics-map/analytics-map.page').then(m => m.AnalyticsMapPageComponent)
    },
    {
        path: 'map',
        loadComponent: () => import('./pages/map/map.page').then(m => m.MapPageComponent)
    },
    {
        path: 'admin/auth',
        loadComponent: () => import('./pages/admin/auth/auth.page').then(m => m.AuthPageComponent)
    },
    {
        path: 'admin',
        loadComponent: () => import('./pages/admin/admin-hub/admin-hub.page').then(m => m.AdminHubPageComponent)
    }
];

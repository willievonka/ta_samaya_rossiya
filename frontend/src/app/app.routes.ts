import { Routes } from '@angular/router';
import { adminAuthGuard } from './guards/admin-auth.guard';
import { canDeactivateGuard } from './guards/can-deactivate.guard';

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
        canActivate: [adminAuthGuard],
        loadComponent: () => import('./pages/admin/admin-hub/admin-hub.page').then(m => m.AdminHubPageComponent)
    },
    {
        path: 'admin/edit-analytics-map',
        canActivate: [adminAuthGuard],
        canDeactivate: [canDeactivateGuard],
        loadComponent: () => import('./pages/admin/edit-analytics-map/edit-analytics-map.page').then(m => m.EditAnalyticsMapPageComponent)
    },
    {
        path: 'admin/edit-map',
        canActivate: [adminAuthGuard],
        canDeactivate: [canDeactivateGuard],
        loadComponent: () => import('./pages/admin/edit-map/edit-map.page').then(m => m.EditMapPageComponent)
    }
];

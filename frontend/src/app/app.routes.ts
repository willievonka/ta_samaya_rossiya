import { Routes } from '@angular/router';
import { adminAuthGuard } from './guards/admin-auth.guard';
import { canDeactivateGuard } from './guards/can-deactivate.guard';
import { DesktopGuard } from './guards/desktop.guard';

export const routes: Routes = [
    {
        path: '',
        canActivate: [DesktopGuard],
        loadComponent: () => import('./pages/main-hub/main-hub.page').then(m => m.MainHubPageComponent)
    },
    {
        path: 'analytics-map',
        canActivate: [DesktopGuard],
        loadComponent: () => import('./pages/analytics-map/analytics-map.page').then(m => m.AnalyticsMapPageComponent)
    },
    {
        path: 'map',
        canActivate: [DesktopGuard],
        loadComponent: () => import('./pages/map/map.page').then(m => m.MapPageComponent)
    },

    {
        path: 'admin/auth',
        canActivate: [DesktopGuard],
        loadComponent: () => import('./pages/admin/auth/auth.page').then(m => m.AuthPageComponent)
    },
    {
        path: 'admin',
        canActivate: [adminAuthGuard, DesktopGuard],
        loadComponent: () => import('./pages/admin/admin-hub/admin-hub.page').then(m => m.AdminHubPageComponent)
    },
    {
        path: 'admin/edit-analytics-map',
        canActivate: [adminAuthGuard, DesktopGuard],
        canDeactivate: [canDeactivateGuard],
        loadComponent: () => import('./pages/admin/edit-analytics-map/edit-analytics-map.page').then(m => m.EditAnalyticsMapPageComponent)
    },
    {
        path: 'admin/edit-map',
        canActivate: [adminAuthGuard, DesktopGuard],
        canDeactivate: [canDeactivateGuard],
        loadComponent: () => import('./pages/admin/edit-map/edit-map.page').then(m => m.EditMapPageComponent)
    },
    {
        path: 'admin/create-map',
        canActivate: [adminAuthGuard, DesktopGuard],
        canDeactivate: [canDeactivateGuard],
        loadComponent: () => import('./pages/admin/edit-map/edit-map.page').then(m => m.EditMapPageComponent)
    },
    {
        path: 'mobile-device',
        loadComponent: () => import('./pages/mobile-device/mobile-device.page').then(m => m.MobileDevicePageComponent)
    },
    {
        path: '**',
        redirectTo: ''
    }
];

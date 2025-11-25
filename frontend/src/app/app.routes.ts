import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./pages/main-hub/main-hub.page').then(m => m.MainHubPageComponent)
    }
];

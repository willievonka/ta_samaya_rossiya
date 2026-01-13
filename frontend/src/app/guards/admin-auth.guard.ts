import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { map, catchError, of } from 'rxjs';

export const adminAuthGuard: CanActivateFn = () => {
    const authService: AuthService = inject(AuthService);
    const router: Router = inject(Router);

    if (!authService.checkAuth()) {
        return router.createUrlTree(['/admin/auth']);
    }

    return authService.refreshToken()
        .pipe(
            map(() => true),
            catchError(() => {
                authService.forceLogoutLocal();

                return of(router.createUrlTree(['/admin/auth']));
            })
        );
};

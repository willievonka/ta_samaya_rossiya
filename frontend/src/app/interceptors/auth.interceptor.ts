import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn) => {
    const authService: AuthService = inject(AuthService);
    const router: Router = inject(Router);

    const isEndpointWithAuth: boolean = request.url.includes('/admin/') && !request.url.includes('/auth/login');
    const token: string | null = authService.getToken();

    const requestToSend: HttpRequest<unknown> = isEndpointWithAuth && token
        ? request.clone({ headers: request.headers.set('Authorization', `Bearer ${token}`) })
        : request;

    return next(requestToSend)
        .pipe(
            catchError((error: unknown) => {
                if (error instanceof HttpErrorResponse && (error.status === 401 || error.status === 403)) {
                    authService.forceLogoutLocal();
                    router.navigate(['/admin/auth']);
                }

                return throwError(() => error);
            })
        );
};

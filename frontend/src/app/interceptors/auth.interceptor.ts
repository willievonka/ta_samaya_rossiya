import { HttpErrorResponse, HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';

const retriedRequests: WeakMap<HttpRequest<unknown>, boolean> = new WeakMap<HttpRequest<unknown>, boolean>();

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
    const authService: AuthService = inject(AuthService);
    const router: Router = inject(Router);

    const token: string | null = authService.getAccessToken();
    const isProtected: boolean = req.url.includes('/admin/') && !req.url.includes('/auth/login');
    const alreadyRetried: boolean = retriedRequests.get(req) ?? false;
    const requestToSend: HttpRequest<unknown> = isProtected && token
        ? req.clone({ headers: req.headers.set('Authorization', `Bearer ${token}`) })
        : req;

    const logout: () => void = () => {
        authService.forceLogoutLocal();
        router.navigate(['/admin/auth']);
    };

    return next(requestToSend).pipe(
        catchError((error: unknown) => {
            if (!(error instanceof HttpErrorResponse)) {
                return throwError(() => error);
            }

            if (error.status === 403 && isProtected) {
                logout();

                return throwError(() => error);
            }

            if (error.status === 401 && isProtected && !alreadyRetried && !req.url.includes('/auth/refresh')) {
                retriedRequests.set(req, true);

                return authService.refreshToken().pipe(
                    switchMap(() => {
                        const newToken: string | null = authService.getAccessToken();
                        const retriedReq: HttpRequest<unknown> = newToken
                            ? req.clone({ headers: req.headers.set('Authorization', `Bearer ${newToken}`) })
                            : req;
                        retriedRequests.set(retriedReq, true);

                        return next(retriedReq);
                    }),
                    catchError(() => {
                        logout();

                        return throwError(() => error);
                    })
                );
            }

            return throwError(() => error);
        })
    );
};

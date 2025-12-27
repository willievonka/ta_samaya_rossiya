import { provideEventPlugins } from '@taiga-ui/event-plugins';
import { provideAnimations } from '@angular/platform-browser/animations';
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { TUI_LANGUAGE, TUI_RUSSIAN_LANGUAGE } from '@taiga-ui/i18n';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { of } from 'rxjs';
import { authInterceptor } from './interceptors/auth.interceptor';
import { tuiValidationErrorsProvider } from '@taiga-ui/kit';

export const appConfig: ApplicationConfig = {
    providers: [
        provideAnimations(),
        provideBrowserGlobalErrorListeners(),
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(routes),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideEventPlugins(),
        { provide: TUI_LANGUAGE, useValue: of(TUI_RUSSIAN_LANGUAGE) },
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения',
            email: 'Неверный формат электронной почты',
            invalidCredits: 'Неверный email или пароль',
            serverError: 'Сервис временно недоступен. Попробуйте позже'
        })
    ]
};

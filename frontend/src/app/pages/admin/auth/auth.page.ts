import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { TuiTextfield, TuiButton, TuiIcon, TuiError } from '@taiga-ui/core';
import { TuiFieldErrorPipe, TuiPassword, tuiValidationErrorsProvider } from '@taiga-ui/kit';
import { IAuthModel } from './interfaces/auth-model.interface';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AsyncPipe } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

@Component({
    selector: 'auth-page',
    standalone: true,
    templateUrl: './auth.page.html',
    styleUrl: './styles/auth-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        AsyncPipe,
        ReactiveFormsModule,
        TuiTextfield,
        TuiButton,
        TuiIcon,
        TuiPassword,
        TuiError,
        TuiFieldErrorPipe,
        RouterLink
    ],
    providers: [
        tuiValidationErrorsProvider({
            required: 'Поле обязательно для заполнения'
        })
    ]
})
export class AuthPageComponent {
    protected readonly authForm: FormGroup = new FormGroup<IAuthModel>(
        {
            'login': new FormControl<string | null>(null, [Validators.required]),
            'password': new FormControl<string | null>(null, [Validators.required])
        },
        Validators.required
    );
    private readonly _authService: AuthService = inject(AuthService);
    private readonly _router: Router = inject(Router);

    /** Войти в аккаунт */
    protected login(): void {
        const login: string = this.authForm.controls['login'].value;
        const password: string = this.authForm.controls['password'].value;

        this.authForm.markAllAsTouched();

        if (this.authForm.valid) {
            this._authService.login(login, password);
            this._router.navigate(['admin']);
        }
    }
}

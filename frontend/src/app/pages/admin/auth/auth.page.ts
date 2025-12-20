import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'auth-page',
    standalone: true,
    templateUrl: './auth.page.html',
    styleUrl: './styles/auth-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthPageComponent {

}

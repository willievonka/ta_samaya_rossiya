import { inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { DeviceService } from '../services/device.service';

@Injectable({ providedIn: 'root' })
export class DesktopGuard implements CanActivate {
    private readonly _deviceService: DeviceService = inject(DeviceService);
    private readonly _router: Router = inject(Router);

    /** @inheritdoc */
    public canActivate(): boolean {
        if (this._deviceService.isMobile()) {
            this._router.navigate(['mobile-device']);

            return false;
        }

        return true;
    }
}

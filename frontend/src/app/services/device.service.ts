import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DeviceService {
    /** Проверить, является ли устройство юзера мобильным */
    public isMobile(): boolean {
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        const userAgent: string = navigator.userAgent || navigator.vendor || (window as any).opera;

        return /android|iphone|ipad|iPod|windows phone/i.test(userAgent.toLowerCase());
    }
}

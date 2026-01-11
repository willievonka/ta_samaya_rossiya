import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'mobile-device-page',
    standalone: true,
    templateUrl: './mobile-device.page.html',
    styleUrl: './styles/mobile-device-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MobileDevicePageComponent {}

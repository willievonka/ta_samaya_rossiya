import { ChangeDetectionStrategy, Component } from '@angular/core';
import { MapModalBaseComponent } from '../../../../components/map-modal/map-modal.base.component';
import { DeclOfNumPipe } from '../../../../pipes/decl-of-num.pipe';

@Component({
    selector: 'analytics-map-modal',
    standalone: true,
    templateUrl: './analytics-map-modal.component.html',
    styleUrl: './styles/analytics-map-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [DeclOfNumPipe]
})
export class AnalyticsMapModalComponent extends MapModalBaseComponent {}

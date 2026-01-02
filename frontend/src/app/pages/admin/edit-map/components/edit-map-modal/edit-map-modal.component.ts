import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
    selector: 'edit-map-modal',
    standalone: true,
    templateUrl: './edit-map-modal.component.html',
    styleUrl: './styles/edit-map-modal.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class EditMapModalComponent {

}

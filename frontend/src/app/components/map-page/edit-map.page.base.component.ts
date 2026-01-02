import { Directive, signal, WritableSignal } from '@angular/core';
import { MapPageBaseComponent } from './map.page.base.component';
import { IPageHeaderOptions } from '../page-header/interfaces/page-header-options.interface';
import { IMapConfig } from '../map/interfaces/map-config.interface';
import { editMapConfig } from '../map/configs/edit-map.config';
import { ICanComponentDeactivate } from '../../guards/can-deactivate.guard';

@Directive()
export class EditMapPageBaseComponent<TData>
    extends MapPageBaseComponent<TData>
    implements ICanComponentDeactivate
{
    protected readonly hasUnsavedChanges: WritableSignal<boolean> = signal(false);

    protected override readonly headerOptions: IPageHeaderOptions = {
        isDetached: true,
        adminState: {
            changeRedirect: true,
            showLogoutIcon: false
        }
    };
    protected readonly config: IMapConfig = editMapConfig;

    /** Защита от потери изменений при выходе со страницы */
    public canDeactivate(): boolean {
        return !this.hasUnsavedChanges() || confirm('Вы уверены, что хотите покинуть страницу?\nИзменения могут не сохраниться');
    }
}

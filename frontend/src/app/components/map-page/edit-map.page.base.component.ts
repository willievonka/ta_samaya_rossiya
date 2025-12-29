import { Directive } from '@angular/core';
import { MapPageBaseComponent } from './map.page.base.component';
import { IPageHeaderOptions } from '../page-header/interfaces/page-header-options.interface';
import { IMapConfig } from '../map/interfaces/map-config.interface';
import { editMapConfig } from '../map/configs/edit-map.config';

@Directive()
export class EditMapPageBaseComponent<TData> extends MapPageBaseComponent<TData> {
    protected override readonly headerOptions: IPageHeaderOptions = {
        isDetached: true,
        adminState: {
            changeRedirect: true,
            showLogoutIcon: false
        }
    };
    protected readonly config: IMapConfig = editMapConfig;
}

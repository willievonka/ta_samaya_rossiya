import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { EditMapPageBaseComponent } from '../../../components/map-page/edit-map.page.base.component';
import { IMapPoint } from '../../../components/map/interfaces/map-point.interface';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { MapComponent } from '../../../components/map/map.component';
import { MapZoomComponent } from '../../../components/map-zoom/map-zoom.component';
import { EditMapModalComponent } from './components/edit-map-modal/edit-map-modal.component';
import { IMapModel } from '../../../components/map/models/map.model';
import { IMapLayer } from '../../../components/map/interfaces/map-layer.interface';
import { ɵFormGroupRawValue } from '@angular/forms';
import { IMapSettingsForm } from '../../../components/edit-map-modal/interfaces/map-settings-form.interface';
import { finalize, take, tap } from 'rxjs';
import { Router } from '@angular/router';

@Component({
    selector: 'edit-map',
    standalone: true,
    templateUrl: './edit-map.page.html',
    styleUrl: './styles/edit-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent,
        EditMapModalComponent
    ]
})
export class EditMapPageComponent extends EditMapPageBaseComponent<IMapPoint> {
    protected readonly isEditMode: boolean = !!this.mapId;
    private readonly _router: Router = inject(Router);

    /** Обработчик сохранения карты */
    protected handleMapSave(savedData: ɵFormGroupRawValue<IMapSettingsForm>): void {
        this.isSaving.set(true);

        this.model.update((current) => ({
            pageTitle: savedData.title,
            infoText: savedData.mapInfo,
            layers: current?.layers ?? [],
            layerWithPointsColor: savedData.layerWithPointsColor,
            pointColor: savedData.pointsColor
        }));

        const model: IMapModel | undefined = this.model();
        if (model) {
            this.mapDataService.saveMap(this.mapId, {
                isAnalytics: false,
                title: model.pageTitle,
                infoText: model.infoText,
                backgroundImage: savedData.cardBackgroundImage,
                description: savedData.cardDescription,
                layers: model.layers.map(layer => layer.properties),
                pointColor: model.pointColor,
                activeLayerColor: model.layerWithPointsColor
            })
                .pipe(
                    take(1),
                    tap(() => this.hasUnsavedChanges.set(false)),
                    finalize(() => this.isSaving.set(false))
                )
                .subscribe((mapId) => {
                    if (!this.isEditMode) {
                        this._router.navigate([`admin/edit-map?id=${mapId}`]);
                    }
                });
        }
    }

    /** Обработчик удаления карты */
    protected handleMapDelete(): void {
        this.mapDataService.deleteMap(this.mapId)
            .pipe(
                take(1),
                tap(() => this._router.navigate(['admin']))
            )
            .subscribe();
    }

    /**
     * Обработчик изменения точек
     * @param points
     */
    protected handlePointsChange(points: IMapPoint[]): void {
        const current: IMapModel | undefined = this.model();
        if (!current) {
            return;
        }

        const updatedLayers: IMapLayer[] = current.layers.map(layer => {
            const pointsForLayer: IMapPoint[] = points.filter(p => p.regionName === layer.properties.regionName);

            return {
                ...layer,
                properties: {
                    ...layer.properties,
                    points: pointsForLayer
                }
            };
        });

        this.model.set({
            ...current,
            layers: updatedLayers
        });
    }

    /**
     * Обработчик изменения цветов для карты
     * @param colors
     */
    protected handleColorsChange(colors: { layerWithPointsColor: string, pointColor: string }): void {
        const current: IMapModel | undefined = this.model();
        if (!current) {
            return;
        }

        this.model.set({
            ...current,
            layerWithPointsColor: colors.layerWithPointsColor,
            pointColor: colors.pointColor
        });
    }
}

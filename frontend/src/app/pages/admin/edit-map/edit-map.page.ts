import { ChangeDetectionStrategy, Component, inject, signal, WritableSignal } from '@angular/core';
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
import { catchError, finalize, forkJoin, Observable, of, take, tap } from 'rxjs';
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
    protected readonly isEditMode: WritableSignal<boolean> = signal<boolean>(!!this.mapId);
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
        if (!model) {
            this.isSaving.set(false);
            alert('Данные не сохранены, попробуйте еще раз');

            return;
        }

        const layers: IMapLayer[] = this.cloneLayersWithPoints(model.layers);
        const pointsToDownload: IMapPoint[] = this.collectPointsWithImagePath(layers);

        const preload$: Observable<unknown> = pointsToDownload.length
            ? forkJoin(pointsToDownload.map(p => this.downloadPointImage(p)))
            : of(null);

        preload$
            .pipe(take(1))
            .subscribe(() => this.saveMapWithImages(layers, model, savedData));
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

    /**
     * Сделать копию слоев
     * @param layers
     */
    private cloneLayersWithPoints(layers: IMapLayer[]): IMapLayer[] {
        return layers.map(layer => ({
            ...layer,
            properties: {
                ...layer.properties,
                points: (layer.properties.points ?? []).map(p => ({ ...p }))
            }
        }));
    }

    /**
     * Собрать точки с imagePath
     * @param layers
     */
    private collectPointsWithImagePath(layers: IMapLayer[]): IMapPoint[] {
        return layers.flatMap(layer =>
            layer.properties.points?.filter(p => !p.image && p.imagePath?.trim()) ?? []
        );
    }

    /**
     * Загрузить изображение точки
     * @param point
     * @returns
     */
    private downloadPointImage(point: IMapPoint): Observable<File | null> {
        return this.fileService.downloadAsFile(
            point.imagePath!.trim(),
            this.fileService.getFileNameFromUrl(point.imagePath!) ?? point.title
        ).pipe(
            tap(file => point.image = file),
            catchError(() => of(null))
        );
    }

    /**
     * Сохранить карту с изображениями точек
     * @param layers
     * @param model
     * @param savedData
     */
    private saveMapWithImages(layers: IMapLayer[], model: IMapModel, savedData: ɵFormGroupRawValue<IMapSettingsForm>): void {
        this.mapDataService.saveMap(this.mapId, {
            isAnalytics: false,
            title: model.pageTitle,
            infoText: model.infoText,
            backgroundImage: savedData.cardBackgroundImage,
            description: savedData.cardDescription,
            layers: layers.map(layer => layer.properties),
            pointColor: model.pointColor,
            activeLayerColor: model.layerWithPointsColor
        })
            .pipe(
                take(1),
                tap(() => this.hasUnsavedChanges.set(false)),
                finalize(() => this.isSaving.set(false))
            )
            .subscribe((mapId) => {
                if (!this.isEditMode()) {
                    this._router.navigate(
                        ['admin/edit-map'],
                        { queryParams: { id: mapId } }
                    );
                    this.isEditMode.set(false);
                }
            });
    }
}

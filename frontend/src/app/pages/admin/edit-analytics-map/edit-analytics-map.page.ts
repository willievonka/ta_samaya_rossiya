import { ChangeDetectionStrategy, Component } from '@angular/core';
import { IAnalyticsMapLayerProperties, IMapLayer, IMapLayerProperties } from '../../../components/map/interfaces/map-layer.interface';
import { PageHeaderComponent } from '../../../components/page-header/page-header.component';
import { MapZoomComponent } from '../../../components/map-zoom/map-zoom.component';
import { MapComponent } from '../../../components/map/map.component';
import { EditAnalyticsMapModalComponent } from './components/edit-analytics-map-modal/edit-analytics-map-modal.component';
import { EditMapPageBaseComponent } from '../../../components/map-page/edit-map.page.base.component';
import { IMapModel } from '../../../components/map/models/map.model';
import { ɵFormGroupRawValue } from '@angular/forms';
import { IAnalyticsMapSettingsForm } from '../../../components/edit-map-modal/interfaces/analytics-map-settings-form.interface';
import { catchError, finalize, forkJoin, Observable, of, take, tap } from 'rxjs';

@Component({
    selector: 'edit-analytics-map-page',
    standalone: true,
    templateUrl: './edit-analytics-map.page.html',
    styleUrl: './styles/edit-analytics-map-page.master.scss',
    changeDetection: ChangeDetectionStrategy.OnPush,
    imports: [
        PageHeaderComponent,
        MapComponent,
        MapZoomComponent,
        EditAnalyticsMapModalComponent
    ]
})
export class EditAnalyticsMapPageComponent extends EditMapPageBaseComponent<IMapLayerProperties> {
    /** Обработчик сохранения карты */
    protected handleMapSave(savedData: ɵFormGroupRawValue<IAnalyticsMapSettingsForm>): void {
        this.isSaving.set(true);
        this.model.update((current) => ({
            pageTitle: savedData.title,
            infoText: savedData.mapInfo,
            layers: current?.layers ?? []
        }));

        const model: IMapModel | undefined = this.model();
        if (!model) {
            this.isSaving.set(false);
            alert('Данные не сохранены, попробуйте еще раз');

            return;
        }

        const layers: IMapLayer[] = this.cloneLayersWithAnalytics(model.layers);
        const layersToDownload: IMapLayer[] = this.collectLayersWithAnalyticsImage(layers);

        const preload$: Observable<unknown> = layersToDownload.length
            ? forkJoin(layersToDownload.map(l => this.downloadAnalyticsImage(l)))
            : of(null);

        preload$
            .pipe(take(1))
            .subscribe(() => this.saveAnalyticsMap(layers, model, savedData));
    }

    /**
     * Обработчик изменения регионов
     * @param regions
     */
    protected handleRegionsChange(regions: IMapLayerProperties[]): void {
        const current: IMapModel | undefined = this.model();
        if (!current) {
            return;
        }

        const updatedLayers: IMapLayer[] = current.layers.map(layer => {
            const updatedProps: IMapLayerProperties | undefined = regions.find(r => r.regionName === layer.properties.regionName);

            if (updatedProps) {
                return { ...layer, properties: updatedProps };
            } else {
                return {
                    geoData: layer.geoData,
                    properties: {
                        id: layer.properties.id,
                        regionName: layer.properties.regionName
                    }
                };
            }
        });

        this.model.set({
            ...current,
            layers: updatedLayers
        });
    }

    /**
     * Создать копию слоев
     * @param layers
     * @returns
     */
    private cloneLayersWithAnalytics(layers: IMapLayer[]): IMapLayer[] {
        return layers.map(layer => ({
            ...layer,
            properties: {
                ...layer.properties,
                analyticsData: layer.properties.analyticsData
                    ? { ...layer.properties.analyticsData }
                    : undefined
            }
        }));
    }

    /**
     * Собрать слои с imagePath
     * @param layers
     * @returns
     */
    private collectLayersWithAnalyticsImage(layers: IMapLayer[]): IMapLayer[] {
        return layers.filter(layer => {
            const analytics: IAnalyticsMapLayerProperties | undefined = layer.properties.analyticsData;

            return !!analytics && !analytics.image && analytics.imagePath?.trim();
        });
    }

    /**
     * Загрузить изображение аналитического слоя
     * @param layer
     */
    private downloadAnalyticsImage(layer: IMapLayer): Observable<File | null> {
        const analytics: IAnalyticsMapLayerProperties = layer.properties.analyticsData!;
        const url: string = analytics.imagePath.trim();

        return this.fileService.downloadAsFile(
            url,
            this.fileService.getFileNameFromUrl(url) ?? analytics.imagePath
        ).pipe(
            tap(file => analytics.image = file),
            catchError(() => of(null))
        );
    }

    /**
     * Сохранить аналитическую карту с изображениями
     * @param layers
     * @param model
     * @param savedData
     */
    private saveAnalyticsMap(
        layers: IMapLayer[],
        model: IMapModel,
        savedData: ɵFormGroupRawValue<IAnalyticsMapSettingsForm>
    ): void {
        this.mapDataService.saveMap(this.mapId, {
            isAnalytics: true,
            title: model.pageTitle,
            infoText: model.infoText,
            backgroundImage: savedData.cardBackgroundImage,
            description: savedData.cardDescription,
            layers: layers.map(layer => layer.properties)
        })
            .pipe(
                take(1),
                tap(() => this.hasUnsavedChanges.set(false)),
                finalize(() => this.isSaving.set(false))
            )
            .subscribe();
    }
}

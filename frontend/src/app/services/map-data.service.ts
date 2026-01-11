import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, EMPTY, map, Observable, switchMap } from 'rxjs';
import { IMapLayerProperties } from '../components/map/interfaces/map-layer.interface';
import { environment } from '../../environments';
import { IMapDto } from '../components/map/dto/map.dto';
import { IMapModel } from '../components/map/models/map.model';
import { Router } from '@angular/router';
import { ISaveMapDto } from '../components/map/dto/save-map.dto';

@Injectable({ providedIn: 'root' })
export class MapDataService {
    private readonly _http: HttpClient = inject(HttpClient);
    private readonly _clientApiUrl: string = environment.clientApiUrl;
    private readonly _adminApiUrl: string = environment.adminApiUrl;
    private readonly _router: Router = inject(Router);

    /**
     * Получить данные для карты по mapId
     * @param mapId
     */
    public getMapData(mapId: string, raw: boolean = false): Observable<IMapModel> {
        const baseUrl: string = raw ? this._adminApiUrl : this._clientApiUrl;
        const options: { params?: HttpParams } | undefined = raw
            ? undefined
            : { params: new HttpParams({ fromObject: { mapId } }) };

        return this._http.get<IMapDto>(`${baseUrl}/maps`, options)
            .pipe(
                map((dto: IMapDto) => this.mapDtoToModel(dto)),
                catchError(() => {
                    this._router.navigate(['/']);

                    return EMPTY;
                })
            );
    }

    /**
     * Сохранить карту
     * @param mapId
     * @param data
     */
    public saveMap(mapId: string, dto: ISaveMapDto): Observable<string> {
        const formData = (isCreate: boolean): FormData => this.mapDtoToFormData(dto, isCreate);
        const handleError = (error: HttpErrorResponse): Observable<never> => {
            if (error.status !== 401) {
                alert('Данные не сохранены, попробуйте еще раз');
            }

            return EMPTY;
        };

        if (!mapId) {
            return this._http.post<{ mapId: string }>(`${this._adminApiUrl}/maps`, formData(true))
                .pipe(
                    catchError(handleError),
                    switchMap((res) => {
                        return this._http.put<string>(
                            `${this._adminApiUrl}/maps`,
                            formData(false),
                            { params: new HttpParams({ fromObject: { mapId: res.mapId } }) }
                        ).pipe(
                            map(() => res.mapId),
                            catchError(handleError)
                        );
                    })
                );
        }

        return this._http.put<string>(
            `${this._adminApiUrl}/maps`,
            formData(false),
            { params: new HttpParams({ fromObject: { mapId } }) }
        ).pipe(catchError(handleError));
    }

    /**
     * Удалить карту
     * @param mapId
     */
    public deleteMap(mapId: string): Observable<void> {
        return this._http.delete<void>(
            `${this._adminApiUrl}/maps`,
            { params: new HttpParams({ fromObject: { mapId } }) }
        ).pipe(
            catchError((error) => {
                if (error.status !== 401) {
                    alert('Не удалось удалить проект, попробуйте еще раз');
                }

                return EMPTY;
            })
        );
    }

    /**
     * Смаппить dto в модель
     * @param dto
     */
    private mapDtoToModel(dto: IMapDto): IMapModel {
        return {
            pageTitle: dto.pageTitle,
            infoText: dto.infoText,
            layers: dto.layers.features.map((feature) => ({
                geoData: feature.geometry,
                properties: feature.properties as IMapLayerProperties
            })),
            layerWithPointsColor: dto.activeLayerColor,
            pointColor: dto.pointColor
        };
    }

    /**
     * Смаппить dto в FormData
     * @param dto
     */
    private mapDtoToFormData(dto: ISaveMapDto, isCreate?: boolean): FormData {
        const formData: FormData = new FormData();

        formData.append('isAnalytics', String(dto.isAnalytics));
        formData.append('title', dto.title);
        formData.append('description', dto.description);
        formData.append('infoText', dto.infoText);

        if (dto.activeLayerColor) {
            formData.append('activeLayerColor', dto.activeLayerColor);
        }

        if (dto.pointColor) {
            formData.append('pointColor', dto.pointColor);
        }

        if (dto.backgroundImage) {
            formData.append('backgroundImage', dto.backgroundImage);
        }

        if (!isCreate) {
            dto.layers.forEach((layer, layerIndex) => {
                const layerPrefix: string = `layers[${layerIndex}]`;

                formData.append(`${layerPrefix}.id`, layer.id);
                formData.append(`${layerPrefix}.regionName`, layer.regionName);
                formData.append(`${layerPrefix}.isActive`, String(layer.isActive ?? false));

                if (layer.style) {
                    Object.entries(layer.style).forEach(([key, value]: [string, unknown]) => {
                        if (value !== undefined && value !== null) {
                            formData.append(
                                `${layerPrefix}.style.${key}`,
                                String(value)
                            );
                        }
                    });
                }

                if (layer.analyticsData) {
                    const analyticsPrefix: string = `${layerPrefix}.analyticsData`;

                    formData.append(
                        `${analyticsPrefix}.partnersCount`,
                        String(layer.analyticsData.partnersCount)
                    );
                    formData.append(
                        `${analyticsPrefix}.excursionsCount`,
                        String(layer.analyticsData.excursionsCount)
                    );
                    formData.append(
                        `${analyticsPrefix}.membersCount`,
                        String(layer.analyticsData.membersCount)
                    );
                    formData.append(
                        `${analyticsPrefix}.isActive`,
                        'true'
                    );

                    if (layer.analyticsData.image) {
                        formData.append(
                            `${analyticsPrefix}.image`,
                            layer.analyticsData.image
                        );
                    }
                }

                layer.points?.forEach((point, pointIndex) => {
                    const pointPrefix: string = `${layerPrefix}.points[${pointIndex}]`;

                    formData.append(`${pointPrefix}.title`, point.title);
                    formData.append(`${pointPrefix}.year`, String(point.year));
                    formData.append(
                        `${pointPrefix}.coordinates[0]`,
                        String(point.coordinates[0])
                    );
                    formData.append(
                        `${pointPrefix}.coordinates[1]`,
                        String(point.coordinates[1])
                    );
                    formData.append(`${pointPrefix}.description`, point.description);

                    if (point.id) {
                        formData.append(`${pointPrefix}.id`, point.id);
                    }
                    if (point.excursionUrl) {
                        formData.append(`${pointPrefix}.excursionUrl`, point.excursionUrl);
                    }
                    if (point.image) {
                        formData.append(
                            `${pointPrefix}.image`,
                            point.image
                        );
                    }
                });
            });
        }

        return formData;
    }
}

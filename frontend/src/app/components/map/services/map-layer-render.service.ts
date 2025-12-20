import { Injectable, signal, WritableSignal } from '@angular/core';
import { geoJSON, Layer, LeafletMouseEvent, DomEvent, PathOptions, Map as LeafletMap } from 'leaflet';
import { IMapLayer, IMapLayerProperties } from '../interfaces/map-layer.interface';
import { IActiveLeafletLayer } from '../interfaces/leaflet-layer.interface';
import { customCoordsToLatLng } from '../utils/custom-coords-to-lat-lng.util';
import { makeBrighterColor } from '../utils/make-brighter-color.util';
import { mapConfig } from '../map.config';
import { Feature, GeoJsonProperties } from 'geojson';

@Injectable()
export class MapLayerRenderService {
    private readonly _activeLayer: WritableSignal<IActiveLeafletLayer | null> = signal<IActiveLeafletLayer | null>(null);
    private readonly _defaultLayerStyle: PathOptions = mapConfig.defaultLayerStyle;

    /**
     * Отрисовать слои
     * @param mapInstance
     * @param layers
     * @param layerWithPointsColor
     * @param onLayerSelected
     */
    public renderLayers(
        mapInstance: LeafletMap,
        layers: IMapLayer[],
        layerWithPointsColor: string | undefined,
        onLayerSelected: (props: IMapLayerProperties) => void
    ): void {
        layers.forEach((layer) => {
            const properties: GeoJsonProperties = this.createLayerProperties(
                layer,
                layerWithPointsColor,
                onLayerSelected
            );
            geoJSON(layer.geoData, properties).addTo(mapInstance);
        });
    }

    /** Снять выделение активного слоя */
    public resetActiveSelection(): void {
        const currentLayer: IActiveLeafletLayer | null = this._activeLayer();
        if (currentLayer) {
            this.resetLayerStyle(currentLayer);
            this._activeLayer.set(null);
        }
    }

    /**
     * Создать свойства слоя
     * @param mapLayer
     * @param layerWithPointsColor
     * @param onLayerSelected
     */
    private createLayerProperties(
        mapLayer: IMapLayer,
        layerWithPointsColor: string | undefined,
        onLayerSelected: (props: IMapLayerProperties) => void
    ): GeoJsonProperties {
        const { properties }: IMapLayer = mapLayer;
        const isActive: boolean = !!properties.isActive;
        const hasPoints: boolean = !!properties.points?.length;

        const layerStyle: PathOptions = this.buildLayerStyle(
            properties.style,
            hasPoints,
            layerWithPointsColor
        );

        return {
            ...properties,
            style: layerStyle,
            interactive: isActive,
            coordsToLatLng: customCoordsToLatLng,
            onEachFeature: (feature: Feature, leafletLayer: Layer): void => {
                if (!isActive) {
                    return;
                }

                const pathLayer: IActiveLeafletLayer = leafletLayer as IActiveLeafletLayer;
                pathLayer.originalStyle = { ...pathLayer.options };

                leafletLayer.on({
                    click: (event: LeafletMouseEvent) => {
                        DomEvent.stopPropagation(event);
                        onLayerSelected(mapLayer.properties);
                        this.applyActiveStyle(pathLayer);
                        pathLayer.bringToFront();
                    }
                });
            }
        };
    }

    /**
     * Собрать стиль слоя
     * @param originalStyle
     * @param hasPoints
     * @param layerWithPointsColor
     */
    private buildLayerStyle(
        originalStyle: PathOptions | undefined,
        hasPoints: boolean,
        layerWithPointsColor?: string
    ): PathOptions {
        const newColor: string | undefined = hasPoints ? layerWithPointsColor : undefined;

        return {
            ...this._defaultLayerStyle,
            ...originalStyle,
            ...(newColor && { fillColor: newColor })
        };
    }

    /**
     * Применить стиль активного слоя
     * @param leafletLayer
     */
    private applyActiveStyle(leafletLayer: IActiveLeafletLayer): void {
        const currentActive: IActiveLeafletLayer | null = this._activeLayer();
        const originalStyle: PathOptions = leafletLayer.originalStyle!;

        if (currentActive && currentActive !== leafletLayer) {
            this.resetLayerStyle(currentActive);
        }
        if (!leafletLayer.brighterColor) {
            leafletLayer.brighterColor = makeBrighterColor(originalStyle.fillColor!, 15);
        }

        leafletLayer.setStyle({
            ...originalStyle,
            weight: (this._defaultLayerStyle.weight ?? 1) * 3,
            fillColor: leafletLayer.brighterColor
        });

        const element: SVGElement | undefined = leafletLayer.getElement() as SVGElement | undefined;
        if (element) {
            element.style.filter = 'drop-shadow(0px 4px 4px rgba(0, 0, 0, 0.33))';
        }

        this._activeLayer.set(leafletLayer);
    }

    /**
     * Сбросить стиль слоя к оригинальному
     * @param leafletLayer
     */
    private resetLayerStyle(leafletLayer: IActiveLeafletLayer): void {
        const originalStyle: PathOptions = leafletLayer.originalStyle ?? this._defaultLayerStyle;
        leafletLayer.setStyle(originalStyle);

        const element: SVGElement | undefined = leafletLayer.getElement() as SVGElement | undefined;
        if (element) {
            element.style.filter = '';
        }
    }
}

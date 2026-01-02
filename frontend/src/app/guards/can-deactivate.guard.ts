import { CanDeactivateFn } from '@angular/router';

export interface ICanComponentDeactivate {
    canDeactivate: () => boolean;
}
export const canDeactivateGuard: CanDeactivateFn<ICanComponentDeactivate> =
    (component: ICanComponentDeactivate): boolean => component.canDeactivate();

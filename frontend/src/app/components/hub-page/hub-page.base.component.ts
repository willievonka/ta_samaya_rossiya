import { Directive, inject, signal, WritableSignal } from '@angular/core';
import { IHubCard } from '../hub-card/interfaces/hub-card.interface';
import { HubService } from '../../services/hub.service';
import { take } from 'rxjs';
import { PageBaseComponent } from '../page/page.base.component';

@Directive({
    standalone: true
})
export class HubPageBaseComponent extends PageBaseComponent {
    protected readonly mapCardsList: WritableSignal<IHubCard[] | undefined> = signal(undefined);
    protected readonly hubService: HubService = inject(HubService);

    constructor() {
        super();
        this.loadMapCardList();
    }

    /** Загрузить список карточек карт */
    private loadMapCardList(): void {
        this.hubService.getMapCardsList()
            .pipe(take(1))
            .subscribe(list => {
                this.mapCardsList.set(list);
                setTimeout(() => this.isLoading.set(false), 500);
            });
    }
}

import { Component } from '@angular/core';

@Component({
    selector: 'change-hosts',
    templateUrl: './change-hosts.component.html'
})
export class ChangeHostsPageComponent {
    public currentCount = 0;

    public incrementCounter() {
        this.currentCount++;
    }
}

import { Component } from '@angular/core';
import {StardewService} from '../../services/stardewService';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    providers: [StardewService]
})
export class HomeComponent {

}
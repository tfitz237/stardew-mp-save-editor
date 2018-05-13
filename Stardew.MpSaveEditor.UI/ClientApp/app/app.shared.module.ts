import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { AddPlayersPageComponent } from './components/add-players/add-players.component';
import { ChangeHostsPageComponent } from './components/change-hosts/change-hosts.component';
import { RemoveCabinPageComponent } from './components/remove-cabin/remove-cabin.component';
import { StardewService } from './services/stardewService';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        AddPlayersPageComponent,
        ChangeHostsPageComponent,
        RemoveCabinPageComponent,
        HomeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'add-players', component: AddPlayersPageComponent },
            { path: 'change-hosts', component: ChangeHostsPageComponent },
            { path: 'remove-cabin', component: RemoveCabinPageComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ],
    providers: [StardewService]
})
export class AppModuleShared {
}

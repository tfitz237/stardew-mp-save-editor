import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { StardewService, SaveFiles } from '../../services/stardewService';

@Component({
    selector: 'add-players',
    templateUrl: './add-players.component.html'
})
export class AddPlayersPageComponent {
    public saveFiles: SaveFiles[] = [];
    public numberOfPlayers: number = 0;
    public saveFileSelection: string = "";
    public message: string = "";
    constructor(private stardewService: StardewService) {        
        this.stardewService.RetrieveSaveFiles().subscribe(saves => {
            this.saveFiles = saves;
        })
    }

    public addPlayerSlots() {
        if (this.saveFileSelection && this.numberOfPlayers > 0) {
            this.stardewService.AddPlayers(this.saveFileSelection, this.numberOfPlayers).subscribe(result => {
                this.message = result.message;
            });
        }
    }
    
}



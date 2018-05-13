import { Injectable, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
@Injectable()
export class StardewService {
    
    constructor(
        private http: Http,                 
        @Inject('BASE_URL') 
        private baseUrl: string
    ) {}

    public RetrieveSaveFiles(): Observable<SaveFiles[]> {
        return this.http.get(this.baseUrl + 'api/stardew/getSaveFiles').map(res => res.json());;
    }

    public AddPlayers(saveFile: string, numPlayers: number): Observable<Result> {
        return this.http.post(this.baseUrl + 'api/stardew/addPlayers', {
            saveFile: saveFile,
            numberOfPlayers: numPlayers
        }).map ( res => 
            res.json());
    }

}


export interface SaveFiles {
    saveFileName: string;
    saveFilePath: string;
}

export interface Result {
    result: boolean;
    message: string;
}
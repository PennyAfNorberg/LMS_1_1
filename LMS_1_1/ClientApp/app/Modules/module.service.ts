import { Injectable, OnDestroy } from '@angular/core';
import { Subject, throwError, Observable } from 'rxjs';
import { HttpHeaders, HttpClient, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { takeUntil, tap, catchError } from 'rxjs/operators';
import { IModule, Module } from '../Courses/course';
import { Guid } from 'guid-typescript';
import { IDubbParas } from './Check-if-dubbs/DubbParas';

@Injectable({
    providedIn: 'root'
})
export class ModuleService implements OnDestroy {

    private moduleUrl = "https://localhost:44396/api/module1";
    private token: string = "";
    private unsubscribe: Subject<void> = new Subject();

    private getAuthHeader(): HttpHeaders {
        this.AuthService.isAuthenticated;
        return new HttpHeaders({ "Authorization": "Bearer " + this.token });
    }

    constructor(private http: HttpClient, private AuthService: AuthService) {
        this.AuthService.token
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(i => this.token = i);
    }

    CheckIfDubblett(paras :IDubbParas):Observable<boolean>
    {
     
      return this.http.post<boolean>(this.moduleUrl+"/TestIfInRange",paras
      ,{
        headers: this.getAuthHeader()
    }).pipe(
      tap(result => JSON.stringify(result)),
      catchError(this.handleError)
  );

    }


    CreateModule(Module: IModule): any {
        return this.http.post(this.moduleUrl+"/PostModule", Module,
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(result => JSON.stringify(result)),
                catchError(this.handleError)
            );
    }

    EditModule(id: Guid, Module: IModule): any {
        return this.http.put(this.moduleUrl+ "/Edit/" + id, Module,
        {
            headers: this.getAuthHeader()
        }).pipe(
            tap(result => JSON.stringify(result)),
            catchError(this.handleError)
        );
      }

    
      GetModule(Moduleid: string):  Observable<IModule>  {
        return this.http.get<IModule>(this.moduleUrl+"/"+Moduleid,
        {headers: this.getAuthHeader() 
  }).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
            );
      }


    private handleError(err: HttpErrorResponse) {
        // in a real world app, we may send the server to some remote logging infrastructure
        // instead of just logging it to the console
        let errorMessage = '';
        if (err.error instanceof ErrorEvent) {
            // A client-side or network error occurred. Handle it accordingly.
            errorMessage = `An error occurred: ${err.error.message}`;
        } else {
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong,
            errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
        }
        console.error(errorMessage);
        return throwError(errorMessage);
    }

    //Delete a module by a given guid.
    DeleteModule(id: Guid) {
        let urlString = this.moduleUrl + "/" + id;
        return this.http.delete(urlString,
            {
                headers: this.getAuthHeader()
            })
            .pipe(
                tap(result => JSON.stringify(result)), catchError(this.handleError)
            );
    }

    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }


}

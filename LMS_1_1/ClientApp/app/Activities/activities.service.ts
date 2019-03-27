import { Injectable, OnDestroy } from '@angular/core';
import { Subject, throwError, Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { takeUntil, tap, catchError } from 'rxjs/operators';
import { IActivity, IActivityType, Activity } from '../Courses/course';
import { Guid } from 'guid-typescript';

@Injectable({
    providedIn: 'root'
})
export class ActivitiesService implements OnDestroy {


    private activityUrl = "https://localhost:44396/api/activity1";
    private token: string = "";
    private unsubscribe: Subject<void> = new Subject();

    constructor(private http: HttpClient, private AuthService: AuthService) {
        this.AuthService.token
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(i => this.token = i);
    }



    private getAuthHeader(): HttpHeaders {
        return new HttpHeaders({ "Authorization": "Bearer " + this.token });
    }


    CreateActivity(Activity: IActivity): any {
        return this.http.post(this.activityUrl + "/PostActivity", Activity,
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(result => JSON.stringify(result)),
                catchError(this.handleError)
            );
    }

    getActitityTypes(): Observable<IActivityType[]> {
        return this.http.get<IActivityType[]>(this.activityUrl + "/ActivityTypes",
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(data => console.log('All:' + JSON.stringify(data))),
                catchError(this.handleError)
            );
    }


    EditActivity(id: Guid, Activity: IActivity): any {
        return this.http.put(this.activityUrl + "/Edit/" + id, Activity,
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(result => JSON.stringify(result)),
                catchError(this.handleError)
            );
    }

    MoveActivity(id: Guid, Activity: IActivity): any {
        return this.http.put(this.activityUrl + "/Move/" + id, Activity,
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(result => JSON.stringify(result)),
                catchError(this.handleError)
            );
      }

    GetActivity(Activityid: string): Observable<Activity> {
        return this.http.get<Activity>(this.activityUrl + "/" + Activityid,
            {
                headers: this.getAuthHeader()
            }).pipe(
                tap(data => console.log('All:' + JSON.stringify(data))),
                catchError(this.handleError)
            );
    }

    //Delete a activity by a given guid.
    DeleteActivity(id: Guid) {
        let urlString = this.activityUrl + "/" + id;
        return this.http.delete(urlString,
            {
                headers: this.getAuthHeader()
            })
            .pipe(
                tap(result => JSON.stringify(result)), catchError(this.handleError)
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

    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }

}

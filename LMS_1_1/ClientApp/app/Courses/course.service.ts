import { Injectable, OnDestroy } from '@angular/core';

import { ICourse,course, IModule, IActivity2, IActivity, CloneType } from './course';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject, Subject } from 'rxjs';

import { tap, catchError, map, takeUntil } from 'rxjs/operators';
import { Guid } from 'guid-typescript';
import { Data } from '@angular/router';
import { AuthService } from '../auth/auth.service';


@Injectable(
    {
        providedIn: 'root'
    })

export class CourseService implements  OnDestroy {


    private courseUrl = "https://localhost:44396/api/courses1";
    private token: string="";
    private unsubscribe : Subject<void> = new Subject();


    
    private getAuthHeader() : HttpHeaders
    {
        this.AuthService.isAuthenticated;
      return  new HttpHeaders({ "Authorization": "Bearer " + this.token });
    }

    constructor(private http: HttpClient,  private AuthService:AuthService) {
        this.AuthService.token
        .pipe(takeUntil(this.unsubscribe))
        .subscribe( i => this.token=i);
    }
    getCourses(): Observable<ICourse[]> {
        return this.http.get<ICourse[]>(this.courseUrl+"/foruser",
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
            );
    }
    getCourseById(id: string): Observable<ICourse> {
        return this.http.get<ICourse>(this.courseUrl +"/"+id,
        {headers: this.getAuthHeader()  
}).pipe(

            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );
    }

    getCourseAllById(id: string): Observable<ICourse> {
        return this.http.get<ICourse>(this.courseUrl +"/All?id=" +id,
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );
    }

    getCourseAndModulebyId(courseid: string) : Observable<ICourse>
    {
        return this.http.get<ICourse>(this.courseUrl +"/CAndM?id=" +courseid,
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );

    }

    getActivitybymodulId(Moduleid: string) : Observable<IActivity2[]>
    {
        return this.http.get<IActivity2[]>(this.courseUrl +"/AfromMid?id=" +Moduleid,
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );

    }

    getModulAndActivitybyId(Moduleid: string) : Observable<IModule>
    {
        return this.http.get<IModule>(this.courseUrl +"/MAndAfromMid?id=" +Moduleid,
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );

    }

    GetCloneTypes():Observable<CloneType[]> {
        return this.http.get<CloneType[]>(this.courseUrl +"/CloneTypes",
        {headers: this.getAuthHeader() 
}).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );
      }

    createCourse(course: any) {

        return this.http.post(this.courseUrl+'/Create',  course,
            {headers: this.getAuthHeader() 
    }).pipe(
            tap(result => JSON.stringify(result)),
            catchError(this.handleError)
        );
    }

    cloneCourse(course: any){
        return this.http.post(this.courseUrl+'/Clone',  course,
        {headers: this.getAuthHeader() 
}).pipe(
        tap(result => JSON.stringify(result)),
        catchError(this.handleError)
    );
      }
    

    //Delete a course by a given guid.
    DeleteCourse(id: Guid) {
        let urlString = this.courseUrl +"/"+ id;
        return this.http.delete(urlString,
            {headers: this.getAuthHeader() 
        })
        .pipe(
            tap(result=>JSON.stringify(result)),catchError(this.handleError)
        );
    }

    //Edit a course with a given guid.
    EditCourse(id: Guid, editCourse: any) {
        //console.log("COURSE FORM EDIT COURSE" + editCourse);
        let urlString = this.courseUrl + "/" + id;
        return this.http.put(urlString, editCourse,
            {
                headers: new HttpHeaders({ "Authorization": "Bearer " + this.token })
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
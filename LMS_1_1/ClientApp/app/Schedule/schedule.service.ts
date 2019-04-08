import { Injectable, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject, throwError, Observable } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';
import { takeUntil, tap, catchError } from 'rxjs/operators';
import { ScheduleFormModel,  CourseSettingsViewModel, Scheduleentity } from './Scheduleentites';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService  implements  OnDestroy {

  private courseUrl = "https://localhost:44396/api/courses1";
  private token: string="";
  private unsubscribe : Subject<void> = new Subject();
  private coursesettingsUrl = "https://localhost:44396/api/CourseSettings";

  constructor(private http: HttpClient,  private AuthService:AuthService
   )// ,private cd: ChangeDetectorRef) 
  {
    this.AuthService.token
    .pipe(takeUntil(this.unsubscribe))
    .subscribe( i =>
      {
        this.token=i;
        //this.cd.markForCheck();
      } 
        );
}



  private getAuthHeader() : HttpHeaders
  {
    if (this.AuthService.isAuthenticated)
    return  new HttpHeaders({ "Authorization": "Bearer " + this.token });

    return  new HttpHeaders({ "Authorization": "Bearer " + this.token });
  }

/*

        public string CourseId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
*/

   public GetModulesWithColour(scheduleFormModel:ScheduleFormModel) : Observable<Scheduleentity[][][]>| undefined
{
   return this.http.post<Scheduleentity[][][]>(this.courseUrl+'/ModulesWithColor',  scheduleFormModel,
   {headers: this.getAuthHeader() 
}).pipe(
   tap(result => JSON.stringify(result)),
   catchError(this.handleError)
);
}

public GetActivitiesWithColour(scheduleFormModel:ScheduleFormModel) : Observable<Scheduleentity[][][]>| undefined
{
   return this.http.post<Scheduleentity[][][]>(this.courseUrl+'/ActivitiesWithColor',  scheduleFormModel,
   {headers: this.getAuthHeader() 
}).pipe(
   tap(result => JSON.stringify(result)),
   catchError(this.handleError)
);
}

 public GetCourseSettings(courseId:string, startDate:Date, endDate: Date)
{
  return this.http.get<CourseSettingsViewModel[]>(this.coursesettingsUrl+'/Get?CourseId='+courseId+'&StartDate='+startDate.toLocaleString()+'&EndDate='+endDate.toLocaleString(),
  {headers: this.getAuthHeader() 
}).pipe(
  tap(result => JSON.stringify(result)),
  catchError(this.handleError)
);
}

//GetAsync(String CourseId, DateTime StartDate, DateTime EndDate)

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

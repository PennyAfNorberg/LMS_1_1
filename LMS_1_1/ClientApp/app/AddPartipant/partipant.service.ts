import { Injectable, OnDestroy } from '@angular/core';
import { IPartipant,ICourseNameData,ICourseNameSubdata } from './partipant';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';

import {Observable, throwError, Subject, BehaviorSubject} from 'rxjs';
import {catchError, tap, map, takeUntil} from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { ICourse } from '../Courses/course';
import { RegisterUser } from '../Login/Register/registeruser';


@Injectable({
    providedIn: 'root'
})

export class PartipantService implements  OnDestroy{


    public Choosed: IPartipant[] = [];

    public CourseId: string="";
    private token: string="";
    private unsubscribe : Subject<void> = new Subject();
    
    private ShowPartipantListSource = new BehaviorSubject<boolean>(false);
    ShowPartipantList = this.ShowPartipantListSource.asObservable();

    constructor(private http: HttpClient,  private AuthService:AuthService) 
    {
        this.AuthService.token
        .pipe(takeUntil(this.unsubscribe))
        .subscribe( i => {

         this.token=i;

        }
         );
    }
     
public SendPartipantList(arg:boolean)
{
    this.ShowPartipantListSource.next(arg);
}


    private getAuthHeader() : HttpHeaders
    {
        this.AuthService.isAuthenticated;
      return  new HttpHeaders({ "Authorization": "Bearer " + this.token });
    }
	

    public GetStudentsOff(): Observable<IPartipant[] | undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetusersOff";  
        let parmas={"CourseId":this.CourseId}; 
        return this.http.post<IPartipant[]>(url,parmas,
            {headers: this.getAuthHeader() 
    } )
        .pipe(
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        );    
    }

    public GetCourseName(): Observable<ICourseNameData| undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetCourseName";  
        let parmas={"CourseId":this.CourseId}; 
        return this.http.post<ICourseNameData>(url,parmas
            ,{headers: this.getAuthHeader() 
        })
        .pipe(
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        ); 

     }
    public GetStudentsOn(): Observable<IPartipant[] | undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetusersOn";  
        let parmas={"CourseId":this.CourseId};   
        return this.http.post<IPartipant[]>(url,parmas
            ,{headers: this.getAuthHeader() 
    })
        .pipe(
           /* map(
            (response:IPartipant[])=>
        {this.Choosed=response;
        }
        ),*/
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        );  
    }

    public GetUsers()
    {
        let url:string="https://localhost:44396/CourseUsers/GetUsers";  
 
        return this.http.get<RegisterUser[]>(url,
        {headers: this.getAuthHeader() 
    })
        .pipe(tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError));

    } 

    public GetUser(id:string)
    {
        let url:string="https://localhost:44396/CourseUsers/GetUser";  
        let parmas={"CourseId":id}; 
        return this.http.post<RegisterUser[]>(url,parmas,
        {headers: this.getAuthHeader() 
    })
        .pipe(tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError));

    } 

    public SaveStudents()
    {
        let url:string="https://localhost:44396/CourseUsers/AddStudentsToCourse";  
        let Userids: string[] =[];
        for(const part of this.Choosed )
        {
            Userids.push(part.userid);
        }
        //let parmas={"CourseId":this.CourseId,Userids};    
        return this.http.post(url,{"CourseId":this.CourseId,Userids},
        {headers: this.getAuthHeader() 
    })
        .pipe(tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError));

    } 
/*
    public DeleteUser(id: string) {
        let url:string="https://localhost:44396/CourseUsers/DeleteUser";  
        return this.http.post(url,{"UserId":id,},
        {headers: this.getAuthHeader() 
    })
        .pipe(tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError));

      }
    */

    public AddStudent(user: IPartipant ):void
    {
        let item: IPartipant = this.Choosed.find(i => i.userid == user.userid  );
        
        if (!item) 
        {
            this.Choosed.push(user);
        }
    
    }

    public RemoveStudent(user: IPartipant):void
    {
        let item: IPartipant = this.Choosed.find(i => i.userid == user.userid  );
        let index = this.Choosed.indexOf(item);
        if(item)
        {
            
            this.Choosed.splice(index,1);
        }
    }


    public GetCoursesOff(userid?: string): Observable<ICourse[] | undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetCoursesOff";  
        let parmas={"UserId":userid}; 
        return this.http.post<ICourse[]>(url,parmas,
            {headers: this.getAuthHeader() 
    } )
        .pipe(
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        );    
    }


    public GetCoursesOn(userid?: string): Observable<ICourse[] | undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetCoursesOn";  
        let parmas={"UserId":userid};   
        return this.http.post<ICourse[]>(url,parmas
            ,{headers: this.getAuthHeader() 
    })
    .pipe(
            /* map(
            (response:IPartipant[])=>
        {this.Choosed=response;
        }
        ),*/
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        );  
    }

    public GetUserName(userid: string): Observable<ICourseNameData| undefined>
    {
        let url:string="https://localhost:44396/CourseUsers/GetUserName";  
        let parmas={"UserId":userid}; 
        return this.http.post<ICourseNameData>(url,parmas
            ,{headers: this.getAuthHeader() 
        })
        .pipe( 
            tap(data => console.log('All: ' + JSON.stringify(data))),
        catchError(this.handleError)
        ); 

     }

     public SaveCourses(userid: string, Choosed: ICourse[] )
     {
         let url:string="https://localhost:44396/CourseUsers/AddCoursesToStudent";  
         let courseids: string[] =[];
         for(const part of Choosed )
         {
            courseids.push(part.id.toString());
         }
         //let parmas={"CourseId":this.CourseId,Userids};    
         return this.http.post(url,{"UserId":userid,courseids},
         {headers: this.getAuthHeader() 
     })
         .pipe(tap(data => console.log('All: ' + JSON.stringify(data))),
         catchError(this.handleError));
 
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
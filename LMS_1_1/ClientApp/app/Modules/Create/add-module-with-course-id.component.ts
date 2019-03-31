import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { IModule,Module } from '../../Courses/course';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';
import { CourseService } from 'ClientApp/app/Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { ModuleService } from '../module.service';
import { Guid } from 'guid-typescript';

 @Component({	
  selector: 'app-add-module-with-course-id',	
  templateUrl: './add-module-with-course-id.component.html',	
  styleUrls: ['./add-module-with-course-id.component.css']	
})	
export class AddModuleWithCourseIdComponent implements OnInit, OnDestroy  {	
  private unsubscribe : Subject<void> = new Subject();
// get course set up coursestatdate
   Module: IModule = new Module();	
   coursestartdate: Date;
  CourseId: string ="";	
  errorMessage:string="";
  CourseName:string =""
  constructor(private route: ActivatedRoute
    ,private db: AuthService
    , private cd: ChangeDetectorRef
    ,private messhandler: LoginMessageHandlerService
   // ,private CourseService: CourseService
    ,private ModuleService: ModuleService
  //  ,  private router: Router
    ) { }	

   ngOnInit() {	
    this.messhandler.Courseid
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status => {
       // let tmpguid= Guid.parse(status); 
        this.Module.courseid=status;
        this.cd.markForCheck();
      }
    )

    this.messhandler.CourseStartDate
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status => {
        this.coursestartdate=status;
        this.cd.markForCheck();
      }
    )
    this.messhandler.CourseName
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status => {
        this.CourseName=status;
        this.cd.markForCheck();
      }
    )
  }	

public gotDate():void
{
   if(this.Module.startDate != null && this.Module.endDate != null)
   {
      this.messhandler.SendDubbId(this.Module.courseid);
      this.messhandler.SendDubbType("Module");
      this.messhandler.SendDubbStart(new Date(this.Module.startDate+":00.000Z"));
      this.messhandler.SendDubbEnd(new Date(this.Module.endDate+":00.000Z"));
      this.messhandler.SendWeek(new Date(this.Module.startDate+":00.000Z"));
   }

  // post data
}

public Create(theForm):void
{
  this.errorMessage = "";
  if(this.Module.startDate.valueOf()<this.coursestartdate.valueOf())
  {
      this.errorMessage= this.errorMessage + "Start date on module may not be before course start date ("+this.coursestartdate+")";
  }
  if(this.Module.endDate.valueOf()<this.coursestartdate.valueOf())
  {
      this.errorMessage= this.errorMessage + "End date on module may not be before course start date ("+this.coursestartdate+")";
  } 
  if(this.Module.endDate.valueOf()<this.Module.startDate.valueOf())
  {
    this.errorMessage= this.errorMessage +" A module must end after it's start";
  } 
  if(this.errorMessage=="")
  {
    this.ModuleService.CreateModule(this.Module)
    .pipe(takeUntil(this.unsubscribe))
    .subscribe( status =>
      {
        if(status)
        {
          this.errorMessage="Module "+this.Module.name+" saved"

        }
        this.cd.markForCheck();
      }
      ,err =>  this.errorMessage = <any>err
      
      )
  }
}
  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

 }
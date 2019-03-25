import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { IModule,Module } from 'ClientApp/app/Courses/course';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';
import { ModuleService } from '../module.service';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit, OnDestroy {
  private unsubscribe : Subject<void> = new Subject();
// get course set up coursestatdate
   Module: IModule = new Module();	
   coursestartdate: Date;
  CourseId: string ="";	
  errorMessage:string="";
  CourseName:string =""
  Moduleid : string="";
  constructor(private route: ActivatedRoute
    ,private db: AuthService
    , private cd: ChangeDetectorRef
    ,private messhandler: LoginMessageHandlerService
   // ,private CourseService: CourseService
    ,private ModuleService: ModuleService) { }

  ngOnInit() {
    this.Moduleid = this.route.snapshot.paramMap.get("id"); // null if no hit?
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
      this.ModuleService.GetModule(this.Moduleid)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
        resp =>  
        { 
          this.Module=resp;
          this.cd.markForCheck();
        });
  }

  public gotDate():void
  {
     if(this.Module.startDate != null && this.Module.endDate != null)
     {
        this.messhandler.SendDubbId(this.Module.courseid);
        this.messhandler.SendDubbType("Module");
        this.messhandler.SendDubbStart(new Date(this.Module.startDate+":00.000Z"));
        this.messhandler.SendDubbEnd(new Date(this.Module.endDate+":00.000Z"));
     }
  
    // post data
  }

   public move(): void
   {
      // Send to service => backend add diff date to all later once, change also start timdes to other with thesame start time
      this.errorMessage = "";
      this.validate();
      if(this.errorMessage=="")
      {
        this.ModuleService.MoveModule(this.Module.id,this.Module )
        .pipe(takeUntil(this.unsubscribe))
        .subscribe( status =>
          {
            this.errorMessage="Module updated"
            this.cd.markForCheck();
          }
          ,err =>  this.errorMessage = <any>err
          
          )
      }
   }

private validate():void
{
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
}
  public Register(theForm):void
  {
    this.errorMessage = "";
    this.validate();
    if(this.errorMessage=="")
    {
      this.ModuleService.EditModule(this.Module.id,this.Module )
      .pipe(takeUntil(this.unsubscribe))
      .subscribe( status =>
        {
          this.errorMessage="Module updated"
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

import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { IActivity, Activity, IActivityType } from 'ClientApp/app/Courses/course';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';
import { ActivitiesService } from '../activities.service';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { Scheduleentity } from 'ClientApp/app/Schedule/Scheduleentites';

@Component({
  selector: 'app-edit',
  templateUrl: './edit.component.html',
  styleUrls: ['./edit.component.css']
})
export class EditComponent implements OnInit, OnDestroy {
  private unsubscribe : Subject<void> = new Subject();
  Activity: IActivity  = new Activity();
  Modulestartdate: Date;
  Moduleenddate: Date;
  errorMessage: string="";
  ModuleName: string="";
  Activityid: string="";
  ActivityTypes:IActivityType[];
  Courseid: string ="";
  private startstartdate: Date;

   private schedulentity: Scheduleentity;
  constructor(private db: AuthService
    , private cd: ChangeDetectorRef ,private route: ActivatedRoute
    ,private messhandler: LoginMessageHandlerService
    ,private ActivititesService: ActivitiesService) { }

  ngOnInit() {
    this.Activityid = this.route.snapshot.paramMap.get("id"); // null if no hit?
    this.messhandler.Modulid
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status => 
      { 
        if (status != null) 
        {
       // let tmpguid= Guid.parse(status); 
          this.Activity.moduleid=status;
        }
      this.cd.markForCheck();
    }

    );
    this.messhandler.Courseid
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status =>{if (status!= null)
         {
       // let tmpguid= Guid.parse(status); 
        this.Courseid=status;
        
      }
      this.cd.markForCheck();
    }

    )

    this.messhandler.ModulStartDate
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status =>{if (status!= null) 
        {
        this.Modulestartdate=status;
        
      }
      this.cd.markForCheck();
    }
    )
    this.messhandler.ModulEndDate
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status =>{ if(status!= null) 
      {
        this.Moduleenddate=status;
      }
        this.cd.markForCheck();
    }
  
    )
    this.messhandler.ModulName
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      status => {
         if(status!= null)
         {
        this.ModuleName=status;
        
      }
      this.cd.markForCheck();
    }
    )
    this.ActivititesService.getActitityTypes()
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      resp => 
      {
        this.ActivityTypes=resp;
        this.cd.markForCheck();
      });
      this.ActivititesService.GetActivity(this.Activityid)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
        resp =>  
        { 
          this.Activity=resp;
          this.startstartdate=resp.startDate;
          this.cd.markForCheck();
        });
  }

  public gotDate():void
  {
     if(this.Activity.startDate != null && this.Activity.endDate != null)
     {
        let startdatework:Date
        if((this.Activity.startDate.toLocaleString().length==16))
        {
          startdatework=new Date(this.Activity.startDate+":00.000")
        }
        else
        {
          startdatework=new Date(this.Activity.startDate);
        }
        let enddatework:Date
        if((this.Activity.endDate.toLocaleString().length==16))
        {
          enddatework=new Date(this.Activity.endDate+":59.000")
        }
        else
        {
          enddatework=new Date(this.Activity.endDate);
        }
        this.messhandler.SendDubbId(this.Activity.moduleid);
        this.messhandler.SendDubbType("Activity");
        this.messhandler.SendDubbStart(startdatework);
        this.messhandler.SendDubbEnd(enddatework);
      //  this.messhandler.SendWeek(startdatework);
        this.messhandler.SendChangeEntity(this.buildEntity(startdatework,enddatework));
     }
  
    // post data
  }
  buildEntity(startdatework:Date,enddatework:Date): Scheduleentity {
    return  {
      weekday:"",
      id:this.Activity.id.toString(),
      color:this.Activity.color,
      length:null,
      offsettime:null,
      calcoffset:null,
      name:this.Activity.name,
      description:this.Activity.description,
      startTime:startdatework.toLocaleString(),
      endTime:enddatework.toLocaleString(),
      zindex:0,
      width:100,
      left:0,
      startD:startdatework,
      endD:enddatework,
      operationid:2
      
    }

  }
/*export class Scheduleentity
{
    weekday: string="";
    id:string;
    color: string;
    length:number;
    offsettime: number;
    name:string;
    description:string;
    startTime:string;
    endTime:string;
    zindex:number=0;
    width:number=100;
    left:number=0;
    activitytypid?:number=null;
    startD?:Date;
    endD?:Date;
}*/
  public move(): void
  {
     // Send to service => backend add diff date to all later once, change also start timdes to other with thesame start time
     this.errorMessage = "";
     this.validate();

     if(this.errorMessage=="")
     {
      if (this.startstartdate!=this.Activity.startDate )
      this.errorMessage=this.errorMessage+" Startdate change don't apply too move neighbor, however it will update this activity";
   
       this.ActivititesService.MoveActivity(this.Activity.id,this.Activity )
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

  private validate(): void
  {
    if(new Date(this.Activity.startDate+":00").valueOf()<this.Modulestartdate.valueOf()+1)
    {
        let asd=this.Activity.startDate.valueOf();
        let msd=this.Modulestartdate.valueOf();
        this.errorMessage= this.errorMessage + "Start date on activity may not be before module start date ("+this.Modulestartdate+")";
    }
    if(new Date(this.Activity.endDate+":59").valueOf()<this.Modulestartdate.valueOf())
    {
        this.errorMessage= this.errorMessage + "End date on activity may not be before module start date ("+this.Modulestartdate+")";
    } 
    if(new Date(this.Activity.startDate+":00").valueOf()>this.Moduleenddate.valueOf())
    {
        this.errorMessage= this.errorMessage + "Start date on activity may not be after module end date ("+this.Moduleenddate+")";
    }
    if(new Date(this.Activity.endDate+":59").valueOf()>this.Moduleenddate.valueOf())
    {
        this.errorMessage= this.errorMessage + "End date on activity may not be after module end date ("+this.Moduleenddate+")";
    } 
    if(this.Activity.endDate.valueOf()<this.Activity.startDate.valueOf())
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
      this.ActivititesService.EditActivity(this.Activity.id,this.Activity)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe( status =>
        {
 
            this.errorMessage="Activity updated"
  
         
          this.cd.markForCheck();
        }
        ,err =>  this.errorMessage = <any>err
        
        )
    }
  }

  ngOnDestroy(): void {
    this.messhandler.SendChangeEntity(null);
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }


}

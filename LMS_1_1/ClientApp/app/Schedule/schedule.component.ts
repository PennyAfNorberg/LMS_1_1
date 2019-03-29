import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject, VirtualTimeScheduler } from 'rxjs';
import { Router } from '@angular/router';
import { CourseService } from '../Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { Scheduleentites, weekdays, ScheduleFormModel, CourseSettingsViewModel } from './Scheduleentites';
import { Activity,Module } from '../Courses/course';
import { ScheduleService } from './schedule.service';
import { isDefaultChangeDetectionStrategy } from '@angular/core/src/change_detection/constants';
import { LoginMessageHandlerService } from '../Login/login-message-handler.service';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit, OnDestroy {

  private unsubscribe : Subject<void> = new Subject();
   private courseid:string;
   private scheduleFormModel: ScheduleFormModel= new ScheduleFormModel();
  private _week;
  errorMessage: string;
  actsub: any= null;
  courseSettings: CourseSettingsViewModel[];
  get week()
  {
    return this._week;
  }
  set week(value)
  {
    let temp=this.GetStartAndFromWeek(value);
    this.scheduleFormModel.startTime=temp[0];
    this.scheduleFormModel.endTime=temp[1];
    this._week=value;
    this.requery();
    
  }
   get startdate()
   {
     return this.scheduleFormModel.startTime;
   }
   set startdate(value)
   {
    this.scheduleFormModel.startTime=value;
    this.requery();
   }

   get enddate()
   {
    return this.scheduleFormModel.endTime;
  }
  set enddate(value)
  {
   this.scheduleFormModel.endTime=value;
   this.requery();
  }
  private _type:string;
  get type()
  {
    return this._type;
  }
  set type(value)
  {
    this._type= value;
    this.requery();
  }
  private weekdays: weekdays[]; //=["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
  private entities:Scheduleentites[][];
 
  constructor(private cd: ChangeDetectorRef , private router: Router
    , private CourseService: CourseService
    ,private ScheduleService :ScheduleService
    ,private messhandler: LoginMessageHandlerService 
    ) {
     }

  ngOnInit() {
     this.week=this.getWeekNumber(new Date());
     this.messhandler.Courseid
     .pipe(takeUntil(this.unsubscribe))
     .subscribe(
      (id: string)  =>
      {
         this.scheduleFormModel.courseId=id;
         this.cd.markForCheck();
      }); 
    
     this.weekdays=
    [
      {
       id:1,
      name:"Monday"
     },
     {
      id:2,
     name:"Tuesday"
    },
    {
      id:3,
     name:"Wednesday"
    },
    {
      id:4,
     name:"Thursday"
    },
    {
      id:5,
     name:"Friday"
    },
    ];
     this.type="Activities";

     // courseid
     // hämta data
     // format
     this.requery();
  
  }

  private requery(): any {
   // if all pars send q
    if(this.actsub!=null)
      this.actsub.unsubscribe();
    if(this.type=="Activities")
    {
      this.actsub=this.ScheduleService.GetActivitiesWithColour(this.scheduleFormModel)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
         (entities: Scheduleentites[][])  =>
         {
         // this.entities=entities;
            this.GetCourseSettings(entities);
            this.cd.markForCheck();
         },
         error => this.errorMessage = <any>error
      );
    }
    if(this.type=="Modules")
    {
      this.actsub=this.ScheduleService.GetModulesWithColour(this.scheduleFormModel)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
         (entities: Scheduleentites[][])  =>
         {
         // this.entities=entities;
            this.GetCourseSettings(entities);
            this.cd.markForCheck();
         },
         error => this.errorMessage = <any>error
      );
    }

  }

   private GetCourseSettings(entities: Scheduleentites[][])
   {
    this.ScheduleService.GetCourseSettings(this.scheduleFormModel.courseId, this.scheduleFormModel.startTime, this.scheduleFormModel.endTime)
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
        (cs:CourseSettingsViewModel[]) =>
        {
          this.courseSettings=cs;
          this.mapEntities(entities);
          this.cd.markForCheck();
        }

    );
   }

  mapEntities(entities: Scheduleentites[][]) {
    //Size...
    let size1=entities.length;
    for(let i=0; i<size1 ; i++)
    {
        let size2= entities[i].length;
        for( let j=0; j<size2; j++)
        {
            if(entities[i][j].length==null)
            {
               
              let endt:Date= new  Date(entities[i][j].endTime);
              let startt:Date=new Date(entities[i][j].startTime);
              let diff=this.datediff(endt,startt);
              if(diff<9) // eg daylength
                entities[i][j].length=diff;
           /*   else
              {
                while(diff>8)

              }*/

            }
        }
    }
    this.entities=entities;
  }

 

  private getWeekNumber(d:any):any {
    // Copy date so don't modify original
    d = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate()));
    // Set to nearest Thursday: current date + 4 - current day number
    // Make Sunday's day number 7
    d.setUTCDate(d.getUTCDate() + 4 - (d.getUTCDay()||7));
    // Get first day of year
    let yearStart:any = new Date(Date.UTC(d.getUTCFullYear(),0,1));
    // Calculate full weeks to nearest Thursday
    let weekNo = Math.ceil(( ( (d - yearStart) / 86400000) + 1)/7);
    // Return array of year and week number
    return d.getUTCFullYear()+"-W"+weekNo;
  }
  private GetStartAndFromWeek( w: string): Date[]
  {
    let year=w.substring(0,4);
    let weeknr=+w.substring(6)-1;
    let startstr=year+"-01-01T04:00:01";
    let endstr=year+"-01-01T23:59:59";
    let startd=new Date(startstr);
    let endd=new Date(endstr);
    let rest:number=this.mapFirstToStart(startd);
    startd=this.adddays(startd,7*weeknr+rest);
    endd=this.adddays(endd,7*weeknr+6+rest);
    //let startd:Date=new Date(Date.UTC(d.getUTCFullYear(),0,1))

    return [startd,endd];
  }
   private adddays(d:Date, days: number):Date
   {
    d.setDate(d.getDate() + days);
     return d;
   }

    private datediff(a:Date,b:Date):number
    {
      return (a.valueOf()-b.valueOf()) /(1000*60*60)
    }
private mapFirstToStart(d:Date):number
{
  switch( d.getDay())
  {
    case 1:
    {
        return 0;
    }
    case 2:
    {
        return -1;
    }  
    case 3:
    {
        return -2;
    }
    case 4:
    {
        return -3;
    }  
    case 5:
    {
        return 3;
    }  
    case 6:
    {
        return 2;
    }  
    default:
    {
      return 1;
    } 
  };
}
  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
   
}

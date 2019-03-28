import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject, VirtualTimeScheduler } from 'rxjs';
import { Router } from '@angular/router';
import { CourseService } from '../Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { Scheduleentites, weekdays } from './Scheduleentites';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit, OnDestroy {

  private unsubscribe : Subject<void> = new Subject();
   private courseid:string;
  private _week;
  get week()
  {
    return this._week;
  }
  set week(value)
  {
    let temp=this.GetStartAndFromWeek(value);
    this.startdate=temp[0];
    this.enddate=temp[1];
    this._week=value;
    
  }
   private _startdate:Date;
   get startdate()
   {
     return this._startdate;
   }
   set startdate(value)
   {
    this._startdate=value;
    this.requery();
   }
   private _enddate:Date;
   get enddtdate()
   {
    return this._enddate;
  }
  set enddate(value)
  {
   this._enddate=value;
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
  private Weekdays: weekdays[]; //=["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
  private entities:Scheduleentites[][];
 
  constructor(private cd: ChangeDetectorRef , private router: Router
    , private CourseService: CourseService) {
     }

  ngOnInit() {
     this.week=this.getWeekNumber(new Date());
     this.Weekdays=
    [
      {
       id:0,
      name:"Monday"
     },
     {
      id:1,
     name:"Tuesday"
    },
    {
      id:2,
     name:"Wednesday"
    },
    {
      id:3,
     name:"Thursday"
    },
    {
      id:4,
     name:"Friday"
    },
    ];
     this.type="Activities";

     // courseid
     // hämta data
     // format

     this.CourseService.getCourseById("1")
     .pipe(takeUntil(this.unsubscribe))
     .subscribe(
       status =>
       {
        this.cd.markForCheck();
       });
  }

  private requery(): any {
   // if all pars send q

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
    endd=this.adddays(endd,7*weeknr+7+rest);
    //let startd:Date=new Date(Date.UTC(d.getUTCFullYear(),0,1))

    return [startd,endd];
  }
   private adddays(d:Date, days: number):Date
   {
    d.setDate(d.getDate() + days);
     return d;
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

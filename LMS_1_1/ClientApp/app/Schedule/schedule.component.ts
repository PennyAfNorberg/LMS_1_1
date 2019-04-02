import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject, VirtualTimeScheduler } from 'rxjs';
import { Router } from '@angular/router';
import { CourseService } from '../Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { Scheduleentites, weekdays, ScheduleFormModel, CourseSettingsViewModel, findKmodel, ScheduleTimes, ScheduleColors } from './Scheduleentites';
import { Activity,Module } from '../Courses/course';
import { ScheduleService } from './schedule.service';
import { isDefaultChangeDetectionStrategy } from '@angular/core/src/change_detection/constants';
import { LoginMessageHandlerService } from '../Login/login-message-handler.service';
import { CssSelector } from '@angular/compiler';

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
  cls: ScheduleTimes[]=[];
  private  m:number=0;
 private n:number=1;
 private k:number=0;
 private savek:number=-1;
 private savei:number=-1;
  maxlength:number=2;
  errorMessage: string;
  actsub: any= null;
  position:string="relative";
  courseSettings: CourseSettingsViewModel[];
 private size2: number;
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
    // this.week=this.getWeekNumber(new Date());
     this.messhandler.Week
     .pipe(takeUntil(this.unsubscribe))
     .subscribe(
      (id: Date)  =>
      {
        this.week=this.getWeekNumber(id);
         this.cd.markForCheck();
      }); 
    
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
      name:"Monday",
      acronym:"Mon",
     },
     {
      id:2,
     name:"Tuesday",
     acronym:"Tu",
    },
    {
      id:3,
     name:"Wednesday",
     acronym:"Wed",
    },
    {
      id:4,
     name:"Thursday",
     acronym:"Th",
    },
    {
      id:5,
     name:"Friday",
     acronym:"Fri",
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
          this.calculatemaxlength();
          this.AddSchedulesTimes();
          this.mapEntities(entities);
          this.ListenForExternalChanges();
          this.cd.markForCheck();
        }

    );
   }
 private ListenForExternalChanges(): void {
    this.messhandler.ScheduleColor
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
          (newColor:ScheduleColors)  =>
           {
             if(newColor != null)
             {
                this.UpdateColor(newColor);
             }
            this.cd.markForCheck();
           }  
    );
    this.messhandler.ChangedEntity
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
          (newEntity:Scheduleentites) =>
          {
             if(newEntity != null)
             {
                this.AddEntity(newEntity);
             }
            this.cd.markForCheck();
          }
    );

    // add listen for cud
  }
 private AddEntity(newEntity: Scheduleentites):void {
  //  throw new Error("Method not implemented.");

  // get this.entities, ad this one, set calc props to null for all affekted, the same day? a and the runt map?
   let entities=this.entities;
   let est:Date,eend:Date;
   let nst:Date,nend:Date;
   nst=new Date(newEntity.startTime);
   nend=new Date(newEntity.endTime);

    for(let rowid=0; rowid< entities.length;rowid++)
    {
        for(let entid=0; entid <entities[rowid].length; entid++)
        {
            est=new Date(entities[rowid][entid].startTime);
            eend=new Date(entities[rowid][entid].endTime);
            if((nend>=est) && (eend<=nst) ) // fast hela dagarna?
            {
              entities[rowid][entid].length=null;
              entities[rowid][entid].offsettime=null;
              entities[rowid][entid].zindex=null;
              entities[rowid][entid].width=null;
              entities[rowid][entid].left=null;
              if(entid==0)
              { // strange?
                  // if nst >= fram  but <=ost and lst>olst  nst => 0, skift the rest
                  // else do nothing
              }
              else
              {
                // if lst<=nst<=rst and ll<=nl<=rll then nst between l och r, skift the rest 
                // else if no r, add nst last, no skift.
              }

            }

        }

       
    }
    this.mapEntities( entities);
  }



  private UpdateColor(newColor: ScheduleColors): void {

    for(let rowid=0; rowid< this.entities.length;rowid++)
    {
        for(let entid=0; entid <this.entities[rowid].length; entid++)
        {
  
            if(((newColor.id != null) && (newColor.id===this.entities[rowid][entid].id)) || 
            ((newColor.activitytypid!= null) && (newColor.activitytypid=== this.entities[rowid][entid].activitytypid)))
            {
              this.entities[rowid][entid].color=newColor.color;
            }
        
    
        }
      }
  }

  private AddSchedulesTimes(): any {
    let testdate=this.courseSettings[0].forDate;
    let lastend=new Date("200-01-01 17:00:00");
    let i=0;
    for(let cs of this.courseSettings)
    {
        if(testdate != cs.forDate)
          return;
        let st=new Date(cs.startTime);
        let et=new Date(cs.endTime);
        let offsettime:number=0;
        if(this.compDay(lastend,st))
        {
           offsettime=this.datediff(st,lastend);
        }
        this.cls[i]= 
        {
          name:st.getHours().toString(),
          length:this.datediff(et,st),
          offsettime:offsettime,
        } ;
        lastend=et;
        i++;

    }
  }
  calculatemaxlength(): void {
  
     let maxlen=0;
     let actlen=0;
    let  lastDate:string="";
    let startTime:Date;
    let endTime:Date;
    for(let cs of this.courseSettings)
    {
      startTime=new  Date(cs.startTime);
      endTime=new  Date(cs.endTime);
      if(cs.startTime.substring(0,10)==lastDate)
      {
        actlen+=1+this.datediff(endTime,startTime);
      }
      else
      {
        actlen=1.65+this.datediff(endTime,startTime);
        lastDate=cs.startTime.substring(0,10);
      }

      if(actlen>maxlen)
      {
        maxlen=actlen;
      }
    }
    this.maxlength=maxlen;
  }

 private mapEntities(entities: Scheduleentites[][]) {
    //Size...
    let size1=entities.length;
    let sizek= this.courseSettings.length;
    this.k=0;
  
    let startTime=new  Date(this.courseSettings[0].startTime);
    let endTime=new  Date(this.courseSettings[0].endTime);
    let startTime2:Date=startTime;
    let laststart=startTime;
    let lastend:Date=startTime;
    let parmsK:findKmodel ={startTime:startTime,endTime:endTime,laststart:laststart, lastend:lastend }
    let i=1;
    while(i<size1)
  //  for(let i=1; i<size1 ; i++)
    {
      if(this.savei != -1)
      {
        i=this.savei;
        this.savei=-1;
      }
      this.size2= entities[i].length;
      
        let j=0;
        while(j<this.size2)
      //  for( let j=0; j<size2; j++)
        {
          
          if(entities[i][j] && entities[i][j].length==null)
            {     
              let endt:Date= new  Date(entities[i][j].endTime);
              let startt:Date=new Date(entities[i][j].startTime);
              
              let paramsSet= {i:i,j:j};
              
              //find k
              this.findK(parmsK,startt,sizek );
             
              laststart=parmsK.startTime;
              let ent=entities[paramsSet.i][paramsSet.j];
              this.setN(entities,ent,paramsSet);

              //set present
              
              this.setPresent(entities,parmsK,paramsSet,startt,endt,sizek, size1)
              i=paramsSet.i;
              j=paramsSet.j;
              
           
            }
            else
            {
              j++;
            }
        }
        i++;
    }
    this.entities=entities;
  }

  private findK(parmsK: findKmodel, startt :Date, sizek:number): any {
    if(this.savek != -1)
    {
      this.k=this.savek;
      parmsK.startTime=new  Date(this.courseSettings[this.k].startTime);
      parmsK.endTime=new  Date(this.courseSettings[this.k].endTime);
      parmsK.laststart=(this.k>0)?new  Date(this.courseSettings[this.k-1].startTime):parmsK.startTime;
      parmsK.lastend=(this.k>0)?new  Date(this.courseSettings[this.k-1].endTime):parmsK.endTime;
      this.savek=-1;
    }
    while((parmsK.endTime < startt)  && (this.k< sizek) )
    {
      this.k++;
      let startTime2=new  Date(this.courseSettings[this.k].startTime);
      if(this.compDay(parmsK.lastend,startTime2))
      {
        parmsK.lastend=parmsK.endTime;
      }
      else
      { // new Day
        parmsK.lastend=startTime2;
      }
      parmsK.startTime=startTime2;
      parmsK.endTime=new  Date(this.courseSettings[this.k].endTime);
    }
  } 

   private checkpred(s1:Date,s2:Date,e1:Date, e2:Date ): boolean
   {
     return (s1<= e2) && (e1 >=s2);
   }
   private setN(entities: Scheduleentites[][],ent:Scheduleentites,paramsSet: {i:number,j:number}): void
   {
      
      // only call if not set so no check if cell is worked.
      if(this.courseSettings[this.k].n==-1)
      {
        let startTime=new  Date(ent.startTime);
        let endTime=new  Date(ent.endTime);
        let csStartTime=new Date(this.courseSettings[this.k].startTime);
        let csEndTime =new Date(this.courseSettings[this.k].endTime);
        startTime=(startTime<csStartTime || startTime > csEndTime)?csStartTime:startTime;
        endTime=(endTime>csEndTime || endTime<csStartTime)?csEndTime:endTime;
        //let tmpent=entities[paramsSet.i];
       // let test=[];
         let i=0;
        for(let row of entities)
        {
           for(let cs of row)
           {
              if(this.checkpred(new Date(cs.startTime), startTime,new Date(cs.endTime), endTime ))
              {
              //  test[i]=cs;
                i++;
              }
            }
        } 

        this.courseSettings[this.k].n=i;
      }
      
      if(this.courseSettings[this.k].n != -1)
      {
         this.n=this.courseSettings[this.k].n ;
      }
      else
      {
        this.n=1;              
      }
      this.m=this.courseSettings[this.k].m;
      if((this.n>1)&& (this.m+1<this.n) && this.savek==-1)
      {
        this.savek=this.k;
      }
   }

 private setPresent(entities: Scheduleentites[][], parmsK: findKmodel, paramsSet: { i: number; j: number; }, startt: Date, endt: Date,sizek :number,size1:number): void
 {
  entities[paramsSet.i][paramsSet.j].offsettime=this.datediff(startt,parmsK.startTime)
   if(this.m==0)
   {
    entities[paramsSet.i][paramsSet.j].offsettime+=this.datediff(parmsK.startTime,parmsK.lastend);
   }
   else
   {
    entities[paramsSet.i][paramsSet.j].offsettime+=entities[paramsSet.i][paramsSet.j-1].offsettime-entities[paramsSet.i][paramsSet.j-1].length;
   }
    entities[paramsSet.i][paramsSet.j].zindex=this.m;
    entities[paramsSet.i][paramsSet.j].width=100*(this.n-this.m)/this.n;
    entities[paramsSet.i][paramsSet.j].left=100*this.m/this.n;
    this.courseSettings[this.k].m++;
    this.m++;
  
    let tmpstartt=(parmsK.startTime>startt)?parmsK.startTime:startt
    let tmpendt=(parmsK.endTime<endt)?parmsK.endTime:endt;
    entities[paramsSet.i][paramsSet.j].length=this.datediff(tmpendt,tmpstartt);
   if((startt=>parmsK.startTime) && (endt<= parmsK.endTime)  )
   {
    paramsSet.j++;  
   }
   else
   {
    this.loopNext(entities,parmsK,paramsSet,startt,endt,sizek,size1 );
    //loop next
    }
  }


 private loopNext(entities: Scheduleentites[][], parmsK: findKmodel, paramsSet: { i: number; j: number; }, startt: Date, endt: Date,sizek :number, size1:number): any {
    let ent=entities[paramsSet.i][paramsSet.j];
    this.k++;
    parmsK.lastend=parmsK.endTime;
    parmsK.laststart=parmsK.startTime;
    parmsK.startTime=new  Date(this.courseSettings[this.k].startTime);
    parmsK.endTime=new  Date(this.courseSettings[this.k].endTime);
       
    paramsSet.j++;
    while((parmsK.startTime<= endt) && (paramsSet.i< size1) && (this.k<sizek))
    {
       // next slot
       if(!this.compDay(parmsK.laststart,parmsK.startTime)) 
       {
          this.savei=paramsSet.i;
          paramsSet.i++;
          paramsSet.j=0;
          parmsK.lastend=parmsK.startTime;
          parmsK.laststart=parmsK.startTime;
          this.size2= entities[paramsSet.i].length;
          while(entities[paramsSet.i][paramsSet.j] && entities[paramsSet.i][paramsSet.j].length!= null &&  paramsSet.j<this.size2 )
            paramsSet.j++;

        }
        if(paramsSet.i< size1)
        { // måste placera rätt här.
          this.setN(entities,ent,paramsSet ); 


          let offsettime=0;
          offsettime+=this.datediff(parmsK.startTime,parmsK.lastend);
          if(this.m==0)
          {
              if((this.courseSettings[this.k-1].m >0) && ( paramsSet.j>0))
              {
                offsettime+=entities[paramsSet.i][paramsSet.j-1].offsettime;
              }
          }
          if(this.m>0)
          {
           
            if(paramsSet.j>0)
              offsettime+=entities[paramsSet.i][paramsSet.j-1].offsettime-entities[paramsSet.i][paramsSet.j-1].length;
          }
     
            entities[paramsSet.i].splice(paramsSet.j,0,{
              weekday:"",
              id:"",
              color:ent.color,
              length:(parmsK.endTime>endt)?this.datediff(endt,parmsK.startTime):this.datediff(parmsK.endTime,parmsK.startTime),
              offsettime:offsettime,
              name:ent.name,
              description:ent.description,
              startTime:(parmsK.startTime>startt)?this.courseSettings[this.k].startTime:ent.startTime,
              endTime:(parmsK.endTime<endt)?this.courseSettings[this.k].endTime:ent.endTime,
              zindex:this.m,
              width:100*(this.n-this.m)/this.n,
              left:100*this.m/this.n
              });
              this.size2++;
              this.courseSettings[this.k].m++;
              this.m++;
        }
        paramsSet.j++; 
        this.k++;
        if(this.k==sizek)
            break;
        else
        {  
          this.setN(entities,ent,paramsSet);
          let startTime2=new  Date(this.courseSettings[this.k].startTime);
          if(this.compDay(parmsK.laststart,startTime2))
          {
            parmsK.lastend=parmsK.endTime;
          }
          else
          { // new Day
            parmsK.lastend=startTime2;
          }
          parmsK.laststart=parmsK.startTime;
          parmsK.startTime=startTime2;
          parmsK.endTime=new  Date(this.courseSettings[this.k].endTime);
          
        }  
    }
  }




private compDay(a:Date, b:Date): boolean
{
 return a.toISOString().substr(0,10) == b.toISOString().substring(0,10);

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

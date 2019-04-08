import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject, VirtualTimeScheduler, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CourseService } from '../Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { Scheduleentity, weekdays, ScheduleFormModel, CourseSettingsViewModel, findKmodel, ScheduleTimes, ScheduleColors } from './Scheduleentites';
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
 private deleteColor:string="#cd3434";
  maxlength:number=2;
  errorMessage: string;
  actsub: any= null;
  sub:any =null;
  position:string="relative";
  courseSettings: CourseSettingsViewModel[];
 private size2: number;
  getinprogress: boolean=false;
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
    if(!this.getinprogress)
    {
       this.requery();
    }
    
  }
   get startdate()
   {
     return this.scheduleFormModel.startTime;
   }
   set startdate(value)
   {
    this.scheduleFormModel.startTime=value;
    if(!this.getinprogress)
    {
       this.requery();
    }
   }

   get enddate()
   {
    return this.scheduleFormModel.endTime;
  }
  set enddate(value)
  {
   this.scheduleFormModel.endTime=value;
   if(!this.getinprogress)
   {
      this.requery();
   }
  }
  private _type:string;
  get type()
  {
    return this._type;
  }
  set type(value)
  {
    this._type= value;
    if(!this.getinprogress)
    {
       this.requery();
    }
  }
  private weekdays: weekdays[]; //=["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
  private entities:Scheduleentity[][];
 
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

  private async requery() :Promise<void> {
   // if all pars send q
  
   let entities: Scheduleentity[][];
   let cs:CourseSettingsViewModel[];
   let newColor:ScheduleColors;
   
    if(this.sub!=null)
      this.sub.unsubscribe();
      if(this.actsub!=null)
      this.actsub.unsubscribe();  
      this.getinprogress=true;
    if(this.type=="Activities")
    {
       
       entities = await this.ScheduleService
      .GetActivitiesWithColour(this.scheduleFormModel).toPromise();
       
/*
      this.actsub=this.ScheduleService.GetActivitiesWithColour(this.scheduleFormModel)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
         (entities: Scheduleentity[][])  =>
         {
         // this.entities=entities;
            this.GetCourseSettings(entities);
            
            this.cd.markForCheck();
         },
         error => this.errorMessage = <any>error
      );*/
    }
    if(this.type=="Modules")
    {
      entities = await this.ScheduleService
      .GetModulesWithColour(this.scheduleFormModel).toPromise();


    /*  this.actsub=this.ScheduleService.GetModulesWithColour(this.scheduleFormModel)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
         (entities: Scheduleentity[][])  =>
         {
         // this.entities=entities;
            this.GetCourseSettings(entities);
            this.cd.markForCheck();
         },
         error => this.errorMessage = <any>error
      );*/
    }
    cs=await this.ScheduleService.GetCourseSettings(this.scheduleFormModel.courseId, this.scheduleFormModel.startTime, this.scheduleFormModel.endTime)
    .toPromise();
    if(cs!= null)
    {
      this.courseSettings=cs;
      this.calculatemaxlength();
      this.AddSchedulesTimes();
      this.mapEntities(entities);
      this.actsub=this.ListenForExternalChangesColor()
      this.sub= this.ListenForExternalChanges();
    }
    this.getinprogress=false;
  }

   private async GetCourseSettings(entities: Scheduleentity[][])
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
          this.getinprogress=false;
          this.ListenForExternalChanges();
          this.cd.markForCheck();
        }

    );
   }
 private ListenForExternalChangesColor(): Subscription {
   return  this.messhandler.ScheduleColor
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
          }

    private  ListenForExternalChanges(): Subscription {
  return  this.messhandler.ChangedEntity
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
          (newEntity:Scheduleentity) =>
          {
             if(newEntity != null)
             {
                if(!this.getinprogress)
                {
              this.getinprogress=true;
             
              

                this.AddEntityStart(newEntity);

                this.getinprogress=false;
                }
             }
            this.cd.markForCheck();
          }
    );

  
  }
 private async AddEntityStart(newEntity: Scheduleentity):Promise<void> {
  //  throw new Error("Method not implemented.");
  let week=this.getWeekNumber(newEntity.startD);
  let temp=this.GetStartAndFromWeek(week);
  this.week=week;
   let newscheduleFormModel:ScheduleFormModel=
   {
    courseId:this.scheduleFormModel.courseId,
    startTime:temp[0],
    endTime:temp[1]
   }
  let  ReqEntities = await this.ScheduleService
  .GetActivitiesWithColour(this.scheduleFormModel).toPromise();
  let cs=await this.ScheduleService.GetCourseSettings(this.scheduleFormModel.courseId, this.scheduleFormModel.startTime, this.scheduleFormModel.endTime)
  .toPromise();
  if(cs!= null)
  {
    this.courseSettings=cs;
    this.calculatemaxlength();
    this.AddSchedulesTimes();
    this.mapEntities(ReqEntities);
    this.AddEntity(newEntity);
  }
 }
 private  AddEntity(newEntity: Scheduleentity):void {

  // get this.entities, ad this one, set calc props to null for all affekted, the same day? a and the runt map?
   let entities=this.entities;
   let est:Date,eend:Date,lastst:Date;
   let nst:Date,nend:Date,lastend:Date;
    let tmpnewEntity = {... newEntity};
  // newEntity.startD=new Date(newEntity.startTime);
  // newEntity.endD=new Date(newEntity.endTime);
   nst=newEntity.startD;
   nend=newEntity.endD;
   let deleted:boolean;
   let undone=true;
//GrEDate(a:Date, b:Date)
let rowid=1
let entid=0
let maxentid=0;
lastst=entities[rowid][entid].startD;
lastend=entities[rowid][entid].endD;
   while(rowid< entities.length)
    {
        entid=0
        maxentid=entities[rowid].length;
        while(entid <maxentid)
        {
          deleted=false;
            est=entities[rowid][entid].startD;
            eend=entities[rowid][entid].endD;
            if(newEntity.operationid==2 && entities[rowid][entid].operationid!=2)
            {
              if(newEntity.id==entities[rowid][entid].id)
              {

                  entities[rowid].splice(entid,1);
                  maxentid--;
                  deleted=true;
              }

            }
            if(newEntity.operationid==3)
            {
              if(newEntity.id==entities[rowid][entid].id)
              {
                  entities[rowid][entid].color=this.deleteColor;
                  
              }

            }    

            if(newEntity.operationid==1 || newEntity.operationid==2)
            {
              if((entid==0 || entid<maxentid) && this.GrEDate(nend,est) && this.GrEDate(eend,nst) ) // fast hela dagarna?
              {
                this.resetNM(est,eend);
                if(!deleted)
                {
                  entities[rowid][entid].length=null;
                  entities[rowid][entid].offsettime=null;
                  entities[rowid][entid].zindex=null;
                  entities[rowid][entid].width=null;
                  entities[rowid][entid].left=null;
                }
                if(undone)
                {
                  if(entid==0)
                  { // strange?
                      // if nst >= fram  but <=ost and lst>olst  nst => 0, skift the rest
                      if(newEntity.startD<= est && this.datediff(newEntity.endD,newEntity.startD)>= this.datediff(eend, est))
                      {
                        entities[rowid].splice(entid,0,tmpnewEntity);
                        maxentid++;
                        undone=false;
                      }
                      else
                      {
                        if (entid+1>=entities[rowid].length)
                        {
                          entities[rowid].splice(entid,0,tmpnewEntity);
                          maxentid++;
                          undone=false;
                        } 

                      }
                      // else do nothing
                  }
                  else
                  {
                    // if lst<=nst<=rst and ll<=nl<=rll then nst between l och r, skift the rest 
                    if((lastst<=newEntity.startD)
                    && (newEntity.startD<=est)
                    && (this.datediff(lastend,lastst)>=this.datediff(newEntity.endD,newEntity.startD) )
                    && (this.datediff(newEntity.endD,newEntity.startD) >=this.datediff(eend,est) ))
                    {
                      entities[rowid].splice(entid,0,tmpnewEntity);
                      maxentid++;
                      undone=false;
                    } 
                    else
                    {
                        if (entid+1>=entities[rowid].length)
                        {
                          entities[rowid].splice(entid,0,tmpnewEntity);
                          maxentid++;
                          undone=false;
                        } 

                    }
                    // else if no r, add nst last, no skift.
                  }
                }
             }
             if(deleted) entid--;
             lastst=est;
             lastend=eend;

        }
         entid++;
      }
      rowid++;
    }
    this.mapEntities( entities);
  }
 private resetNM(est: Date, eend: Date): void {
    let k=0;
    let sizek=this.courseSettings.length;
    let lastend=this.courseSettings[k].startD;
    while((k< sizek) && (this.courseSettings[k].endD < est)    )
    {
      k++;
      let startTime2=this.courseSettings[k].startD;
      if(this.compDay(lastend,startTime2))
      {
        lastend=this.courseSettings[k].endD;
      }
      else
      { // new Day
        lastend=startTime2;
      }
     
      
    }
    while((k< sizek) && this.courseSettings[k].endD <= eend  )
    {
        this.courseSettings[k].n=-1;
        this.courseSettings[k].m=0;
        k++;

    }

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

        let st=cs.startD;
        let et=cs.endD;
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
    for(let i=0;i<this.courseSettings.length;i++)
    {
        if(this.courseSettings[i].startD==null)
          this.courseSettings[i].startD=new  Date(this.courseSettings[i].startTime);
        if(this.courseSettings[i].endD==null)
          this.courseSettings[i].endD=new  Date(this.courseSettings[i].endTime);  

    }
    for(let cs of this.courseSettings)
    {
      startTime=cs.startD
      endTime=cs.endD;
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

 private mapEntities(entities: Scheduleentity[][]) {
    //Size...
     if(entities)
     {
    let size1=entities.length;
    let sizek= this.courseSettings.length;
    this.k=0;
  
    let startTime=this.courseSettings[0].startD;
    let endTime=this.courseSettings[0].endD;
    let startTime2:Date=startTime;
    let laststart=startTime;
    let lastend:Date=startTime;
    let parmsK:findKmodel ={startTime:startTime,endTime:endTime,laststart:laststart, lastend:lastend, nextstart:this.courseSettings[1].startD}
    let i=1;
    let j=0;
    for(i=1; i< entities.length;i++)
    {
      for(j=0; j<entities[i].length;j++)
      {
        if(entities[i][j].startD==null)
        {
          entities[i][j].startD=new Date(entities[i][j].startTime);
        }
        if(entities[i][j].endD==null)
        {
          entities[i][j].endD=new Date(entities[i][j].endTime);
        }
      }
    }

    i=1;
    while(i<size1)
  //  for(let i=1; i<size1 ; i++)
    {
      if(this.savei != -1)
      {
        i=this.savei;
        this.savei=-1;
      }
      this.size2= entities[i].length;
      
         j=0;
        while(j<this.size2)
      //  for( let j=0; j<size2; j++)
        {
          
          if(entities[i][j] && entities[i][j].length==null)
            {    
              
              
              let endt:Date=entities[i][j].endD;
              let startt:Date=entities[i][j].startD;
              
              let paramsSet= {i:i,j:j};
              
              //find k
              this.findK(parmsK,startt,sizek );
             
              laststart=parmsK.startTime;
              let ent=entities[paramsSet.i][paramsSet.j];
              this.setN(entities,0,sizek);

              //set present
              
              this.setPresent(entities,parmsK,paramsSet,startt,endt,sizek, size1)
              i=paramsSet.i;
              j=paramsSet.j;
              
              if(this.savei != -1)
              {
                i=this.savei;
                this.savei=-1;
                j=0;
              }
              this.size2= entities[i].length;
              if(this.k>=sizek)
                j++;
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
  }

  private findK(parmsK: findKmodel, startt :Date, sizek:number): any {
    if(this.savek != -1 || this.courseSettings[this.k].savek != -1)
    {
      if(this.courseSettings[this.k].savek != -1 &&  this.savek == -1)
      {
        this.savek=this.courseSettings[this.k].savek;
        this.courseSettings[this.k].savek=-1;
      }

      this.k=this.savek;
      parmsK.startTime=this.courseSettings[this.k].startD;
      parmsK.endTime=this.courseSettings[this.k].endD;
      parmsK.laststart=(this.k>0)?this.courseSettings[this.k-1].startD:parmsK.startTime;
      parmsK.lastend=(this.k>0)?this.courseSettings[this.k-1].endD:parmsK.endTime;
      if(this.k>0&& this.k+1<sizek && this.compDay(parmsK.startTime,this.courseSettings[this.k-1].startD))
      {
        parmsK.nextstart=parmsK.startTime;
      }
      else
      {
        parmsK.nextstart=this.courseSettings[this.k+1].startD;
        
      }
      this.savek=-1;
    }
    else
    {
      parmsK.laststart=parmsK.startTime;
    }
    while((this.k< sizek)  && (parmsK.endTime < startt)    )
    {
      this.k++;
      let startTime2=this.courseSettings[this.k].startD;
      if(this.compDay(parmsK.lastend,startTime2))
      {
        parmsK.lastend=parmsK.endTime;
        parmsK.nextstart=parmsK.endTime;
      }
      else
      { // new Day
        parmsK.lastend=startTime2;
    
          parmsK.nextstart=startTime2;//this.courseSettings[this.k+1].startD;
        
      }
      parmsK.startTime=startTime2;
      parmsK.endTime=this.courseSettings[this.k].endD;
      
    }
  } 

   private checkpred(s1:Date,s2:Date,e1:Date, e2:Date ): boolean
   {
     return (s1<= e2) && (e1 >=s2);
   }
   private setN(entities: Scheduleentity[][], extraN:number, sizek:number): void
   {
      if(this.k<sizek)
      {

      
      // only call if not set so no check if cell is worked.
      if(this.courseSettings[this.k].n==-1)
      {
      //  let startTime=ent.startD;
       // let endTime=ent.endD;
        let csStartTime=this.courseSettings[this.k].startD;
        let csEndTime =this.courseSettings[this.k].endD;
       // startTime=(startTime<csStartTime || startTime > csEndTime)?csStartTime:startTime;
       // endTime=(endTime>csEndTime || endTime<csStartTime)?csEndTime:endTime;
        //let tmpent=entities[paramsSet.i];
       // let test=[];
         let i=0;
        for(let row of entities)
        {
           for(let cs of row)
           {
              if(this.checkpred(cs.startD,csStartTime,cs.endD, csEndTime ))
              {
              //  test[i]=cs;
                i++;
              }
            }
        } 

        this.courseSettings[this.k].n=i+extraN;
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
      if((this.n>1)&& (this.m+1<this.n))
      {
        if( this.savek==-1)
        {
           this.savek=this.k;
        }
        else
        {
            this.courseSettings[this.savek].savek=this.k;
        }
      }
    }
   }

 private setPresent(entities: Scheduleentity[][], parmsK: findKmodel, paramsSet: { i: number; j: number; }, startt: Date, endt: Date,sizek :number,size1:number): void
 {
 let precalsoffset:number=0;
  let ent=entities[paramsSet.i][paramsSet.j];
  entities[paramsSet.i][paramsSet.j].offsettime=this.datediff(startt,parmsK.startTime)
   if(this.m==0)
   {
    entities[paramsSet.i][paramsSet.j].offsettime+=this.datediff(parmsK.startTime,parmsK.lastend);
   }
   else
   {
    //precalsoffset=this.datediff(parmsK.startTime,parmsK.nextstart);
     if(paramsSet.j>0)
     {
        entities[paramsSet.i][paramsSet.j].offsettime+=this.datediff(ent.startD,entities[paramsSet.i][paramsSet.j-1].startD)+entities[paramsSet.i][paramsSet.j-1].offsettime-entities[paramsSet.i][paramsSet.j-1].length;
     }
   }
   entities[paramsSet.i][paramsSet.j].calcoffset=entities[paramsSet.i][paramsSet.j].offsettime-precalsoffset;
    entities[paramsSet.i][paramsSet.j].zindex=this.m;
    entities[paramsSet.i][paramsSet.j].width=100*(this.n-this.m)/this.n;
    entities[paramsSet.i][paramsSet.j].left=100*this.m/this.n;
    this.courseSettings[this.k].m++;
    this.m++;
  
    let tmpstartt=(parmsK.startTime>startt)?parmsK.startTime:startt
    let tmpendt=(parmsK.endTime<endt)?parmsK.endTime:endt;
    entities[paramsSet.i][paramsSet.j].startD=tmpstartt;
    entities[paramsSet.i][paramsSet.j].endD=tmpendt;
    entities[paramsSet.i][paramsSet.j].startTime=tmpstartt.toLocaleString();
    entities[paramsSet.i][paramsSet.j].endTime=tmpendt.toLocaleString();
    entities[paramsSet.i][paramsSet.j].operationid=null;
    entities[paramsSet.i][paramsSet.j].length=this.datediff(tmpendt,tmpstartt);
   if((startt=>parmsK.startTime) && (endt<= parmsK.endTime)  )
   {
    paramsSet.j++;  
   }
   else
   {
    this.loopNext(entities,ent,parmsK,paramsSet,startt,endt,sizek,size1 );
    //loop next
    }

  }


 private loopNext(entities: Scheduleentity[][],ent:Scheduleentity, parmsK: findKmodel, paramsSet: { i: number; j: number; }, startt: Date, endt: Date,sizek :number, size1:number): any {
    
    this.k++;
    parmsK.lastend=parmsK.endTime;
    parmsK.laststart=parmsK.startTime;
    parmsK.startTime=this.courseSettings[this.k].startD;
    parmsK.endTime=this.courseSettings[this.k].endD;
       
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
          this.setN(entities,1 ,sizek); 
          // need to count the next insert too
 

          let offsettime=0, precalcoffsettime=0;
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
            precalcoffsettime=0;
            if(paramsSet.j>0)
              offsettime+=entities[paramsSet.i][paramsSet.j-1].calcoffset-entities[paramsSet.i][paramsSet.j-1].length;
          }
     
            entities[paramsSet.i].splice(paramsSet.j,0,{
              weekday:"",
              id:ent.id,
              color:ent.color,
              length:(parmsK.endTime>endt)?this.datediff(endt,parmsK.startTime):this.datediff(parmsK.endTime,parmsK.startTime),
              offsettime:offsettime,
              calcoffset:offsettime-precalcoffsettime,
              name:ent.name,
              description:ent.description,
              startTime:(parmsK.startTime>startt)?this.courseSettings[this.k].startTime:ent.startTime,
              endTime:(parmsK.endTime<endt)?this.courseSettings[this.k].endTime:ent.endTime,
              zindex:this.m,
              width:100*(this.n-this.m)/this.n,
              left:100*this.m/this.n,
              startD:(parmsK.startTime>startt)?parmsK.startTime:startt,
              endD:(parmsK.endTime<endt)?parmsK.endTime:endt,
              operationid:null
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
        //  this.setN(entities,0,sizek);
          let startTime2=this.courseSettings[this.k].startD;
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
          parmsK.endTime=this.courseSettings[this.k].endD;
          
        }  
    }
  }




private compDay(a:Date, b:Date): boolean
{
 return a.toISOString().substr(0,10) == b.toISOString().substring(0,10);

}

 private GrEDate(a:Date, b:Date): boolean
 {
   // checks is a>=b for day ( yyyy-mm-dd)
  let aloc:Date=new Date(a.toLocaleString());
  let bloc:Date=new Date(b.toLocaleString());
  aloc.setHours(1,0,0,0);
  bloc.setHours(1,0,0,0);
   return aloc>=bloc;

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

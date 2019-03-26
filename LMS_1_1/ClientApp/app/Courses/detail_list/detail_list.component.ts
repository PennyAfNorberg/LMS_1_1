import { Component, OnInit, Input, OnDestroy, ChangeDetectorRef } from "@angular/core";
import { ICourse, course } from '../course';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../course.service';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { Subscription, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';

@Component({
    selector: "detail_list",
    templateUrl: "detail_list.component.html",
    styleUrls:[]
})

export class detailList implements OnInit, OnDestroy {

     course: ICourse;
     errorMessage: string;
     @Input()   courseid: string;
     private unsubscribe : Subject<void> = new Subject();

   private savesubs: Array<[string,Subscription]>= new Array<[string,Subscription]>();
     isTeacher: boolean;
    constructor(private route: ActivatedRoute,
        private CourseService: CourseService
        , private AuthService : AuthService
        ,private cd: ChangeDetectorRef
        ,private messhandler: LoginMessageHandlerService
        ) 
        { }
    
    ngOnInit() {
        this.isTeacher=this.AuthService.isTeacher;
        this.messhandler.SendModulid(null);
        this.messhandler.SendModulStartDate(null);
        this.messhandler.SendModulEndDate(null);
        this.messhandler.SendModulName(null);
        /*this.AuthService.isTeacher
        .pipe(takeUntil(this.unsubscribe))
        .subscribe( i =>{
             this.isTeacher=i;
             this.cd.markForCheck();
        }
        );*/


        this.CourseService.getCourseAndModulebyId(this.courseid)
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(
                course => {
                    this.course = course;
                    this.cd.markForCheck();
                },
                error => this.errorMessage = <any>error
            );
    }

    public TogggelCollapse(mid: string): void
    {
         if(this.course.modules.find(m => m.id.toString()==mid).isExpanded ==" show")
        {

              this.course.modules.find(m => m.id.toString()==mid).isExpanded="";
              if (this.savesubs.find( t => t[0]==mid))
              {

                  this.savesubs.find( t => t[0]==mid)[1].unsubscribe();
                  this.savesubs.splice(this.savesubs.indexOf(this.savesubs.find( t => t[0]==mid)),1);
              }

        }
         else
        {
           this.course.modules.find(m => m.id.toString()==mid).isExpanded=" show";
           this.messhandler.SendModulid(mid);
           let mod=this.course.modules.find(m => m.id.toString()==mid);
           let localstart: Date=mod.startDate;
           localstart=new Date("2019-03-26 16:00:05");
           if(localstart.getHours()<17)
           {
            localstart.setDate(localstart.getDate() - 1);
           }
           this.messhandler.SendModulStartDate(localstart);
           let localend:Date = mod.endDate;
           if(localend.getHours()>12)
           {
            localend.setDate(localend.getDate() + 1);
           }
           this.messhandler.SendModulEndDate(localend);
           this.messhandler.SendModulName(mod.name);
           let temp=this.CourseService.getActivitybymodulId(mid)
           .pipe(takeUntil(this.unsubscribe))
           .subscribe(
                    activities=>
                    {
                        this.course.modules.find(m => m.id.toString()==mid).activities=activities;
                        this.cd.markForCheck();
                    },
                    error => this.errorMessage = <any>error
                );
            if (this.savesubs.find( t => t[0]==mid))
            {
                this.savesubs.find( t => t[0]==mid)[1]=temp;
            }
            else
            {
                this.savesubs.push([mid,temp])  ;
            }
        }
    }
    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
      }
   
}
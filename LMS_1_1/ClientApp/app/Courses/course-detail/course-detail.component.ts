import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ICourse, course } from '../course';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from '../course.service';
import { Guid } from 'guid-typescript';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PartipantService } from 'ClientApp/app/AddPartipant/partipant.service';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';


@Component({
 
  templateUrl: './course-detail.component.html',
  styleUrls: ['./course-detail.component.css']
})
export class CourseDetailComponent implements OnInit, OnDestroy  {

    course: ICourse;
    errorMessage: string;
    isTeacher: boolean=false;
    showpartipantlist:boolean=false;
  showpartipantlistmsg: string="Show";

    private unsubscribe : Subject<void> = new Subject();
    constructor(private route: ActivatedRoute, private CourseService: CourseService, private AuthService : AuthService
      ,private cd: ChangeDetectorRef
      ,private partipantservice:PartipantService
      ,private messhandler: LoginMessageHandlerService
      ) { }

    ngOnInit(): void {
      this.isTeacher=this.AuthService.isTeacher;
        /*this.AuthService.isTeacher
        .pipe(takeUntil(this.unsubscribe))
        .subscribe( i =>
          { 
            this.isTeacher=i;
            this.cd.markForCheck(); 
          });*/
        let id: string = this.route.snapshot.paramMap.get('id');
        this.messhandler.SendCourseid(id);
        this.messhandler.SendWeek(new Date());
        
        this.CourseService.getCourseAndModulebyId(id)
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(
                (course : ICourse) => {
                    this.course = course;
                    this.messhandler.SendCourseStartDate(course.startDate);
                    this.messhandler.SendCourseName(course.name);
                    this.cd.markForCheck();
                },
                error => this.errorMessage = <any>error
            );
  }

   public toggelPartipantList()
   {
      if(this.showpartipantlist)
      {
          this.showpartipantlist=false;
          this.showpartipantlistmsg="Show";
          
      }
      else
      {
        this.showpartipantlist=true;
        this.showpartipantlistmsg="Hide";
      }
      this.partipantservice.SendPartipantList(this.showpartipantlist);
   }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}

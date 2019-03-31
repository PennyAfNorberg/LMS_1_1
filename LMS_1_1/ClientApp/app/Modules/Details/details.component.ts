import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { IModule } from 'ClientApp/app/Courses/course';
import { ActivatedRoute } from '@angular/router';
import { CourseService } from 'ClientApp/app/Courses/course.service';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';

@Component({
  selector: 'app-details',
  templateUrl: './details.component.html',
  styleUrls: ['./details.component.css']
})
export class ModulDetailsComponent implements OnInit, OnDestroy  {

  private unsubscribe : Subject<void> = new Subject();
  module: IModule;
  errorMessage: string;
  isTeacher: boolean;
  constructor(private route: ActivatedRoute, private CourseService: CourseService, private AuthService : AuthService
    , private cd: ChangeDetectorRef
    ,private messhandler: LoginMessageHandlerService
    ) { }

  ngOnInit(): void {
    this.isTeacher=this.AuthService.isTeacher;
     /* this.AuthService.isTeacher
      .pipe(takeUntil(this.unsubscribe))
      .subscribe( i => this.isTeacher=i);
      */
      let Modulid: string = this.route.snapshot.paramMap.get('id');
      this.messhandler.SendWeek(new Date());
      this.CourseService.getModulAndActivitybyId(Modulid)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(
          modul=> {
                  this.module = modul;
                  this.cd.markForCheck();
              },
              error => this.errorMessage = <any>error
          );     
      }

      ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
      }
      

}

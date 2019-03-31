import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ModuleService } from '../module.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CourseService } from 'ClientApp/app/Courses/course.service';
import { IModule, course } from 'ClientApp/app/Courses/course';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { error } from 'util';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';


@Component({
    selector: 'app-module-delete',
    templateUrl: './module-delete.component.html',
    styleUrls: ['./module-delete.component.css']
})
export class ModuleDeleteComponent implements OnInit, OnDestroy {

    module_delete: IModule;
    private unsubscribe: Subject<void> = new Subject();
    courseid: string ="";
    constructor(private route: ActivatedRoute,
       // private courseService: CourseService,
        private moduleService: ModuleService,
        private router: Router,
        private cd: ChangeDetectorRef
        ,private messhandler: LoginMessageHandlerService
        ) { }

    ngOnInit() {
        let id = this.route.snapshot.paramMap.get("id");
        this.moduleService.GetModule(id)
        .pipe(takeUntil(this.unsubscribe))
            .subscribe(
                tmodule => {
                    this.module_delete = tmodule;
                    this.cd.markForCheck();
                },
                error => { console.log(error); }
            );
        this.messhandler.Courseid
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(
            courseid =>
            {
                this.courseid=courseid;
                this.cd.markForCheck();
            }
        );
    }

    ConfirmedDelete() {
        this.moduleService.DeleteModule(this.module_delete.id)
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(s => {
                this.cd.markForCheck();
             //   this.router.navigate(['/courses', this.courseid]);     //Todo
            });
    }
    
    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
      }
    
}


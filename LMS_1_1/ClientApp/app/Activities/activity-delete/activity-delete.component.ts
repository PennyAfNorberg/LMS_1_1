import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { IActivity } from 'ClientApp/app/Courses/course';
import { Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { ActivitiesService } from '../activities.service';
import { LoginMessageHandlerService } from 'ClientApp/app/Login/login-message-handler.service';
import { Guid } from 'guid-typescript';

@Component({
    selector: 'app-activity-delete',
    templateUrl: './activity-delete.component.html',
    styleUrls: ['./activity-delete.component.css']
})
export class ActivityDeleteComponent implements OnInit {

    private act_delete: IActivity;
    private unsubscribe: Subject<void> = new Subject();
    private csid: Guid;

    constructor(private route: ActivatedRoute,
        private actvservice: ActivitiesService,
        private router: Router,
        private messhandler: LoginMessageHandlerService,
        private cd: ChangeDetectorRef) { }

    ngOnInit() {
        let id = this.route.snapshot.paramMap.get("id");
        this.actvservice.GetActivity(id)
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(
                tactv => {
                    this.act_delete = tactv;
                    this.messhandler.SendWeek(this.act_delete.startDate);
                    this.cd.markForCheck();
                },
            error => { console.log(error); });
        this.messhandler.Courseid
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(
                sid => {
                    // let tmpguid= Guid.parse(status); 
                    this.csid = Guid.parse(sid);
                   
                    this.cd.markForCheck();
                });
    }

    DelActivity() {

        this.actvservice.DeleteActivity(this.act_delete.id)
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(tact => {
                this.cd.markForCheck();
                this.router.navigate(['/courses/'+this.csid]);
            });

    }

    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }
}
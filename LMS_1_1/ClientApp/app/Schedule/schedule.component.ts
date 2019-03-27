import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Subject } from 'rxjs';
import { Router } from '@angular/router';
import { CourseService } from '../Courses/course.service';
import { takeUntil } from 'rxjs/operators';
import { Scheduleentites } from './Scheduleentites';

@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit, OnDestroy {
  private unsubscribe : Subject<void> = new Subject();
  private week;
  private Type;
  private Weekdays: string[]=["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"];
  private entities:Scheduleentites[];
  constructor(private cd: ChangeDetectorRef , private router: Router
    , private CourseService: CourseService) { }

  ngOnInit() {
     this.CourseService.getCourseById("1")
     .pipe(takeUntil(this.unsubscribe))
     .subscribe(
       status =>
       {
        this.cd.markForCheck();
       });
  }

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }
}

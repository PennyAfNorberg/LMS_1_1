import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CourseListComponent } from './course-list/course-list.component';
import { CourseDetailComponent } from './course-detail/course-detail.component';
import { CreateCourseComponent } from './create-course/create-course.component';
import { RouterModule } from '@angular/router';
import { PartipantListComponent } from '../PartipantList/partipant-list.component';
import { detailList } from './detail_list/detail_list.component';


import { CourseDeleteComponent } from './course-delete/course-delete.component';
import { CourseEditComponent } from './course-edit/course-edit.component';

import { SharedModule } from '../Shared/shared.module';
import { IsAuthenticatedGuard } from '../Shared/is-authenticated.guard';
import { IsTeacherGuard } from '../Shared/is-teacher.guard';
import { AngularFontAwesomeModule } from 'angular-font-awesome';
import { UploadComponent } from '../documents/upload/upload.component';
import { DocumentsModule } from '../documents/documents.module';
import { CloneComponent } from './clone/clone.component';
import { ScheduleModule } from '../Schedule/schedule.module';


@NgModule({
    declarations: [
        CourseListComponent,
        CourseDetailComponent,
        CreateCourseComponent,
        PartipantListComponent,
        CourseDeleteComponent,
        CourseEditComponent,
        detailList,
        CloneComponent,
    ],
  imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      AngularFontAwesomeModule,
      DocumentsModule,
      SharedModule,
      ScheduleModule,
      RouterModule.forChild(
          [{
              path: 'courses' 
              ,canActivate: [IsAuthenticatedGuard]
              ,component: CourseListComponent
          },
              {
                  path: 'courses/create'
                  ,canActivate: [IsTeacherGuard] 
                 , component: CreateCourseComponent
              },
              {

                  path: 'courses/:id'
                  ,canActivate: [IsAuthenticatedGuard]
                  , component: CourseDetailComponent
              },
              {
                  path: 'courses/delete/:id'
                  ,canActivate: [IsTeacherGuard] 
                  , component: CourseDeleteComponent
              },
              {
                  path: 'courses/edit/:id'
                  ,canActivate: [IsTeacherGuard] 
                  , component: CourseEditComponent

              },
              {
                  path: 'courses/clone/:id'
                  ,canActivate: [IsTeacherGuard] 
                  , component: CloneComponent

              }
          ]
      )
  ]
})
export class CoursesModule { }

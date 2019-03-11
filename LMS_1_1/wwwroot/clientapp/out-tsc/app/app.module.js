import * as tslib_1 from "tslib";
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { AddPartipantModule } from './AddPartipant/add-partipant.module';
import { NoRouteModule } from './NoRoute/no-route.module';
import { NoRouteComponent } from './NoRoute/no-route.component';
import { CourseListComponent } from './courses/course-list/course-list.component';
import { HttpClientModule } from '@angular/common/http';
import { CourseDetailComponent } from './courses/course-detail/course-detail.component';
import { CreateCourseComponent } from './courses/create-course/create-course.component';
import { PartipantListComponent } from './PartipantList/partipant-list.component';
var AppModule = /** @class */ (function () {
    function AppModule() {
    }
    AppModule = tslib_1.__decorate([
        NgModule({
            declarations: [
                AppComponent,
                CourseListComponent,
                CourseDetailComponent,
                CreateCourseComponent,
                PartipantListComponent
            ],
            imports: [
                FormsModule,
                BrowserModule,
                AddPartipantModule,
                RouterModule.forRoot([
                    {
                        path: 'courses', component: CourseListComponent
                    },
                    {
                        path: 'courses/create', component: CreateCourseComponent
                    },
                    {
                        path: 'courses/:id', component: CourseDetailComponent
                    },
                    {
                        path: '**', component: NoRouteComponent
                    }
                ], {
                    enableTracing: false // for debug
                }),
                NoRouteModule,
                HttpClientModule,
            ],
            providers: [],
            bootstrap: [AppComponent]
        })
    ], AppModule);
    return AppModule;
}());
export { AppModule };
//# sourceMappingURL=app.module.js.map
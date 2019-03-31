import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddActivitiesWithModulIdComponent } from './Create/add-activities-with-modul-id.component';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { DocumentsModule } from '../documents/documents.module';
import { RouterModule } from '@angular/router';
import { IsTeacherGuard } from '../Shared/is-teacher.guard';
import { ModulesModule } from 'ClientApp/app/Modules/modules.module';
import { EditComponent } from '../Activities/edit/edit.component';
import { ActivityDeleteComponent } from './activity-delete/activity-delete.component';
import { ScheduleModule } from '../Schedule/schedule.module';

@NgModule({
    declarations: [
        AddActivitiesWithModulIdComponent,
        EditComponent,
        ActivityDeleteComponent

    ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        DocumentsModule,
        ModulesModule,
        ScheduleModule,
        RouterModule.forChild(
            [
                {
                    path: 'Activities/create'
                    , canActivate: [IsTeacherGuard]
                    , component: AddActivitiesWithModulIdComponent
                }
                , {
                    path: 'Activities/edit/:id'
                    , canActivate: [IsTeacherGuard]
                    , component: EditComponent
                },
                {
                    path: 'Activities/delete/:id'
                    , canActivate: [IsTeacherGuard]
                    , component: ActivityDeleteComponent
                }

                /*,  {
                     path: 'Modules/delete/:id'
                     , canActivate: [IsTeacherGuard]
                     , component: ModuleDeleteComponent
                 }*/
            ]
        )
    ]
})
export class ActivitiesModule { }

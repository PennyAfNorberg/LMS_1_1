import * as tslib_1 from "tslib";
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './Login/login.component';
import { RouterModule } from '@angular/router';
import { LoginpartialComponent } from './LoginPartial/loginpartial.component';
import { IsTeacherGuard } from '../Shared/is-teacher.guard';
import { RegisterComponent } from './Register/register.component';
import { AddStudentToCourseComponent } from './AddStudentToCourse/add_student_to_course.component';
import { ConfirmRegistedUserComponent } from './ConfirmRegistedUser/confirm-registed-user.component';
import { ManageusersComponent } from './ManageUsers/manageusers.component';
import { IsAuthenticatedGuard } from '../Shared/is-authenticated.guard';
import { EdituserComponent } from './EditUser/edituser.component';
import { DeleteuserComponent } from './DeleteUser/deleteuser.component';
var LoginModule = /** @class */ (function () {
    function LoginModule() {
    }
    LoginModule = tslib_1.__decorate([
        NgModule({
            declarations: [LoginComponent,
                LoginpartialComponent,
                RegisterComponent,
                AddStudentToCourseComponent,
                ConfirmRegistedUserComponent,
                LoginComponent,
                ManageusersComponent
            ],
            imports: [
                CommonModule,
                FormsModule,
                RouterModule.forChild([{
                        path: 'Account/Login',
                        component: LoginComponent
                    },
                    {
                        path: 'Account/Register',
                        canActivate: [IsTeacherGuard],
                        component: RegisterComponent
                    },
                    {
                        path: 'Account/ConfirmRegistedUser',
                        canActivate: [IsTeacherGuard],
                        component: ConfirmRegistedUserComponent
                    },
                    {
                        path: 'Account/ManageUsers',
                        canActivate: [IsTeacherGuard],
                        component: ManageusersComponent
                    },
                    {
                        path: 'Account/Edit',
                        canActivate: [IsAuthenticatedGuard],
                        component: EdituserComponent
                    },
                    {
                        path: 'Account/Edit/:id',
                        canActivate: [IsTeacherGuard],
                        component: EdituserComponent
                    },
                    ,
                    {
                        path: 'Account/Delete/:id',
                        canActivate: [IsTeacherGuard],
                        component: DeleteuserComponent
                    },
                ])
            ],
            exports: [LoginpartialComponent,
                LoginComponent
            ]
        })
    ], LoginModule);
    return LoginModule;
}());
export { LoginModule };
//# sourceMappingURL=login.module.js.map
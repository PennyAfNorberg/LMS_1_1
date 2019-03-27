import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ScheduleComponent } from '../Schedule/schedule.component';

@NgModule({
  declarations: [ScheduleComponent],
  imports: [
    CommonModule,
    FormsModule
  ],
   exports: [
    ScheduleComponent
   ]
})
export class ScheduleModule { }

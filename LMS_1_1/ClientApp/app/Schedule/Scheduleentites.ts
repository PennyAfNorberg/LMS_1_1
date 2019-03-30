export class Scheduleentites
{
    weekday: string="";
    id:string;
    color: string;
    length:number;
    offsettime: number;
    name:string;
    description:string;
    startTime:string;
    endTime:string;
}
export class weekdays
{
    id:number;
    name:string;
}

export class ScheduleFormModel
{
    courseId: string;
    startTime: Date;
    endTime: Date;
}

export class CourseSettingsViewModel
{
   id:string;
   courseId?:string;
   date?:Date;
    startTime:string;
    endTime:string;
    forDate:Date;

}
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
    zindex:number=0;
    width:number=100;
    left:number=0;
}
 export class findKmodel 
 {
     startTime:Date;
     endTime:Date;
     laststart:Date;
     lastend:Date;
}

export class weekdays
{
    id:number;
    name:string;
    acronym: string;
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
    m:number;
    n:number;
}
import { Data } from '@angular/router';
import { Guid } from "guid-typescript";
export interface ICourse {
    id?: Guid;
    name: string;
    startDate: Date;
    description: string;
    courseImgPath?: string;
   modules?:IModule[];
}

export class course implements ICourse
{
    id?: Guid;   
    name: string;
    startDate: Date;
    description: string;
    courseImgPath?: string;
    modules?: IModule[];

    
}

export interface IModule {
    id?: Guid;
    name: string;
    startDate: Date;
    endDate: Date;
    description: string;
    name2?:string;
    isExpanded?:string;
    activities?:IActivity2[];
    courseid?: string;
}

 export class Module implements IModule
 {
    id?: Guid;
    name: string="";
    startDate: Date;
    endDate: Date;
    description: string="";
    name2?:string;
    isExpanded?:string;
    activities?:IActivity2[];
    courseid?: string;
}


export interface IActivity {
    id?: Guid;
    name: string;
    startDate: Date;
    endDate: Date;
    description: string;
    moduleid?: string;
    activityTypeId?:number
  //  activityType: IActivityType
}

export class Activity implements IActivity
{
    id?: Guid;
    name: string;
    startDate: Date;
    endDate: Date;
    description: string;
    moduleid?: string;

    activityTypeId?:number 
}


export interface IActivity2 {
    id?: Guid;
    name: string;
    startDate: Date;
    endDate: Date;
    description: string;
    name2?:string;
    isExpanded?:string;

    activityType: string
}
export interface IActivityType
{
    id: number;
    name: string;
}

 export class CloneCourseModel
 {
    id:Guid;
    name:string="";
    newDate:string=null;
    description:string="";
    newCourseId?:Guid=null;
    courseImgPath: string="";
 }

  export class CloneType
  {
    id:number=0;
    name:string="";

  }
//it is also possible to define course class that implement the Icourse interface
//when needed
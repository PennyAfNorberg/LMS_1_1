import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ScheduleColors, Scheduleentity, Operation } from '../Schedule/Scheduleentites';

@Injectable({
  providedIn: 'root'
})
export class LoginMessageHandlerService {

   public operations:Operation[]=[
    {
      id: 1,
      name: "Create"
    },
    {
      id: 2,
      name: "Edit"
    },
    {
      id: 3,
      name: "Delete"
    },
   ];
  private startstring?:string="";
   // To add And make choose to save
  private useridSource = new BehaviorSubject<string>(this.startstring);
  userid = this.useridSource.asObservable();

  // to 
  private HasChoosedCoursesSource = new BehaviorSubject<boolean>(false);
  HasChoosedCourses = this.HasChoosedCoursesSource.asObservable();

  private IsteacherSource = new BehaviorSubject<boolean>(false);
  Isteacher = this.IsteacherSource.asObservable();

   
  private HasSavedCouresSource = new BehaviorSubject<boolean>(false);
  HasSavedCoures = this.HasSavedCouresSource.asObservable();


  private CourseSavedSource = new BehaviorSubject<string>(this.startstring);
  CourseSaved = this.CourseSavedSource.asObservable();


  private ConfirmSource = new BehaviorSubject<string>(this.startstring);
  ConfirmMessage = this.ConfirmSource.asObservable();

 private CurrUserAuthSource = new BehaviorSubject<boolean>(false);
 CurrUserAuth = this.CurrUserAuthSource.asObservable();

 private CurrUserTeacherSource = new BehaviorSubject<boolean>(false);
 CurrUserTeacher = this.CurrUserTeacherSource.asObservable();

 private ConfirmGoOnUrlSource = new BehaviorSubject<string[]>([]);
 ConfirmGoOnUrl = this.ConfirmGoOnUrlSource.asObservable();

 private ConfirmGoOnMessageSource = new BehaviorSubject<string>(this.startstring);
 ConfirmGoOnMessage = this.ConfirmGoOnMessageSource.asObservable();

 private ConfirmGoBackUrlSource = new BehaviorSubject<string[]>([]);
 ConfirmGoBackUrl = this.ConfirmGoBackUrlSource.asObservable();

 private CourseStartDateSource = new BehaviorSubject<Date>(null);
 CourseStartDate = this.CourseStartDateSource.asObservable();

 private CourseidSource = new BehaviorSubject<string>(null);
 Courseid = this.CourseidSource.asObservable();


 private DubbTypeSource = new BehaviorSubject<string>(null);
 DubbType = this.DubbTypeSource.asObservable();

 private DubbIdSource = new BehaviorSubject<string>(null);
 DubbId = this.DubbIdSource.asObservable();

 private DubbStartSource = new BehaviorSubject<Date>(null);
 DubbStart = this.DubbStartSource.asObservable();

 private DubbEndSource = new BehaviorSubject<Date>(null);
 DubbEnd = this.DubbEndSource.asObservable();

 private ModulidSource = new BehaviorSubject<string>(null);
 Modulid = this.ModulidSource.asObservable();

 private ModulStartDateSource = new BehaviorSubject<Date>(null);
 ModulStartDate = this.ModulStartDateSource.asObservable();

 private ModulEndDateSource = new BehaviorSubject<Date>(null);
 ModulEndDate = this.ModulEndDateSource.asObservable();

 private ModulNameSource = new BehaviorSubject<string>(null);
 ModulName = this.ModulNameSource.asObservable();

 private CourseNameSource = new BehaviorSubject<string>(null);
 CourseName = this.CourseNameSource.asObservable();

 private WeekSource = new BehaviorSubject<Date>(null);
 Week = this.WeekSource.asObservable();

 private ScheduleColorSource = new BehaviorSubject<ScheduleColors>(null);
 ScheduleColor= this.ScheduleColorSource.asObservable();
  
 
 private ChangedEntitySource  = new BehaviorSubject<Scheduleentity>(null);
 ChangedEntity= this.ChangedEntitySource.asObservable();



  constructor() { }

public SendChangeEntity(Scheduleentity:Scheduleentity): boolean
{
  this.ChangedEntitySource.next(Scheduleentity);
  return true;
}

   public SendScheduleColor(ScheduleColor:ScheduleColors): boolean 
  {
      this.ScheduleColorSource.next(ScheduleColor);
      return true;
  }

  public SendWeek(week:Date) : boolean 
  {      
    this.WeekSource.next( week);
    return true;
  }

  public SendUserId(userid:string) : boolean 
  {      
    this.useridSource.next( userid==null?'': userid);
    return true;
  }

  public SendHasChoosedCourses(status:boolean) : boolean 
  {      
    this.HasChoosedCoursesSource.next( status==null?false: status);
    return true;
  }

  public SendIsteacher(status:boolean) : boolean 
  {      
    this.IsteacherSource.next( status==null?false: status);
    return true;
  }

  public SendHasSavedCoures(status:boolean) : boolean 
  {      
    this.HasSavedCouresSource.next( status==null?false: status);
    return true;
  }

  public SendConfirm(arg: string): boolean {
    this.ConfirmSource.next( arg==null?'': arg);
    return true;
  }

  public SendCourseSaved(arg?: string): boolean {
    this.CourseSavedSource.next(arg);
    return true;
  }


  public SendCourseStartDate(arg: Date): boolean {
    this.CourseStartDateSource.next(arg);
    return true;
  }

  public SendCourseid(arg: string): boolean {
    this.CourseidSource.next(arg);
    return true;
  }


  public SendCurrUserAuth(status:boolean) : boolean 
  {      
    this.CurrUserAuthSource.next( status==null?false: status);
    return true;
  }

  public SendCurrUserTeacher(status:boolean) : boolean 
  {      
    this.CurrUserTeacherSource.next( status==null?false: status);
    return true;
  }

  public SendConfirmGoOnUrl(arg:string[]): boolean 
  {
    this.ConfirmGoOnUrlSource.next(arg);
    return true;
  }

  public SendConfirmGoOnMessage(arg:string): boolean 
  {
    this.ConfirmGoOnMessageSource.next(arg);
    return true;
  }

  public SendConfirmGoBackUrl(arg:string[]): boolean 
  {
    this.ConfirmGoBackUrlSource.next(arg);
    return true;
  }

  public SendDubbType(arg:string): boolean
  {
      this.DubbTypeSource.next(arg);
      return true;
  }

  public SendDubbId(arg:string): boolean
  {
      this.DubbIdSource.next(arg);
      return true;
  }

  public SendDubbStart(arg:Date): boolean
  {
      this.DubbStartSource.next(arg);
      return true;
  }
  public SendDubbEnd(arg:Date): boolean
  {
      this.DubbEndSource.next(arg);
      return true;
  }

  public SendModulid(arg:string): boolean
  {
      this.ModulidSource.next(arg);
      return true;
  }

  public SendModulStartDate(arg:Date): boolean
  {
      this.ModulStartDateSource.next(arg);
      return true;
  }

  public SendModulEndDate(arg:Date): boolean
  {
      this.ModulEndDateSource.next(arg);
      return true;
  }

  public SendModulName(arg:string): boolean
  {
      this.ModulNameSource.next(arg);
      return true;
  }

  public SendCourseName(arg:string): boolean
  {
      this.CourseNameSource.next(arg);
      return true;
  }
}

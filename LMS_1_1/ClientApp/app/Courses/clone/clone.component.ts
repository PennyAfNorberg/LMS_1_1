import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { CloneCourseModel, course, CloneType } from '../course';
import { CourseService } from '../course.service';
import { Subject } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { takeUntil } from 'rxjs/operators';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { mimeTypeValidator } from '../mimeType.validator';
import { Guid } from 'guid-typescript';

@Component({
  selector: 'app-clone',
  templateUrl: './clone.component.html',
  styleUrls: ['./clone.component.css']
})
export class CloneComponent implements OnInit, OnDestroy {
  private unsubscribe : Subject<void> = new Subject();
  cloneCourse:CloneCourseModel= new CloneCourseModel();
  Courseid :string="";
  errorMessage: string;
  isSubmitted = false;
  public imagePath;
  imgURL: any;
  @ViewChild("fileInput") fileInputVariable: any;
  showMsg: boolean = false;
  courseForm: FormGroup;
  cloneTypes: CloneType[]=[];
  constructor(private route: ActivatedRoute, private cd: ChangeDetectorRef , private router: Router
   , private CourseService: CourseService, private AuthService: AuthService) { }

  ngOnInit() {
    this.Courseid = this.route.snapshot.paramMap.get("id"); 
    this.courseForm = new FormGroup({
      id:new FormControl(''),
     name: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
     startDate: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(200)]),
     description: new FormControl('', [Validators.required]),
     fileData: new FormControl('', [ mimeTypeValidator()]),
     clonetypeid: new FormControl('1', [Validators.required])

 });
    this.CourseService.GetCloneTypes()
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
      res =>
      {
        this.cloneTypes=res;
        this.cd.markForCheck();
      });
      
    this.CourseService.getCourseById(this.Courseid)
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
       (course: course) =>
       {
        this.cloneCourse.id=course.id;
        this.cloneCourse.name=course.name;
        this.cloneCourse.description=course.description;
        this.cloneCourse.newDate=course.startDate.toLocaleString().substring(0,10)
        this.cloneCourse.courseImgPath=course.courseImgPath;
        this.courseForm.patchValue({id:course.id,
          name:course.name,
          startDate:this.cloneCourse.newDate  ,
          description:course.description
        });
      /*  this.courseForm.setValue({});
        this.courseForm.setValue({});
        this.courseForm.setValue({});
        this.courseForm.setValue({});*/
        this.cd.markForCheck();
       });


  }


  preview(files) {
    if (files.length === 0)
        return;
    var mimeType = files[0].type;
    if (mimeType.match(/image\/*/) == null) {
        return;
    }
    var reader = new FileReader();
    this.imagePath = files;
    reader.readAsDataURL(files[0]);
    reader.onload = (_event) => {
        this.imgURL = reader.result;
    }
}

get formControls() { return this.courseForm.controls; }


register() {

    this.isSubmitted = true;
    if (this.courseForm.invalid) {
        return;
    }
 
    let fileToUpload = this.fileInputVariable.nativeElement.files[0];
    let formData = new FormData();
   /* let MIMEtype = fileToUpload.type.split("/")[0];
    console.log(MIMEtype);*/
    formData.append("ID",this.cloneCourse.id.toString());
    formData.append('Name', this.courseForm.value.name);
    formData.append('newDate', this.courseForm.value.startDate);
    formData.append('Description', this.courseForm.value.description);
    formData.append('clonetypeid', this.courseForm.value.clonetypeid);
    formData.append('FileData', fileToUpload);
    console.log(formData);
    this.CourseService.cloneCourse(formData)
    .pipe(takeUntil(this.unsubscribe))
    .subscribe(
        (result) => {
            this.isSubmitted = false;
            this.cd.markForCheck();
            this.router.navigate(['/courses'])
            
            
        },
        error => this.errorMessage = <any>error
    );
}

  ngOnDestroy(): void {
    this.unsubscribe.next();
    this.unsubscribe.complete();
  }

}

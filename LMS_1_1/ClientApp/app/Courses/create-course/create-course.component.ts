import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ICourse, course } from '../course';
import { ActivatedRoute, Data, Router} from '@angular/router';

import { CourseService } from '../course.service';
import { FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ViewChild } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { mimeTypeValidator } from '../mimeType.validator';

@Component({
  selector: 'app-create-course',
  templateUrl: './create-course.component.html',
  styleUrls: ['./create-course.component.css']
})
export class CreateCourseComponent implements OnInit, OnDestroy  {
  private unsubscribe : Subject<void> = new Subject();
    course: ICourse=new course();
    errorMessage: string;
    courseForm: FormGroup;
    isSubmitted = false;
    public imagePath;
    imgURL: any;
    @ViewChild("fileInput") fileInputVariable: any;
    showMsg: boolean = false;
    constructor(private route: ActivatedRoute, private CourseService: CourseService
      , private router: Router,  private cd: ChangeDetectorRef) { }

    ngOnInit() {
      
        this.courseForm = new FormGroup({
            name: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(50)]),
            startDate: new FormControl('', [Validators.required, Validators.minLength(10), Validators.maxLength(200)]),
            description: new FormControl('', [Validators.required]),
            fileData: new FormControl('', [Validators.required, mimeTypeValidator()])

        });
    }

    preview(files) {
        if (files.length === 0)
            return;
        var mimeType = files[0].type;
        if (mimeType.match(/image\/*/) == null) {
            this.errorMessage="Image file is needed if you upload an image";
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
      let MIMEtype = fileToUpload.type.split("/")[0];
         /* console.log(MIMEtype);*/
         if(MIMEtype=="image")
         {
        formData.append('Name', this.courseForm.value.name);
        formData.append('StartDate', this.courseForm.value.startDate);
        formData.append('Description', this.courseForm.value.description);
        formData.append('FileData', fileToUpload);
        console.log(formData);
        this.CourseService.createCourse(formData)
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(
            (result) => {
                this.isSubmitted = false;
             
                this.router.navigate(['/courses'])
                
                this.cd.markForCheck();
            },
            error => this.errorMessage = <any>error
        );
    }
    else
    {
      this.errorMessage="Image file is needed if you upload an image";
    }
    }
   
    ngOnDestroy(): void {
      this.unsubscribe.next();
      this.unsubscribe.complete();
    }
    
}

import { Component, OnInit, ViewChild, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ICourse, course } from '../course';
import { CourseService } from '../course.service';
import { AuthService } from 'ClientApp/app/auth/auth.service';
import { NgForm } from '@angular/forms';
import { Guid } from 'guid-typescript';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-course-edit',
  templateUrl: './course-edit.component.html',
  styleUrls: ['./course-edit.component.css']
})
export class CourseEditComponent implements OnInit, OnDestroy {
    private unsubscribe : Subject<void> = new Subject();
    editCourse: ICourse=new course();
    errorMessage: string;
    @ViewChild("fileInput") fileInputVariable: any;
    isTeacher: boolean;
    selectFilename: string;
    public imagePath;
    imgURL: any;
    constructor(private route: ActivatedRoute, private router: Router, private cd: ChangeDetectorRef
       , private CourseService: CourseService, private AuthService: AuthService) { }

    ngOnInit() {
        let id = this.route.snapshot.paramMap.get("id");
        this.CourseService.getCourseById(id).subscribe(
            tcourse => {
                let tmppath:string;
                if (tcourse!= null || tcourse.courseImgPath != null   ) {
                    tmppath = "";
                }
                else
                {
                    let tmppath = tcourse.courseImgPath.substr(tcourse.courseImgPath.lastIndexOf('\\') + 1);
                }
                //tcourse.courseImgPath = tcourse.courseImgPath.split('\\')[3];
                this.editCourse = tcourse;
                this.editCourse.courseImgPath = tmppath;
            },
            error => { this.errorMessage = <any>error; });
     //   console.log("XXXXXXXXXXXXXX=>" + this.editCourse.courseImgPath);
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
    

    UpdateCourse() {
        let fileToUpload = (this.fileInputVariable.nativeElement.files.length == 0) ?
            (new File([new Blob()], this.editCourse.courseImgPath)) : this.fileInputVariable.nativeElement.files[0];

        this.selectFilename = this.editCourse.courseImgPath.substr(this.editCourse.courseImgPath.lastIndexOf("\\") + 1);
       // console.log("XXXXXXXXXXXXX====>" + this.editCourse.courseImgPath);
       // console.log("XXXXXXXXXXXXX====>" + this.selectFilename);

        let formData = new FormData();
        let MIMEtype = fileToUpload.type.split("/")[0];    
        if(MIMEtype=="image")
        {
        formData.append('criD', this.editCourse.id.toString());
        formData.append('Name', this.editCourse.name);
        formData.append('StartDate', this.editCourse.startDate.toString());
        formData.append('Description', this.editCourse.description);
        formData.append('FileData', fileToUpload);

        this.CourseService.EditCourse(this.editCourse.id, formData)
        .pipe(takeUntil(this.unsubscribe))
        .subscribe(
            status =>
            {
                this.cd.markForCheck();
                this.router.navigate(['/courses']);
            }


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

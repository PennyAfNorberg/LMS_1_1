import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpEventType, HttpClient } from '@angular/common/http';
import { ActivatedRoute, Data } from '@angular/router';
import { CourseService } from '../course.service';

@Component({
    selector: 'app-upload',
    templateUrl: './upload.component.html',
    styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

    @Output() public onUploadFinished = new EventEmitter();

    constructor(private http: HttpClient) { }

    ngOnInit() {
    }

    
}
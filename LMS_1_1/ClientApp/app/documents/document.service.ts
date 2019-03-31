import { Injectable } from '@angular/core';
import { HttpErrorResponse, HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, tap, takeUntil } from 'rxjs/operators';
import { throwError, Observable, Subject } from 'rxjs';
import { IDocument} from './document';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {

    private subject = new Subject<any>();
    private token: string = "";
    private unsubscribe: Subject<void> = new Subject();

    isUploaded(message: boolean) {
        this.subject.next({ message});
    }

    getUplaodtStatus(): Observable<any> {
        return this.subject.asObservable();
    }

   
   

    private documentUrl = "https://localhost:44396/api/documents1/";

    private getAuthHeader(): HttpHeaders {
        this.AuthService.isAuthenticated;
        return new HttpHeaders({ "Authorization": "Bearer " + this.token });
    }


    private getAuthHeader2() {
        this.AuthService.isAuthenticated;
        return {
            headers: new HttpHeaders({
                'Accept': 'text/html, application/xhtml+xml, */*',
                'Content-Type': 'application/x-www-form-urlencoded',
                "Authorization": "Bearer " + this.token
            }),
            responseType: 'blob' as 'json'
        };
    }



    constructor(private http: HttpClient, private AuthService: AuthService) {
        this.AuthService.token
            .pipe(takeUntil(this.unsubscribe))
            .subscribe(i => this.token = i);


    }

    getDocumentsByOwnerId(id: string): Observable<IDocument[]> {
        console.log(this.documentUrl);
        return this.http.get<IDocument[]>(this.documentUrl + "ByOwner?id=" + id, { headers: this.getAuthHeader() }).pipe(
            tap(data => console.log('All:' + JSON.stringify(data))),
            catchError(this.handleError)
        );
    }
    uploadDocument(document: any) {
        return this.http.post(this.documentUrl, document, { headers: this.getAuthHeader() }).pipe(
            tap(result => JSON.stringify(result)),
            catchError(this.handleError)
        );
    }

    downloadFile(filePath: string): Observable<Blob> {
        let input = filePath;
        return this.http.post<Blob>(this.documentUrl + "DownloadFile?fileName=" + input, {}, this.getAuthHeader2()
            ).pipe
            (
            tap(
                data => 
                    console.log(data)),
                     catchError(this.handleError)
            );
    }

    deleteFileById(id: string) {
        return this.http.delete(this.documentUrl + id, { headers: this.getAuthHeader() }).pipe(
            tap(data => console.log(data)),
            catchError(this.handleError)
        );
    }

    private handleError(err: HttpErrorResponse) {
        // in a real world app, we may send the server to some remote logging infrastructure
        // instead of just logging it to the console
        let errorMessage = '';
        if (err.error instanceof ErrorEvent) {
            // A client-side or network error occurred. Handle it accordingly.
            errorMessage = `An error occurred: ${err.error.message}`;
        } else {
            // The backend returned an unsuccessful response code.
            // The response body may contain clues as to what went wrong,
            errorMessage = `Server returned code: ${err.status}, error message is: ${err.message}`;
        }
        console.error(errorMessage);
        return throwError(errorMessage);
    }

    ngOnDestroy(): void {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }
}

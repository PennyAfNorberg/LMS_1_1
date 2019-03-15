import * as tslib_1 from "tslib";
import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DocumentService } from '../document.service';
import { saveAs } from 'file-saver';
var UploadDetailComponent = /** @class */ (function () {
    function UploadDetailComponent(route, DocumentService) {
        this.route = route;
        this.DocumentService = DocumentService;
        this.documents = [];
    }
    UploadDetailComponent.prototype.ngOnInit = function () {
        this.loadDocument();
    };
    UploadDetailComponent.prototype.loadDocument = function () {
        var _this = this;
        this.DocumentService.getDocumentsByOwnerId(this.DocOwnerId).subscribe(function (documents) {
            _this.documents = documents;
        }, function (error) { return _this.errorMessage = error; });
    };
    UploadDetailComponent.prototype.DownLoadFile = function (fileName) {
        var _this = this;
        this.DocumentService.downloadFile(fileName)
            .subscribe(function (success) {
            saveAs(success, fileName);
        }, function (error) { return _this.errorMessage = error; });
    };
    UploadDetailComponent.prototype.DeleteFile = function (id) {
        var _this = this;
        if (window.confirm('Are you sure, you want to delete?')) {
            this.DocumentService.deleteFileById(id)
                .subscribe(function (success) {
                _this.loadDocument();
            }, function (error) { return _this.errorMessage = error; });
        }
    };
    tslib_1.__decorate([
        Input(),
        tslib_1.__metadata("design:type", String)
    ], UploadDetailComponent.prototype, "DocOwnerId", void 0);
    UploadDetailComponent = tslib_1.__decorate([
        Component({
            selector: 'upload-detail',
            templateUrl: './upload-detail.component.html',
            styleUrls: ['./upload-detail.component.css']
        }),
        tslib_1.__metadata("design:paramtypes", [ActivatedRoute, DocumentService])
    ], UploadDetailComponent);
    return UploadDetailComponent;
}());
export { UploadDetailComponent };
//# sourceMappingURL=upload-detail.component.js.map
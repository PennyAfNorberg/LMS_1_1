import { Data } from '@angular/router';

export interface IDocument
{
    id?: string;
    name: string;
    uploadDate: Data;
    description: string;
    uploaderId: string;
    docOwnerTypeId: number;
    path: string;
}

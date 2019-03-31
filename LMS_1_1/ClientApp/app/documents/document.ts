

export interface IDocument
{
    id?: string;
    name: string;
    uploadDate: Date;
    description: string;
    docOwnerTypeId: number;
    path: string;
}

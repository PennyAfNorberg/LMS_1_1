export interface IRegisterUser
{
    email: string,
    firstName: string,
    lastName: string,
    password: string,
    confirmpassword: string,
    role:string,
    id?: string,
    oldpassword:string
}

export class RegisterUser implements IRegisterUser
{
    email: string="";    
    firstName: string="";
    lastName: string="";
    password: string="";
    confirmpassword: string="";
    role: string="Student";  
    id=null;
    oldpassword="";
}
import { Routes } from '@angular/router';
import { AlbumsComponent } from './albums/albums.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './sign-up/sign-up.component';

export const routes: Routes = [
    { path: '', component: AlbumsComponent },
    { path: 'albums', component: AlbumsComponent },
    { path: 'login', component: LoginComponent },
    { path: 'sign-up', component:SignUpComponent}
];

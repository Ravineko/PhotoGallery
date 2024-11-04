import { Routes } from '@angular/router';
import { AlbumsComponent } from './albums/albums.component';
import { LoginComponent } from './login/login.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { UserAlbumsComponent } from './albums/user-albums/user-albums.component';
import { AlbumPhotosComponent } from './albums/album-photos/album-photos.component';

export const routes: Routes = [
    { path: '', component: AlbumsComponent },
    { path: 'albums', component: AlbumsComponent },
    { path: 'my-albums', component: UserAlbumsComponent },
    { path: 'album-photos/:albumId', component:AlbumPhotosComponent},
    { path: 'login', component: LoginComponent },
    { path: 'sign-up', component:SignUpComponent}
];

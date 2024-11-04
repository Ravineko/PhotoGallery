import { Component } from '@angular/core';
import { Album } from '../../core/models/album.model';
import { AlbumService } from '../../core/services/album.service';
import { environment } from '../../environments/environments';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-albums',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './user-albums.component.html',
  styleUrl: './user-albums.component.css'
})
export class UserAlbumsComponent {
  albums: Album[] = [];

  constructor(private albumService: AlbumService) {}

  ngOnInit(): void {
    this.albumService.getUserAlbums().subscribe((data) => {
      this.albums = data.map(album => ({
        
        ...album,
        coverPath: `${environment.apiUrl}${album.coverPath}`
      }));
    });
  }
}

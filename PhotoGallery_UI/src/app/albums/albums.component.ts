import { Component, OnInit } from '@angular/core';
import { Album } from '../core/models/album.model';
import { AlbumService } from '../core/services/album.service';

@Component({
  selector: 'app-albums',
  standalone: true,
  imports: [],
  templateUrl: './albums.component.html',
  styleUrl: './albums.component.css'
})
export class AlbumsComponent implements OnInit{
  albums: Album[] = [];

  constructor(private albumService: AlbumService) {}

  ngOnInit(): void {
    this.albumService.getAlbums().subscribe((data) => {
      this.albums = data;
    });
  }
}

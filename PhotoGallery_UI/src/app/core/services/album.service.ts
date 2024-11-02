import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Album } from '../models/album.model';

@Injectable({
  providedIn: 'root',
})
export class AlbumService {
  private apiUrl = 'https://api.example.com/albums'; 

  constructor(private http: HttpClient) {}

  getAlbums(): Observable<Album[]> {
    return this.http.get<Album[]>(this.apiUrl);
  }
}

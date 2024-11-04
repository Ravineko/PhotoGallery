import { Injectable } from '@angular/core';
import { Photo } from '../models/photo.model';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environments';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class PhotoService {
  private apiUrl = environment.apiUrl + '/api/photos';

  constructor(private http: HttpClient) {}

  uploadPhoto(formData: FormData): Observable<any> {
    return this.http.post(`${this.apiUrl}/upload`, formData);
  }
}

import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlbumService } from '../../core/services/album.service';
import { Photo } from '../../core/models/photo.model';
import { environment } from '../../environments/environments';
import { PhotoService } from '../../core/services/photo.service';

@Component({
  selector: 'app-album-photos',
  standalone: true,
  imports: [],
  templateUrl: './album-photos.component.html',
  styleUrl: './album-photos.component.css',
})
export class AlbumPhotosComponent implements OnInit {
  photos: Photo[] = [];
  albumId!: number;
  isModalOpen = false;
  isUploadModalOpen = false;
  selectedPhotoUrl?: string;
  selectedFile?: File;

  constructor(
    private route: ActivatedRoute,
    private albumService: AlbumService,
    private photoService: PhotoService
  ) {}

  ngOnInit(): void {
    this.albumId = Number(this.route.snapshot.paramMap.get('albumId'));
    this.loadPhotos();
  }

  loadPhotos(): void {
    this.albumService.getPhotosByAlbumId(this.albumId).subscribe((data) => {
      this.photos = data.map((photo) => ({
        ...photo,
        filePath: `${environment.apiUrl}${photo.filePath}`,
      }));
    });
  }

  openModal(photoUrl: string): void {
    this.selectedPhotoUrl = photoUrl;
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedPhotoUrl = undefined;
  }

  openUploadModal(): void {
    this.isUploadModalOpen = true; 
  }

  closeUploadModal(): void {
    this.isUploadModalOpen = false; 
    this.selectedFile = undefined;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }

  uploadPhoto(): void {
    if (this.selectedFile && this.albumId) {
      const formData = new FormData();
      formData.append('file', this.selectedFile);
      formData.append('albumId', this.albumId.toString());

      this.photoService.uploadPhoto(formData).subscribe(() => {
        this.loadPhotos(); 
        this.closeUploadModal();
      });
    }
  }
}

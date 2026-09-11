import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { PhotoForApprove } from '../../../types/user';

@Component({
  selector: 'app-photo-management',
  imports: [],
  templateUrl: './photo-management.html',
  styleUrl: './photo-management.css',
})
export class PhotoManagement implements OnInit {
  private adminService = inject(AdminService);
  protected photos = signal<PhotoForApprove[]>([]);

  ngOnInit(): void {
    this.loadPhotos();
  }

  loadPhotos() {
    this.adminService.getPhotosForApprove().subscribe({
      next: (res) => this.photos.set(res)
    });
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(list => list.filter(p => p.id !== photoId));
      }
    });
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => {
        this.photos.update(list => list.filter(p => p.id !== photoId));
      }
    });
  }
}

import { Component, computed, inject, signal } from '@angular/core';
import { UserService } from '../../../core/services/user-service';
import { AccountService } from '../../../core/services/account-service';
import { Photo } from '../../../types/user';

@Component({
  selector: 'app-member-photo',
  imports: [],
  templateUrl: './member-photo.html',
  styleUrl: './member-photo.css',
})
export class MemberPhoto {
  protected userService = inject(UserService);
  protected accountService = inject(AccountService);

  protected isDragging = signal(false);
  protected selectedFile = signal<File | null>(null);
  protected previewUrl = signal<string | null>(null);

  protected isCurrentUser = computed(() => {
    return this.accountService.currentUser()?.id === this.userService.user()?.id;
  });

  protected displayedPhotos = computed(() => {
    const photos = this.userService.user()?.photos || [];
    if (this.isCurrentUser()) return photos;
    return photos.filter(p => p.isApproved);
  });

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(true);
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.handleFile(event.dataTransfer.files[0]);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleFile(input.files[0]);
    }
  }

  cancelUpload() {
    this.selectedFile.set(null);
    this.previewUrl.set(null);
  }

  uploadPhoto() {
    const file = this.selectedFile();
    if (!file) return;

    this.userService.uploadPhoto(file).subscribe({
      next: (newPhoto) => {
        const current = this.userService.user();
        if (current) {
          this.userService.user.set({
            ...current,
            photos: [...current.photos, newPhoto]
          });
        }
        this.cancelUpload();
      }
    });
  }

  setMain(photo: Photo) {
    if (!photo.isApproved || photo.isMain) return;

    this.userService.setMainPhoto(photo.id).subscribe({
      next: () => {
        const current = this.userService.user();
        const currentUser = this.accountService.currentUser();

        if (current) {
          const updatedPhotos = current.photos.map(p => ({
            ...p,
            isMain: p.id === photo.id
          }));
          this.userService.user.set({ ...current, photos: updatedPhotos });
        }

        if (currentUser) {
          this.accountService.currentUser.set({
            ...currentUser,
            imageUrl: photo.url
          });
        }
      }
    });
  }

  delete(photoId: number) {
    const confirmed = window.confirm('Bạn có chắc chắn muốn xóa bức ảnh này không?');
    if (!confirmed) return;
  
    this.userService.deletePhoto(photoId).subscribe({
      next: () => {
        const current = this.userService.user();
        if (current) {
          this.userService.user.set({
            ...current,
            photos: current.photos.filter(p => p.id !== photoId)
          });
        }
      }
    });
  }

  private handleFile(file: File) {
    if (!file.type.startsWith('image/')) return;
    this.selectedFile.set(file);

    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }
}

import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PhotoForApprove, UserRole } from '../../types/user';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getUserWithRoles() {
    return this.http.get<UserRole[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(userId: number, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + userId + '?roles=' + roles, {});
  }

  getPhotosForApprove() {
    return this.http.get<PhotoForApprove[]>(this.baseUrl + 'admin/photos-for-approve');
  }

  approvePhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'admin/approve-photo/' + photoId, {});
  }

  rejectPhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'admin/reject-photo/' + photoId);
  }
}

import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { tap } from 'rxjs';
import { Photo, UserCard, UserProfile, UserUpdate } from '../../types/user';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  users = signal<UserCard[]>([]);
  user = signal<UserProfile | null>(null);
  editMode = signal<boolean>(false);

  getMembers()  {
    return this.http.get<UserCard[]>(this.baseUrl + 'user').pipe(
      tap((users) => {
        this.users.set(users);
      })
    );
  }

  getMember(id: number) {
    return this.http.get<UserProfile>(this.baseUrl + 'user/' + id).pipe(
      tap(user => this.user.set(user))
    );
  }

  updateMember(user: UserUpdate) {
    return this.http.put(this.baseUrl + 'user', user);
  }

  getMemberPhotos(id: string) {
    return this.http.get<Photo[]>(this.baseUrl + 'members/' + id + '/photos')
  }

  uploadPhoto(file: File) {
    const formData = new FormData;
    formData.append('file', file);
    return this.http.post<Photo>(this.baseUrl + 'photo', formData);
  }

  setMainPhoto(photoId: Number) {
    return this.http.put(this.baseUrl + 'photo/set-main/' + photoId, {});
  }

  deletePhoto(photoId: Number) {
    return this.http.delete(this.baseUrl + 'photo/' + photoId);
  }
}

import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { tap } from 'rxjs';
import { Photo, UserCard, UserParams, UserProfile, UserUpdate } from '../../types/user';
import { PaginatedResult } from '../../types/pagination';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl;

  users = signal<UserCard[]>([]);
  user = signal<UserProfile | null>(null);
  editMode = signal<boolean>(false);

  getMembers(userParams: UserParams)  {
    let params = new HttpParams()
      .set('pageNumber', userParams.pageNumber)
      .set('pageSize', userParams.pageSize)
      .set('orderBy', userParams.orderBy)
      .set('minAge', userParams.minAge)
      .set('maxAge', userParams.maxAge);

    if (userParams.search?.trim()) params = params.set('search', userParams.search.trim());
    if (userParams.gender) params = params.set('gender', userParams.gender);
    if (userParams.lookingFor) params = params.set('lookingFor', userParams.lookingFor);
    if (userParams.city?.trim()) params = params.set('city', userParams.city.trim());
    if (userParams.mbti) params = params.set('mbti', userParams.mbti);
    if (userParams.educationLevel) params = params.set('educationLevel', userParams.educationLevel);

    return this.http.get<PaginatedResult<UserCard>>(this.baseUrl + 'user', {params})};

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

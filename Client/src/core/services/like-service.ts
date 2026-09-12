import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../../types/pagination';
import { UserLike } from '../../types/like';

@Injectable({
  providedIn: 'root',
})
export class LikeService {
  private baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  likeIds = signal<number[]>([]);

  getLikes(type: string, pageNumber: number = 1, pageSize: number = 10) {
    return this.http.get<PaginatedResult<UserLike>>(
      this.baseUrl + 'like?type=' + type + '&pageNumber=' + pageNumber + '&pageSize=' + pageSize
    );
  }

  getLikeIds() {
    return this.http.get<number[]>(this.baseUrl + 'like/list').subscribe({
      next: ids => this.likeIds.set(ids)
    });
  }

  toggleLike(targetMemberId: number) {
    return this.http.post(this.baseUrl + 'like/' + targetMemberId, {}).subscribe({
      next: () => {
        if (this.likeIds().includes(targetMemberId)) {
          this.likeIds.update(ids => ids.filter(x => x !== targetMemberId));
        } else {
          this.likeIds.update(ids => [...ids, targetMemberId]);
        }
      }
    });
  }

  clearLikeIds() {
    this.likeIds.set([]);
  }
}

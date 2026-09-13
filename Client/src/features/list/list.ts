import { Component, inject, signal } from '@angular/core';
import { LikeService } from '../../core/services/like-service';
import { UserLike } from '../../types/like';
import { MemberList } from '../member/member-list/member-list';
import { RouterLink } from '@angular/router';
import { AgePipe } from '../../core/pipes/age-pipe';
import { PresenceService } from '../../core/services/presence-service';

@Component({
  selector: 'app-list',
  imports: [RouterLink, AgePipe],
  templateUrl: './list.html',
  styleUrl: './list.css',
})
export class List {
  protected likeService = inject(LikeService);
  protected presenceService = inject(PresenceService);
  protected members = signal<UserLike[]>([]);
  protected type = 'mutual';

  tabs = [
    { label: 'Tương hợp', value: 'mutual' },
    { label: 'Đã thích', value: 'liked' },
    { label: 'Thích bạn', value: 'likedBy' }
  ];

  ngOnInit(): void {
    this.loadLikes();
  }

  setType(type: string) {
    if (this.type !== type) {
      this.type = type;
      this.loadLikes();
    }
  }

  loadLikes() {
    this.likeService.getLikes(this.type).subscribe({
      next: res => this.members.set(res.items)
    });
  }

  onLike(event: Event, targetId: number) {
    event.stopPropagation();
    this.likeService.toggleLike(targetId);
  }
}

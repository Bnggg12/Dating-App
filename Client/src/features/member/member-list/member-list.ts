import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user-service';
import { UserCard, UserParams } from '../../../types/user';
import { RouterLink } from '@angular/router';
import { AgePipe } from "../../../core/pipes/age-pipe";
import { LikeService } from '../../../core/services/like-service';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { PaginatedResult } from '../../../types/pagination';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-list',
  imports: [FormsModule, RouterLink, AgePipe],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  private accountService = inject(AccountService);
  private memberService = inject(UserService);
  protected likeService = inject(LikeService);
  protected presenceService = inject(PresenceService);

  protected users = signal<UserCard[]>([]);
  protected userParams = new UserParams();
  protected pagination = signal<PaginatedResult<UserCard> | null>(null);

  protected pages = signal<number[]>([]);
  protected cities = signal<string[]>([]);
  protected mbtis = signal<string[]>([]);
  protected eduLevels = signal<string[]>([]);

  protected genderList = [
    { value: 'male', display: 'Nam' },
    { value: 'female', display: 'Nữ' },
  ];

  protected lookingForList = [
    { value: 'male', display: 'Nam' },
    { value: 'female', display: 'Nữ' },
    { value: 'both', display: 'Cả hai' },
  ];

  ngOnInit(): void {
    this.loadMembers();
    this.loadFilter();
  }

  loadFilter() {
    this.accountService.getCities().subscribe({ next: (res) => this.cities.set(res) });
    this.accountService.getMbtis().subscribe({ next: (res) => this.mbtis.set(res) });
    this.accountService.getEduLevels().subscribe({ next: (res) => this.eduLevels.set(res) });
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe({
      next: (res) => {
        const items = res.items || [];
        this.users.set(items);
        this.pagination.set(res);
        const total = res.totalPages ?? res.totalPages ?? 0;
        this.pages.set(Array.from({ length: total }, (_, i) => i + 1));
      },
    });
  }

  resetFilters() {
    this.userParams = new UserParams();
    this.loadMembers();
  }

  pageChanged(page: number) {
    if (this.userParams.pageNumber !== page) {
      this.userParams.pageNumber = page;
      this.loadMembers();
    }
  }

  onLike(event: Event, userId: number) {
    event.stopPropagation();
    this.likeService.toggleLike(userId);
  }
}

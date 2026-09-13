import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AgePipe } from '../../../core/pipes/age-pipe';
import { UserService } from '../../../core/services/user-service';
import { AccountService } from '../../../core/services/account-service';
import { filter } from 'rxjs';
import { LikeService } from '../../../core/services/like-service';
import { TimeAgoPipe } from '../../../core/pipes/time-ago-pipe';
import { PresenceService } from '../../../core/services/presence-service';

@Component({
  selector: 'app-member-detail',
  imports: [RouterLink, RouterLinkActive, RouterOutlet, AgePipe, TimeAgoPipe],
  templateUrl: './member-detail.html',
  styleUrl: './member-detail.css',
})
export class MemberDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  protected accountService = inject(AccountService);
  protected userService = inject(UserService);
  protected likeService = inject(LikeService);
  protected presenceService = inject(PresenceService);

  protected title = signal<string | undefined>('Thông tin cá nhân');
  private routeId = signal<number | null>(null);

  protected mainImageUrl = computed(() => {
    const photos = this.userService.user()?.photos;
    return photos?.find(p => p.isMain && p.isApproved)?.url || '/user.png';
  });

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (id) {
        this.routeId.set(id);
        this.userService.getMember(id).subscribe();
      }
    });

    this.title.set(this.route.firstChild?.snapshot?.title);

    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe({
      next: () => {
        this.title.set(this.route.firstChild?.snapshot?.title);
      }
    });
  }
}

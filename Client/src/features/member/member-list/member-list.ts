import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user-service';
import { UserCard } from '../../../types/user';
import { RouterLink } from '@angular/router';
import { AgePipe } from "../../../core/pipes/age-pipe";

@Component({
  selector: 'app-member-list',
  imports: [RouterLink, AgePipe],
  templateUrl: './member-list.html',
  styleUrl: './member-list.css',
})
export class MemberList implements OnInit {
  private memberService = inject(UserService);
  protected users = signal<UserCard[]>([]);

  ngOnInit(): void {
    this.loadMembers();
  }

  loadMembers() {
    this.memberService.getMembers().subscribe({
      next: (res) => {
        this.users.set(res);
      }
    });
  }
}

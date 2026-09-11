import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { AccountService } from '../../core/services/account-service';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { themes } from '../theme';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../core/services/user-service';
import { HasRole } from '../../shared.directives/has-role';

@Component({
  selector: 'app-nav',
  imports: [RouterLinkActive, RouterLink, FormsModule, HasRole],
  templateUrl: './nav.html',
  styleUrl: './nav.css',
})
export class Nav implements OnInit {
  protected accountService = inject(AccountService);
  protected userService = inject(UserService);
  private router = inject(Router);
  protected creds: any = {}
  protected selectedTheme = signal<string>(localStorage.getItem('theme') || 'light');
  protected themes = themes;

  ngOnInit(): void {
    document.documentElement.setAttribute('data-theme', this.selectedTheme());
  }

  handleSelectTheme(theme: string) {
    this.selectedTheme.set(theme);
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }

  handleSelectUserItem() {
    const elem = document.activeElement as HTMLDivElement;
    if (elem) elem.blur();
  }

  protected mainImageUrl = computed(() => {
    return this.accountService.currentUser()?.imageUrl || '/user.png';
  });

  login() {
    this.accountService.login(this.creds).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.creds = {};
      }
    })
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }
}

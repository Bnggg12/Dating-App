import { DatePipe } from '@angular/common';
import { Component, computed, effect, HostListener, inject, OnDestroy, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../../core/services/account-service';
import { UserService } from '../../../core/services/user-service';
import { UserProfile, UserUpdate } from '../../../types/user';

@Component({
  selector: 'app-member-profile',
  imports: [DatePipe, FormsModule],
  templateUrl: './member-profile.html',
  styleUrl: './member-profile.css',
})
export class MemberProfile implements OnInit, OnDestroy {
  @ViewChild('editForm') editForm?: NgForm;

  @HostListener('window:beforeunload', ['$event']) notify($event: BeforeUnloadEvent) {
    if (this.editForm?.dirty) {
      $event.preventDefault();
    }
  }

  private accountService = inject(AccountService);
  protected userService = inject(UserService);

  protected cities = signal<string[]>([]);
  protected mbtis = signal<string[]>([]);
  protected eduLevels = signal<string[]>([]);
  protected interestsInput = '';

  protected editableMember: UserUpdate = {
    displayName: '',
    description: '',
    city: '',
    lookingFor: '',
    mbti: '',
    educationLevel: '',
    fieldOfStudy: '',
    institution: '',
    interests: []
  };

  constructor() {
    effect(() => {
      const member = this.userService.user();
      if (member) {
        this.editableMember = {
          displayName: member.displayName || '',
          description: member.description || '',
          city: member.city || '',
          lookingFor: member.lookingFor || '',
          mbti: member.mbti || null,
          educationLevel: member.educationLevel || null,
          fieldOfStudy: member.fieldOfStudy || null,
          institution: member.institution || null,
          interests: [...(member.interests || [])]
        };
        this.interestsInput = (member.interests || []).join(', ');
      }
    });
  }

  ngOnInit(): void {
    this.accountService.getCities().subscribe(data => this.cities.set(data));
    this.accountService.getMbtis().subscribe(data => this.mbtis.set(data));
    this.accountService.getEduLevels().subscribe(data => this.eduLevels.set(data));
  }

  protected isCurrentUser = computed(() => {
    return this.accountService.currentUser()?.id === this.userService.user()?.id;
  });

  updateProfile() {
    const currentMember = this.userService.user();
    if (!currentMember) return;

    const payload: UserUpdate = {
      ...this.editableMember,
      interests: this.interestsInput
        ? this.interestsInput.split(',').map(s => s.trim()).filter(s => s.length > 0)
        : []
    };

    this.userService.updateMember(payload).subscribe({
      next: () => {
        const currentUser = this.accountService.currentUser();
        if (currentUser && payload.displayName !== currentUser.displayName) {
          this.accountService.currentUser.set({
            ...currentUser,
            displayName: payload.displayName
          });
        }

        const updatedProfile: UserProfile = {
          ...currentMember,
          ...payload
        };

        this.userService.user.set(updatedProfile);
        this.userService.editMode.set(false);
        this.editForm?.reset(payload);
      }
    });
  }

  ngOnDestroy(): void {
    if (this.userService.editMode()) {
      this.userService.editMode.set(false);
    }
  }
}

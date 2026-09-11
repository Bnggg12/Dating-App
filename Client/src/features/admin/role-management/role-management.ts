import { Component, inject, OnInit, signal } from '@angular/core';
import { AdminService } from '../../../core/services/admin-service';
import { UserRole } from '../../../types/user';

@Component({
  selector: 'app-role-management',
  imports: [],
  templateUrl: './role-management.html',
  styleUrl: './role-management.css',
})
export class RoleManagement implements OnInit {
  private adminService = inject(AdminService);
  protected users = signal<UserRole[]>([]);
  protected availableRoles = ['Admin', 'Moderator', 'Member'];
  
  protected selectedUser = signal<UserRole | null>(null);
  protected selectedRoles = signal<string[]>([]);
  protected isModalOpen = signal(false);

  ngOnInit(): void {
    this.loadUsersWithRoles();
  }

  loadUsersWithRoles() {
    this.adminService.getUserWithRoles().subscribe({
      next: (res) => this.users.set(res)
    });
  }

  openModal(user: UserRole) {
    this.selectedUser.set(user);
    this.selectedRoles.set([...user.roles]);
    this.isModalOpen.set(true);
  }

  closeModal() {
    this.isModalOpen.set(false);
    this.selectedUser.set(null);
  }

  toggleRole(role: string) {
    const roles = this.selectedRoles();
    if (roles.includes(role)) {
      this.selectedRoles.set(roles.filter(r => r !== role));
    } else {
      this.selectedRoles.set([...roles, role]);
    }
  }

  saveRoles() {
    const user = this.selectedUser();
    if (!user) return;

    this.adminService.updateUserRoles(user.userId, this.selectedRoles()).subscribe({
      next: (updatedRoles) => {
        this.users.update(users => users.map(u => {
          if (u.userId === user.userId) {
            return { ...u, roles: updatedRoles };
          }
          return u;
        }));
        this.closeModal();
      }
    });
  }
}

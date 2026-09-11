import { Component, inject } from '@angular/core';
import { RoleManagement } from './role-management/role-management';
import { PhotoManagement } from './photo-management/photo-management';
import { AccountService } from '../../core/services/account-service';

@Component({
  selector: 'app-admin',
  imports: [RoleManagement, PhotoManagement],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
})
export class Admin {
  protected accountService = inject(AccountService);
  activeTab = 'photos';
  tabs = [
    {label: 'Duyệt ảnh', value: 'photos'},
    {label: 'Quản lý vai trò', value: 'roles'},
  ]

  setTab(tab: string) {
    this.activeTab = tab;
  }
}

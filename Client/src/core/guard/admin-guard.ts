import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';
import { inject } from '@angular/core';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);

  const user = accountService.currentUser();
  const roles = user?.roles || [];

  if (roles.includes('Admin') || roles.includes('Moderator')) {
    return true;
  }

  alert('Bạn không có quyền truy cập khu vực này');
  return false;
};

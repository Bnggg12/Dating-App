import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../services/account-service';

export const authGuard: CanActivateFn = (route, state) => {
  const accoutService = inject(AccountService);

  if (accoutService.currentUser()) return true;
  else {
    alert('Yêu cầu đăng nhập');
    return false;
  }
};

import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { AuthService } from './shared/services/auth-service.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserRole } from './shared/types';

@Injectable({
  providedIn: 'root',
})
export class authorizedUserGuard {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ):
    | boolean
    | UrlTree
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree> => {
    const authData = this.authService.getCurrentAuthData();
    if (
      authData &&
      (authData.role === UserRole.User || authData.role === UserRole.Admin)
    ) {
      return true;
    } else {
      this.router.navigate(['/']);
      return false;
    }
  };
}

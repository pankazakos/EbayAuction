import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../services/authentication/auth-service.service';
import { Subscription } from 'rxjs';
import { AuthData } from '../../services/authentication/auth-service.service';
import { UserRole } from '../../types';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavBarComponent implements OnInit, OnDestroy {
  private subscription: Subscription | null = null;
  authData: AuthData | null = null;
  adminRole = UserRole.Admin;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.subscription = this.authService.authData$.subscribe(
      (authData) => (this.authData = authData)
    );
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  callLogoutUser() {
    this.authService.logoutUser();
  }
}

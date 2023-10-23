import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../auth-service.service';
import { Subscription } from 'rxjs';
import { AuthData } from '../auth-service.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavBarComponent implements OnInit, OnDestroy {
  private subscription: Subscription | null = null;
  authData: AuthData | null = null;

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
}

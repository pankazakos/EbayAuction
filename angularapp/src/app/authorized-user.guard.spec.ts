import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { authorizedUserGuard } from './authorized-user.guard';

describe('authorizedUserGuard', () => {
  const executeGuard: CanActivateFn = (...guardParameters) => 
      TestBed.runInInjectionContext(() => authorizedUserGuard(...guardParameters));

  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });
});

import { TestBed } from '@angular/core/testing';

import { BidStepService } from './bid-step.service';

describe('BidStepService', () => {
  let service: BidStepService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BidStepService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

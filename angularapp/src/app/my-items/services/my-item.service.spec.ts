import { TestBed } from '@angular/core/testing';

import { MyItemService } from './my-item.service';

describe('MyItemService', () => {
  let service: MyItemService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MyItemService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

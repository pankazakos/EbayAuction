import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observer } from 'rxjs';
import { BasicItemResponse } from 'src/app/shared/contracts/responses/item';
import { DateTimeFormatService } from 'src/app/shared/date-time-format.service';
import { baseUrl } from 'src/app/shared/types';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.css'],
})
export class ItemComponent {
  item: BasicItemResponse;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private formatter: DateTimeFormatService
  ) {
    this.item = {} as BasicItemResponse;
  }

  ngOnInit(): void {
    const itemId = this.route.snapshot.params['id'];

    this.http.get(`${baseUrl}api/item/${itemId}`).subscribe({
      next: (response: BasicItemResponse) => {
        this.item = response;
        this.item.started = this.item.started
          ? this.formatter.formatDatetime(this.item.started.toString())
          : '';
        this.item.ends = this.item.ends
          ? this.formatter.formatDatetime(this.item.ends.toString())
          : '';
        console.log(this.item);
      },
      error: (error) => {
        console.error(error);
      },
    } as Partial<Observer<any>>);
  }
}

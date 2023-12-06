import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ItemEndpoints } from 'src/app/shared/contracts/endpoints/ItemEndpoints';
import { UserEndpoints } from 'src/app/shared/contracts/endpoints/UserEndpoints';
import { BasicItemResponse } from 'src/app/shared/contracts/responses/item';
import { IdToUsernameResponse } from 'src/app/shared/contracts/responses/user';
import { DateTimeFormatService } from 'src/app/shared/date-time-format.service';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
})
export class ItemComponent {
  itemId: number = -1;
  item: BasicItemResponse = {} as BasicItemResponse;
  username: IdToUsernameResponse = {} as IdToUsernameResponse;
  image: { src: string; isLoading: boolean } = {} as {
    src: string;
    isLoading: boolean;
  };

  constructor(
    private http: HttpClient,
    private formatter: DateTimeFormatService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.itemId = data.itemId;
    this.image = data.image;
  }

  ngOnInit(): void {
    this.http.get(ItemEndpoints.getById(this.itemId)).subscribe({
      next: (response: BasicItemResponse | any) => {
        this.item = response;
        this.item.started = this.item.started
          ? this.formatter.formatDatetime(this.item.started.toString())
          : '';
        this.item.ends = this.item.ends
          ? this.formatter.formatDatetime(this.item.ends.toString())
          : '';

        console.log(this.item);

        this.http
          .get(UserEndpoints.IdToUsername(this.item.sellerId))
          .subscribe({
            next: (response: IdToUsernameResponse | any) => {
              this.username = response.username;
            },
            error: (error) => {
              console.error(error);
            },
          });
      },
      error: (error) => {
        console.error(error);
      },
    });
  }
}

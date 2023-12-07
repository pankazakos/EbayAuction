import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { UserEndpoints } from 'src/app/shared/contracts/endpoints/UserEndpoints';
import { BasicItemResponse } from 'src/app/shared/contracts/responses/item';
import { IdToUsernameResponse } from 'src/app/shared/contracts/responses/user';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DateTimeFormatService } from 'src/app/shared/date-time-format.service';

type loadingItem = { data: BasicItemResponse; isLoading: boolean };
type loadingImage = { src: string; isLoading: boolean };

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss'],
})
export class ItemComponent {
  sellerUsername: IdToUsernameResponse = {} as IdToUsernameResponse;
  item: loadingItem = {} as loadingItem;
  image: loadingImage = {} as loadingImage;

  constructor(
    private http: HttpClient,
    private formatter: DateTimeFormatService,
    private dialogRef: MatDialogRef<ItemComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.item.data = data.item;
    this.image = data.image;
  }

  ngOnInit(): void {
    this.item.data.started = this.item.data.started
      ? this.formatter.formatDatetime(this.item.data.started.toString())
      : '';
    this.item.data.ends = this.item.data.ends
      ? this.formatter.formatDatetime(this.item.data.ends.toString())
      : '';

    this.http
      .get(UserEndpoints.IdToUsername(this.item.data.sellerId))
      .subscribe({
        next: (response: IdToUsernameResponse | any) => {
          this.sellerUsername = response.username;
        },
        error: (error) => {
          console.error(error);
        },
      });
  }

  closeDialog(): void {
    this.dialogRef.close();
  }
}

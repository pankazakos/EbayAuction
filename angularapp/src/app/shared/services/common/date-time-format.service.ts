import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DateTimeFormatService {
  // Format datetime values
  formatDatetime(input: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: '2-digit',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      hour12: false,
    };

    let parsedDatetime: Date;

    parsedDatetime = new Date(input);

    return parsedDatetime.toLocaleDateString('en-US', options);
  }

  formatDatetimeUpToSeconds(input: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: '2-digit',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: false,
    };

    let parsedDatetime: Date;

    parsedDatetime = new Date(input);

    return parsedDatetime.toLocaleDateString('en-US', options);
  }

  convertOnlyToDate(input: string): string {
    const options: Intl.DateTimeFormatOptions = {
      year: '2-digit',
      month: '2-digit',
      day: '2-digit',
    };

    let parsedDatetime: Date;

    parsedDatetime = new Date(input);

    return parsedDatetime.toLocaleDateString('en-US', options);
  }

  isExpired(dateString: string): boolean {
    const now = new Date();
    const endDate = new Date(dateString);

    return now > endDate;
  }
}

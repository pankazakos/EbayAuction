import { HttpClient } from '@angular/common/http';
import { Component, AfterViewInit, ViewChild } from '@angular/core';
import {
  BasicItemResponse,
  ExtendedItemInfo,
} from '../shared/contracts/responses/item';
import { PaginatedResponse } from '../shared/contracts/responses/PaginatedResponse';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ItemEndpoints } from '../shared/contracts/endpoints/ItemEndpoints';
import { environment } from 'src/environments/environment';
import { MatDialog } from '@angular/material/dialog';
import { FiltersDialogComponent } from './filters-dialog/filters-dialog.component';
import { ItemComponent } from '../shared/components/item/item.component';
import { DateTimeFormatService } from '../shared/services/date-time-format.service';
import { AuthData, AuthService } from '../shared/services/auth-service.service';
import { AlertService } from '../shared/services/alert.service';
import { CategoryService } from '../shared/services/category.service';
import { FilterService } from './services/filter.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements AfterViewInit {
  public items: PaginatedResponse<ExtendedItemInfo> = {
    castEntities: [],
    total: 0,
    page: 1,
    limit: 10,
  };
  isLoading: boolean = true;
  title: string = '';
  minPrice: number = 0;
  maxPrice: number = 0;
  categoryQuery: string = '';

  authData: AuthData | null = null;

  showClearFiltersButton: boolean = false;

  @ViewChild('paginatorTop') paginatorTop?: MatPaginator;
  @ViewChild('paginatorBottom') paginatorBottom?: MatPaginator;

  constructor(
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router,
    private filtersDialog: MatDialog,
    private itemDialog: MatDialog,
    private alertService: AlertService,
    private formatter: DateTimeFormatService,
    private authService: AuthService,
    private categoryService: CategoryService,
    private filterService: FilterService
  ) {}

  ngOnInit(): void {
    this.authService.authData$.subscribe(
      (authData) => (this.authData = authData)
    );

    this.route.queryParamMap.subscribe((queryParams: ParamMap) => {
      this.isLoading = true;

      // Reset to default values
      this.items.page = 1;
      this.title = '';
      this.minPrice = 0;
      this.maxPrice = 0;
      this.categoryQuery = '';

      // Set the new values if they exist
      this.setQueryParameter('page', queryParams, (value) => {
        this.items.page = Number(value);
      });

      if (this.items.page == 1) {
        this.removeUrlParameters(['page']);
      }

      this.setQueryParameter('title', queryParams, (value) => {
        this.title = value;
      });
      this.setQueryParameter('minPrice', queryParams, (value) => {
        this.minPrice = Number(value);
      });
      this.setQueryParameter('maxPrice', queryParams, (value) => {
        this.maxPrice = Number(value);
      });

      if (queryParams.has('category')) {
        const categoryNames = queryParams.getAll('category');
        this.categoryQuery = categoryNames
          .map((categoryName) => `categories=${categoryName}`)
          .join('&');
      }

      this.fetchItems();
    });
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.syncPaginators();
    });
  }

  private fetchItems(): void {
    if (environment.production) {
      this.makeApiSearchCall();
    } else {
      setTimeout(() => {
        this.makeApiSearchCall();
      }, environment.timeout);
    }
  }

  private makeApiSearchCall(): void {
    this.http
      .get(
        `${ItemEndpoints.search}?page=${this.items.page}&limit=${
          this.items.limit
        }${this.title && `&title=${this.title}`}${
          this.minPrice !== null &&
          this.maxPrice !== null &&
          `&minPrice=${this.minPrice}&maxPrice=${this.maxPrice}`
        }&${this.categoryQuery}`
      )
      .subscribe({
        next: (response: PaginatedResponse<BasicItemResponse> | any) => {
          this.items = response;
          this.isLoading = false;
          this.fetchImages();
        },
        error: (error) => {
          console.error(error);
          this.isLoading = false;
          alert('An error occured while searching for items');
        },
      });
  }

  private fetchImages(): void {
    // initialize
    this.items.castEntities.map((item) => {
      item.image = {
        src: '',
        isLoading: true,
      };
    });

    if (this.items.castEntities.length === 0) {
      this.openNoItemsFoundSnackBar();
      return;
    }

    this.items.castEntities.map((item, i) => {
      item.auctionStarted = this.formatter.convertOnlyToDate(item.started);
      item.auctionEnds = this.formatter.convertOnlyToDate(item.ends);
      this.http
        .get(`${ItemEndpoints.getImage(item.imageGuid)}`, {
          responseType: 'blob',
        })
        .subscribe({
          next: (imageData: Blob) => {
            setTimeout(() => {
              const blob = new Blob([imageData], {
                type: 'image/jpeg',
              });
              const imageUrl = URL.createObjectURL(blob);
              item.image = {
                src: imageUrl,
                isLoading: false,
              };
            }, environment.timeout);
          },
          error: (error) => {
            console.error(error);
          },
        });
    });
  }

  public showFiltersDialog(): void {
    const filtersDialogRef = this.filtersDialog.open(FiltersDialogComponent, {
      autoFocus: false,
      restoreFocus: false,
    });

    filtersDialogRef.afterClosed().subscribe((result) => {
      if (result === 'apply') {
        this.showClearFiltersButton = true;
      }
    });
  }

  public showItemDialog(item: ExtendedItemInfo): void {
    this.itemDialog.open(ItemComponent, {
      autoFocus: false,
      restoreFocus: false,
      data: {
        item: item,
        image: item.image,
      },
    });
  }

  public clearFilters(): void {
    this.showClearFiltersButton = false;
    this.filterService.clear();
    this.categoryService.clear();
    this.removeUrlParameters(['minPrice', 'maxPrice', 'category']);
  }

  public removeTitle(): void {
    this.removeUrlParameters(['title']);
  }

  public onSearchSubmit(): void {
    this.addUrlParameters([
      { name: 'page', value: '1' },
      { name: 'title', value: this.title },
    ]);
  }

  public onPageChange(event: PageEvent): void {
    this.isLoading = true;

    if (event.pageSize != this.items.limit) {
      this.items.limit = event.pageSize;
      this.syncPaginators();
      this.fetchItems();
      return;
    }

    const selectedPage = event.pageIndex + 1;

    this.items.page = selectedPage;

    this.syncPaginators();

    this.addUrlParameters([{ name: 'page', value: selectedPage.toString() }]);
  }

  public getNumberArray(limit: number): number[] {
    return Array.from({ length: limit }, (_, i) => i);
  }

  private setQueryParameter(
    paramName: string,
    queryParams: ParamMap,
    setter: (value: string) => void
  ): void {
    if (queryParams.has(paramName)) {
      const value = queryParams.get(paramName);
      if (value != null) {
        setter(value);
        return;
      }
    }
  }

  private openNoItemsFoundSnackBar(): void {
    this.alertService.error('No items found', 'Close');
  }

  private addUrlParameters(params: { name: string; value: string }[]): void {
    let queryParams: Record<string, string> = {};

    params.forEach((param) => {
      queryParams[param.name] = param.value;
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
      queryParamsHandling: 'merge',
    });
  }

  private removeUrlParameters(paramNames: string[]): void {
    let queryParams = { ...this.route.snapshot.queryParams };

    paramNames.forEach((paramName) => {
      delete queryParams[paramName];
    });

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: queryParams,
    });
  }

  private syncPaginators(): void {
    if (this.paginatorTop && this.paginatorBottom) {
      this.paginatorTop.pageIndex = this.items.page - 1;
      this.paginatorBottom.pageIndex = this.items.page - 1;
    }
  }
}

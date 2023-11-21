import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSliderModule } from '@angular/material/slider';
import { MatChipsModule, MatChipListbox } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

@NgModule({
  imports: [
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatDialogModule,
    MatTabsModule,
    MatCardModule,
    FlexLayoutModule,
    MatMenuModule,
    MatTableModule,
    MatListModule,
    MatDividerModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSliderModule,
    MatChipsModule,
    MatSelectModule,
    MatAutocompleteModule,
  ],
  exports: [
    MatIconModule,
    MatButtonModule,
    MatToolbarModule,
    MatDialogModule,
    MatTabsModule,
    MatCardModule,
    FlexLayoutModule,
    MatMenuModule,
    MatTableModule,
    MatListModule,
    MatDividerModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSliderModule,
    MatChipsModule,
    MatSelectModule,
    MatAutocompleteModule,
  ],
})
export class MaterialUiModule {}

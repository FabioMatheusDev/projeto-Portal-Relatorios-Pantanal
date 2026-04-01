import { Routes } from '@angular/router';
import { ReportsHubComponent } from './reports-hub.component';
import { SectorReportsComponent } from './sector-reports.component';

export const REPORTS_ROUTES: Routes = [
  { path: '', component: ReportsHubComponent },
  { path: ':sectorSlug', component: SectorReportsComponent },
];

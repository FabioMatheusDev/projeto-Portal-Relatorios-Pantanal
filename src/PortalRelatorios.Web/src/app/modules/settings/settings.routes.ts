import { Routes } from '@angular/router';
import { adminGuard } from '../../core/guards/admin.guard';
import { PermissionsComponent } from './permissions/permissions.component';

export const SETTINGS_ROUTES: Routes = [
  {
    path: 'permissions',
    component: PermissionsComponent,
    canActivate: [adminGuard],
  },
  { path: '', pathMatch: 'full', redirectTo: 'permissions' },
];

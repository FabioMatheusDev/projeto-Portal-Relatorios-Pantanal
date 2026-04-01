import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment';
import { AdUserRow, PermissionMatrix } from '../../../core/models/auth.models';

type SectorPermissionRow = PermissionMatrix['sectors'][number];

@Component({
  selector: 'app-permissions',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './permissions.component.html',
})
export class PermissionsComponent implements OnInit {
  private readonly http = inject(HttpClient);

  readonly users = signal<AdUserRow[]>([]);
  readonly loading = signal(false);
  readonly selectedUsername = signal<string | null>(null);
  readonly matrix = signal<PermissionMatrix | null>(null);
  readonly selectedUserId = signal<string | null>(null);

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.http.get<AdUserRow[]>(`${environment.apiUrl}/users/active-directory`).subscribe({
      next: (rows) => {
        this.users.set(rows);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  selectUser(row: AdUserRow): void {
    this.selectedUsername.set(row.username);
    this.matrix.set(null);
    const uid = row.portalUserId;
    if (uid) {
      this.selectedUserId.set(uid);
      this.loadMatrix(uid);
      return;
    }
    this.selectedUserId.set(null);
    this.http.post<{ userId: string }>(`${environment.apiUrl}/users/ensure`, { username: row.username }).subscribe({
      next: (r) => {
        this.selectedUserId.set(r.userId);
        this.loadUsers();
        this.loadMatrix(r.userId);
      },
    });
  }

  private loadMatrix(userId: string): void {
    this.http.get<PermissionMatrix>(`${environment.apiUrl}/permissions/matrix/${userId}`).subscribe({
      next: (m) => this.matrix.set(m),
    });
  }

  toggleSector(sectorId: string, canView: boolean): void {
    const m = this.matrix();
    const userId = this.selectedUserId();
    if (!m || !userId) return;
    const next = m.sectors.map((s: SectorPermissionRow) =>
      s.sectorId === sectorId ? { ...s, canView: !canView } : s,
    );
    this.matrix.set({ ...m, sectors: next });
  }

  save(): void {
    const m = this.matrix();
    const userId = this.selectedUserId();
    if (!m || !userId) return;
    this.http
      .put(`${environment.apiUrl}/permissions/matrix`, {
        userId,
        permissions: m.sectors.map((s: SectorPermissionRow) => ({
          sectorId: s.sectorId,
          canView: s.canView,
        })),
      })
      .subscribe({
        next: () => this.loadMatrix(userId),
      });
  }
}

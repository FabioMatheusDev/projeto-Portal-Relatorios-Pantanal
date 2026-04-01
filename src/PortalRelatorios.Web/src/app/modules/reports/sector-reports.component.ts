import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-sector-reports',
  standalone: true,
  templateUrl: './sector-reports.component.html',
})
export class SectorReportsComponent {
  private readonly route = inject(ActivatedRoute);

  readonly slug = toSignal(this.route.paramMap.pipe(map((p) => p.get('sectorSlug') ?? '')), {
    initialValue: '',
  });
}

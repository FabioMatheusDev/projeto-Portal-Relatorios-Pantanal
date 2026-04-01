import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoadingService {
  private readonly _pending = signal(0);
  readonly pending = this._pending.asReadonly();

  begin(): void {
    this._pending.update((n) => n + 1);
  }

  end(): void {
    this._pending.update((n) => Math.max(0, n - 1));
  }
}

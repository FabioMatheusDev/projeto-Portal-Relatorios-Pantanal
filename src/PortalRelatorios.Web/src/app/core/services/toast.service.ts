import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ToastService {
  readonly message = signal<string | null>(null);

  show(msg: string, durationMs = 5000): void {
    this.message.set(msg);
    window.setTimeout(() => this.message.set(null), durationMs);
  }
}

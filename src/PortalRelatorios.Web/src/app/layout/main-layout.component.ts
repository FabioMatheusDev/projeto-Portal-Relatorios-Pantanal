import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {
  readonly auth = inject(AuthService);
  readonly collapsed = signal(false);
  readonly brandLogoSrc = signal('assets/brand/logo.png');

  onBrandLogoError(): void {
    if (this.brandLogoSrc() !== 'assets/brand/logo.svg') {
      this.brandLogoSrc.set('assets/brand/logo.svg');
    }
  }

  toggle(): void {
    this.collapsed.update((c) => !c);
  }

  logout(): void {
    this.auth.logout();
  }
}

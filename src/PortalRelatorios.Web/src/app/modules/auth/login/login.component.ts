import { afterNextRender, Component, DestroyRef, ElementRef, inject, signal, viewChild } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);
  private readonly loginVideo = viewChild<ElementRef<HTMLVideoElement>>('loginVideo');

  readonly submitting = signal(false);
  readonly error = signal<string | null>(null);
  readonly brandLogoSrc = signal('assets/brand/logo.png');

  onBrandLogoError(): void {
    if (this.brandLogoSrc() !== 'assets/brand/logo.svg') {
      this.brandLogoSrc.set('assets/brand/logo.svg');
    }
  }

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  constructor() {
    afterNextRender(() => {
      const ref = this.loginVideo();
      if (!ref) {
        return;
      }
      const el = ref.nativeElement;
      el.muted = true;
      el.defaultMuted = true;
      this.tryPlayLoginVideo(el);

      const resume = (): void => {
        if (!document.hidden) {
          this.tryPlayLoginVideo(el);
        }
      };
      document.addEventListener('visibilitychange', resume);
      this.destroyRef.onDestroy(() => document.removeEventListener('visibilitychange', resume));
    });
  }

  onLoginVideoReady(video: HTMLVideoElement): void {
    video.muted = true;
    this.tryPlayLoginVideo(video);
  }

  private tryPlayLoginVideo(video: HTMLVideoElement): void {
    void video.play().catch(() => {
      /* política de autoplay ou codec: permanece no fallback visual */
    });
  }

  submit(): void {
    if (this.form.invalid || this.submitting()) {
      this.form.markAllAsTouched();
      return;
    }
    this.error.set(null);
    this.submitting.set(true);
    const { username, password } = this.form.getRawValue();
    this.auth.login(username, password).subscribe({
      next: () => {
        this.submitting.set(false);
        void this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.error.set('Não foi possível entrar. Verifique usuário e senha.');
        this.submitting.set(false);
      },
    });
  }
}

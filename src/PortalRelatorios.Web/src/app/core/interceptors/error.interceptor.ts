import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const auth = inject(AuthService);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 401 && req.headers.has('Authorization')) {
        auth.logout();
        toast.show('Sessão expirada ou não autorizado.');
        return throwError(() => err);
      }
      const msg =
        typeof err.error === 'object' && err.error && 'detail' in err.error
          ? String((err.error as { detail?: string }).detail)
          : err.message || 'Erro na requisição.';
      toast.show(msg);
      return throwError(() => err);
    }),
  );
};

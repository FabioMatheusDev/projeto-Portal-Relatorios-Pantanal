import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-reports-hub',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './reports-hub.component.html',
})
export class ReportsHubComponent {
  readonly sectors = [
    { name: 'Administrativo', slug: 'administrativo' },
    { name: 'Comercial', slug: 'comercial' },
    { name: 'Financeiro', slug: 'financeiro' },
    { name: 'Suprimentos', slug: 'suprimentos' },
    { name: 'Controladoria', slug: 'controladoria' },
    { name: 'Crédito', slug: 'credito' },
    { name: 'Gerentes', slug: 'gerentes' },
    { name: 'Lojas', slug: 'lojas' },
    { name: 'TI', slug: 'ti' },
    { name: 'Auditoria', slug: 'auditoria' },
  ];
}

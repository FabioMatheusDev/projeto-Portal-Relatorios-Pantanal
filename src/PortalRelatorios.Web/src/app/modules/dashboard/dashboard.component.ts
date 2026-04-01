import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  readonly kpis = [
    { label: 'Pedidos (mês)', value: '1.284', delta: '+12%', tone: 'text-emerald-600' },
    { label: 'Faturamento', value: 'R$ 4,2M', delta: '+4%', tone: 'text-sky-600' },
    { label: 'Estoque crítico', value: '37', delta: '-3%', tone: 'text-amber-600' },
    { label: 'OTIF entregas', value: '94%', delta: '+1%', tone: 'text-violet-600' },
  ];

  readonly bars = [42, 55, 38, 62, 48, 71, 58];
}

import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OnInit } from '@angular/core';
import { environment } from '../../../environment/environment';
import e from 'express';

@Component({
  selector: 'app-address-list',
  templateUrl: './address-list.component.html',
  styleUrl: './address-list.component.css'
})
export class AddressListComponent implements OnInit {
  apiUrl = environment.apiUrl;
  addresses: any[] = [];

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.http.get<any[]>(this.apiUrl + '/addresses').subscribe({
      next: (res) => this.addresses = res,
      error: () => console.error('Erro ao carregar endereços')
    });
  }

  setAsDefault(id: number) {
    this.http.put(this.apiUrl + `/addresses/${id}/default`, {}).subscribe({
      next: () => alert('Endereço definido como padrão!'),
      error: () => alert('Erro ao definir endereço padrão')
    });
  }
}

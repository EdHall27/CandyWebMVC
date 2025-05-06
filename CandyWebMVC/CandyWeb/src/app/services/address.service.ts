import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../environment/environment';
import { Observable } from 'rxjs';

export interface Address {
  id: number;
  street: string;
  city: string;
  state: string;
  zipCode: string;
}

@Injectable({
  providedIn: 'root',
})
export class AddressService {
  private api = `${environment.apiUrl}/addresses`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Address[]> {
    return this.http.get<Address[]>(this.api);
  }

  add(address: Omit<Address, 'id'>): Observable<any> {
    return this.http.post(this.api, address);
  }

  update(id: number, address: Omit<Address, 'id'>): Observable<any> {
    return this.http.put(`${this.api}/${id}`, address);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.api}/${id}`);
  }

  setDefault(id: number): Observable<any> {
    return this.http.put(`${this.api}/${id}/set-default`, {});
  }
}

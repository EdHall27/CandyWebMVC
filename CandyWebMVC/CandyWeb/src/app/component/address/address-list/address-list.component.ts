import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environment/environment';
import { Address, AddressService } from '../../../services/address.service';

@Component({
  selector: 'app-address-list',
  templateUrl: './address-list.component.html',
  styleUrl: './address-list.component.css'
})
export class AddressListComponent implements OnInit {
  addresses: Address[] = []; 
  selectedAddress: Address | null = null; 

  constructor(private addressService: AddressService) {}

  ngOnInit(): void {
    this.loadAddresses(); 
  }

  loadAddresses(): void {
    this.addressService.getAll().subscribe({
      next: (res) => {this.addresses = res;
        console.log('Endereços recebidos do backend:', this.addresses);
      }, 
      error: (err) => console.error('Erro ao carregar endereços', err), // Log error
    });
  }

  setAsDefault(id: number): void {
    this.addressService.setDefault(id).subscribe({
      next: () => {
        alert('Endereço definido como padrão!');
        this.loadAddresses(); 
      },
      error: (err) => alert('Erro ao definir endereço padrão: ' + (err.error || err.message)), 
    });
  }

  editAddress(address: Address): void {
    this.selectedAddress = { ...address }; 
  }

  deleteAddress(id: number): void {
    if (confirm('Tem certeza que deseja deletar este endereço?')) { // Confirmation dialog
      this.addressService.delete(id).subscribe({
        next: () => {
          alert('Endereço deletado com sucesso!');
          this.loadAddresses();
        },
        error: (err) => alert('Erro ao deletar endereço: ' + (err.error || err.message)), 
      });
    }
  }

  onFormSubmitted(): void {
    this.selectedAddress = null; 
    this.loadAddresses(); 
  }
}

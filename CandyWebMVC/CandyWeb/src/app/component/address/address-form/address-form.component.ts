import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Address, AddressService } from '../../../services/address.service';

@Component({
  selector: 'app-address-form',
  templateUrl: './address-form.component.html',
  styleUrl: './address-form.component.css'
})
export class AddressFormComponent implements OnInit {

  
  @Input() addressToEdit: Address | null = null;
 
  @Output() formSubmitted = new EventEmitter<void>();

  addressForm: FormGroup; 

  constructor(private fb: FormBuilder, private addressService: AddressService) {
    this.addressForm = this.fb.group({
      street: ['', Validators.required], 
      city: ['', Validators.required],   
      state: ['', Validators.required],  
      CEP: ['', Validators.required], 
    });
  }

  ngOnInit(): void {
    if (this.addressToEdit) {
      this.addressForm.patchValue(this.addressToEdit);
    }
  }

  onSubmit(): void {
    if (this.addressForm.valid) { 
      const addressData: Omit<Address, 'id'> = this.addressForm.value;

      if (this.addressToEdit) {
        this.addressService
          .update(this.addressToEdit.id, addressData)
          .subscribe({
            next: () => {
              alert('Endereço atualizado com sucesso!'); 
              this.formSubmitted.emit(); 
              this.addressForm.reset(); 
              this.addressToEdit = null; 
            },
            error: () => alert('Erro ao atualizar endereço.'), // Error message
          });
      } else {
        this.addressService.add(addressData).subscribe({
          next: () => {
            alert('Endereço adicionado com sucesso!');
            this.formSubmitted.emit(); 
            this.addressForm.reset(); 
          },
          error: () => alert('Erro ao adicionar endereço.'),
        });
      }
    } else {
      alert('Por favor, preencha todos os campos obrigatórios.'); 
    }
  }
}

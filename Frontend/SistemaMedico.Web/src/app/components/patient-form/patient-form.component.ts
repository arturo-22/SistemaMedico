import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PatientService } from '../../services/patient.service';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './patient-form.component.html',
})
export class PatientFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private patientService = inject(PatientService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  patientForm: FormGroup;
  isEditMode = false;
  patientId?: number;
  today = new Date().toISOString().split('T')[0];

  constructor() {
    this.patientForm = this.fb.group({
      id: [0],
      firstName: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.pattern('^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$'),
        ],
      ],
      lastName: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.pattern('^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$'),
        ],
      ],
      birthDate: ['', [Validators.required, this.noFutureDate]],
      gender: ['', [Validators.required]],
      address: ['', [Validators.required, Validators.minLength(5)]],
      phone: [
        '',
        [
          Validators.required,
          Validators.pattern('^[0-9]{9}$'),
          Validators.minLength(9),
          Validators.maxLength(9),
        ],
      ],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.patientId = Number(id);
      this.loadPatient(this.patientId);
    }
  }

  loadPatient(id: number): void {
  this.patientService.getById(id).subscribe((patient) => {
    this.patientForm.patchValue({
      ...patient,
      birthDate: patient.birthDate
        ? new Date(patient.birthDate).toISOString().split('T')[0]
        : '',
    });
  });
}

  onlyLetters(event: KeyboardEvent): boolean {
    const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]$/;
    if (
      event.key === 'Backspace' ||
      event.key === 'Tab' ||
      event.key === 'Delete' ||
      event.key === 'ArrowLeft' ||
      event.key === 'ArrowRight'
    ) {
      return true;
    }
    if (!regex.test(event.key)) {
      event.preventDefault();
      return false;
    }
    return true;
  }

  noFutureDate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const inputDate = new Date(control.value);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if (inputDate > today) {
      return { futureDate: true };
    }
    return null;
  }

  onlyNumbers(event: KeyboardEvent): boolean {
    if (
      event.key === 'Backspace' ||
      event.key === 'Tab' ||
      event.key === 'Delete' ||
      event.key === 'ArrowLeft' ||
      event.key === 'ArrowRight'
    ) {
      return true;
    }
    if (!/^[0-9]$/.test(event.key)) {
      event.preventDefault();
      return false;
    }
    return true;
  }

  getError(field: string): string {
    const control = this.patientForm.get(field);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;

    if (errors['required']) return 'Este campo es obligatorio.';
    if (errors['minlength']) {
      const min = errors['minlength'].requiredLength;
      return `Mínimo ${min} caracteres requeridos.`;
    }
    if (errors['maxlength']) {
      const max = errors['maxlength'].requiredLength;
      return `Máximo ${max} caracteres permitidos.`;
    }
    if (errors['futureDate'])
      return 'La fecha de nacimiento no puede ser una fecha futura.';
    if (errors['pattern']) {
      if (field === 'phone')
        return 'El teléfono debe contener exactamente 9 dígitos.';
      if (field === 'firstName' || field === 'lastName')
        return 'Solo se permiten letras.';
    }
    if (errors['email']) {
      const value = control.value;
      return `"${value}" no es un correo electrónico válido.`;
    }

    return 'Campo inválido.';
  }

  onSubmit(): void {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }

    const patientData = this.patientForm.value;

    if (this.isEditMode) {
      this.patientService.update(this.patientId!, patientData).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    } else {
      this.patientService.create(patientData).subscribe(() => {
        this.router.navigate(['/patients']);
      });
    }
  }
}

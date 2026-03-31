import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Appointment } from '../../models/appointment.model';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';

@Component({
  selector: 'app-appointment-attend-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './appointment-attend-modal.component.html',
})
export class AppointmentAttendModalComponent {
  private fb = inject(FormBuilder);

  @Input() appointment: Appointment | null = null;
  @Output() onSave = new EventEmitter<{
    diagnosis: string;
    treatment: string;
  }>();
  @Output() onClose = new EventEmitter<void>();

  atencionForm: FormGroup = this.fb.group({
    diagnosis: [
      '',
      [Validators.required, Validators.minLength(5), Validators.maxLength(500)],
    ],
    treatment: [
      '',
      [Validators.required, Validators.minLength(5), Validators.maxLength(500)],
    ],
  });

  getError(field: string): string {
    const control = this.atencionForm.get(field);
    if (!control || !control.errors || !control.touched) return '';
    const errors = control.errors;
    if (errors['required']) return 'Este campo es obligatorio.';
    if (errors['minlength'])
      return `Mínimo ${errors['minlength'].requiredLength} caracteres requeridos.`;
    if (errors['maxlength'])
      return `Máximo ${errors['maxlength'].requiredLength} caracteres permitidos.`;
    return 'Campo inválido.';
  }

  save(): void {
    if (this.atencionForm.invalid) {
      this.atencionForm.markAllAsTouched();
      return;
    }
    this.onSave.emit(this.atencionForm.value);
  }

  Close(): void {
    this.atencionForm.reset();
    this.atencionForm.markAsUntouched();
    this.onClose.emit();
  }
}

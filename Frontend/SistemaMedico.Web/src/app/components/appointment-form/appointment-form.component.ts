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
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { PatientService } from '../../services/patient.service';
import { AppointmentService } from '../../services/appointment.service';
import { Patient } from '../../models/patient.model';
import { AppointmentStatus } from '../../models/enums/appointment-status.enum';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-appointment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './appointment-form.component.html',
})
export class AppointmentFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private patientService = inject(PatientService);
  private appointmentService = inject(AppointmentService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  appointmentForm: FormGroup;
  patients: Patient[] = [];
  appointmentId: number | null = null;
  minDate: string = new Date().toISOString().slice(0, 16);

  statusOptions = [
    { value: AppointmentStatus.Pending, label: 'Pendiente' },
    { value: AppointmentStatus.Confirmed, label: 'Confirmada' },
    { value: AppointmentStatus.Attended, label: 'Atendida' },
    { value: AppointmentStatus.Cancelled, label: 'Cancelada' },
  ];

  constructor() {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      doctorName: [
        '',
        [
          Validators.required,
          Validators.minLength(3),
          Validators.pattern('^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$'),
        ],
      ],
      appointmentDate: ['', [Validators.required, this.noPastDate]],
      status: [AppointmentStatus.Pending, [Validators.required]],
      reason: [
        '',
        [
          Validators.required,
          Validators.minLength(5),
          Validators.maxLength(500),
        ],
      ],
    });
  }

  ngOnInit(): void {
    this.patientService.getAll().subscribe({
      next: (data) => (this.patients = data),
      error: () =>
        Swal.fire('Error', 'No se pudieron cargar los pacientes.', 'error'),
    });

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.appointmentId = Number(idParam);
      this.loadAppointmentData(this.appointmentId);
    } else {
      this.appointmentForm.get('status')?.setValue(AppointmentStatus.Pending);
      this.appointmentForm.get('status')?.disable();
    }
  }

  loadAppointmentData(id: number): void {
    this.appointmentService.getById(id).subscribe({
      next: (appt) => {
        if (appt.appointmentDate) {
          appt.appointmentDate = new Date(appt.appointmentDate)
            .toISOString()
            .slice(0, 16);
        }
        this.appointmentForm.get('status')?.enable();
        this.appointmentForm.patchValue(appt);
      },
      error: () =>
        Swal.fire(
          'Error',
          'No se pudo cargar la información de la cita.',
          'error',
        ),
    });
  }

  noPastDate(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const selected = new Date(control.value);
    const now = new Date();
    return selected < now ? { pastDate: true } : null;
  }

  onlyLetters(event: KeyboardEvent): boolean {
    const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]$/;
    if (
      ['Backspace', 'Tab', 'Delete', 'ArrowLeft', 'ArrowRight'].includes(
        event.key,
      )
    )
      return true;
    if (!regex.test(event.key)) {
      event.preventDefault();
      return false;
    }
    return true;
  }

  getError(field: string): string {
    const control = this.appointmentForm.get(field);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;
    if (errors['required']) return 'Este campo es obligatorio.';
    if (errors['minlength'])
      return `Mínimo ${errors['minlength'].requiredLength} caracteres.`;
    if (errors['pattern']) return 'Solo se permiten letras.';
    if (errors['pastDate']) return 'No puedes agendar en una fecha pasada.';
    return 'Campo inválido.';
  }

  onSubmit(): void {
    if (this.appointmentForm.invalid) {
      this.appointmentForm.markAllAsTouched();
      return;
    }

    const data = this.appointmentForm.getRawValue();

    if (this.appointmentId) {
      this.appointmentService
        .update(this.appointmentId, { id: this.appointmentId, ...data })
        .subscribe({
          next: () => this.notifySuccess('Cita actualizada correctamente'),
          error: (err) => this.notificarError(err),
        });
    } else {
      this.appointmentService.create(data).subscribe({
        next: () => this.notifySuccess('Cita agendada correctamente'),
        error: (err) => this.notificarError(err),
      });
    }
  }

  private notifySuccess(mensaje: string): void {
    Swal.fire({
      title: '¡Éxito!',
      text: mensaje,
      icon: 'success',
      timer: 2000,
      showConfirmButton: false,
    }).then(() => this.router.navigate(['/appointments']));
  }

private notificarError(err: any): void {
  const mensajeServidor = typeof err.error === 'string' 
    ? err.error 
    : (err.error?.message || 'Ocurrió un error al procesar la solicitud.');

  Swal.fire({
    title: 'Aviso del Sistema',
    text: mensajeServidor,
    icon: 'warning',
    confirmButtonColor: '#0d6efd',
  });
}
}

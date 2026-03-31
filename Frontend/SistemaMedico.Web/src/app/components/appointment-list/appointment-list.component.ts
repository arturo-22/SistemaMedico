import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AppointmentService } from '../../services/appointment.service';
import { Appointment } from '../../models/appointment.model';
import { AppointmentStatus } from '../../models/enums/appointment-status.enum';
import { AppointmentAttendModalComponent } from '../appointment-attend-modal/appointment-attend-modal.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-appointment-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    AppointmentAttendModalComponent,
  ],
  templateUrl: './appointment-list.component.html',
})
export class AppointmentListComponent implements OnInit {
  private appointmentService = inject(AppointmentService);
  private route = inject(ActivatedRoute);

  patientFilterId?: number;

  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  selectedAppt: Appointment | null = null;
  Status = AppointmentStatus;

  filterText: string = '';
  filterStatus: number = -1;

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      const pid = params['patientId'];
      this.patientFilterId = pid ? +pid : undefined;
      this.loadAppointments();
    });
  }

  loadAppointments(): void {
    this.appointmentService.getAll().subscribe({
      next: (data) => {
        this.appointments = data;
        this.applyFilters();
      },
      error: (err) =>
        Swal.fire('Error', 'No se pudieron cargar las citas.', 'error'),
    });
  }

  applyFilters(): void {
    const text = this.filterText.toLowerCase().trim();
    this.filteredAppointments = this.appointments.filter((appt) => {
      const matchesPatient = !this.patientFilterId || appt.patientId === this.patientFilterId;
      const matchesText =
        !text ||
        appt.patientFullName?.toLowerCase().includes(text) ||
        appt.doctorName?.toLowerCase().includes(text);

      const matchesStatus =
        this.filterStatus == -1 || appt.status == Number(this.filterStatus);

      return matchesPatient && matchesText && matchesStatus;
    });
  }

  openAttentionModal(appt: Appointment): void {
    this.selectedAppt = appt;
  }

  confirmAttention(datosMedicos: {
    diagnosis: string;
    treatment: string;
  }): void {
    if (!this.selectedAppt) return;

    const attendDto = {
      id: this.selectedAppt.id,
      diagnosis: datosMedicos.diagnosis,
      treatment: datosMedicos.treatment,
    };

    this.appointmentService.attend(attendDto.id, attendDto).subscribe({
      next: () => {
        this.loadAppointments();
        this.closeModal();
        Swal.fire({
          title: '¡Atendida!',
          text: 'La atención médica ha sido registrada.',
          icon: 'success',
          timer: 2000,
          showConfirmButton: false,
        });
      },
      error: (err) => {
        const msg =
          typeof err.error === 'string'
            ? err.error
            : 'No se pudo registrar la atención.';
        Swal.fire('Atención', msg, 'warning');
      },
    });
  }

  ViewSummaryAttention(appt: Appointment): void {
    Swal.fire({
      title: `<span class="text-primary">Resumen de Atención</span>`,
      icon: 'info',
      html: `
      <div class="text-start mt-3">
        <p><strong>Paciente:</strong> ${appt.patientFullName}</p>
        <p><strong>Médico:</strong> ${appt.doctorName}</p>
        <hr>
        <div class="bg-light p-3 rounded">
          <h6 class="fw-bold text-dark">Diagnóstico:</h6>
          <p class="mb-3">${appt.diagnosis || 'No registrado'}</p>
          <h6 class="fw-bold text-dark">Tratamiento y Receta:</h6>
          <p class="mb-0 text-muted" style="white-space: pre-line;">${appt.treatment || 'No registrado'}</p>
        </div>
      </div>
    `,
      confirmButtonText: 'Cerrar',
      confirmButtonColor: '#0d6efd',
      showCloseButton: true,
    });
  }

  closeModal(): void {
    this.selectedAppt = null;
  }

  getStatusClass(status: number): string {
    switch (status) {
      case AppointmentStatus.Pending:
        return 'badge bg-warning text-dark';
      case AppointmentStatus.Confirmed:
        return 'badge bg-info text-dark';
      case AppointmentStatus.Attended:
        return 'badge bg-success text-dark';
      case AppointmentStatus.Cancelled:
        return 'badge bg-danger text-dark';
      default:
        return 'badge bg-secondary text-dark';
    }
  }

  getStatusLabel(status: number): string {
    switch (status) {
      case AppointmentStatus.Pending:
        return 'Pendiente';
      case AppointmentStatus.Confirmed:
        return 'Confirmada';
      case AppointmentStatus.Attended:
        return 'Atendida';
      case AppointmentStatus.Cancelled:
        return 'Cancelada';
      default:
        return 'Desconocido';
    }
  }

  deleteAppointment(id: number): void {
    const appt = this.appointments.find((a) => a.id === id);
    if (!appt) return;

    Swal.fire({
      title: '¿Confirmar cancelación?',
      text: `La cita del paciente ${appt.patientFullName} pasará a estado Cancelada.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sí, cancelar cita',
      cancelButtonText: 'Volver',
    }).then((result) => {
      if (result.isConfirmed) {
        this.appointmentService.delete(id).subscribe({
          next: () => {
            this.loadAppointments();
            Swal.fire(
              '¡Cancelada!',
              'La cita ha sido marcada como cancelada.',
              'success',
            );
          },
          error: (err) => {
            const msg =
              typeof err.error === 'string'
                ? err.error
                : 'No se pudo realizar la acción.';
            Swal.fire('Error', msg, 'error');
          },
        });
      }
    });
  }
}

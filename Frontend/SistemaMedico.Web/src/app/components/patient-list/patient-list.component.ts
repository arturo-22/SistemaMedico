import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../services/patient.service';
import { Patient } from '../../models/patient.model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-patient-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './patient-list.component.html',
})
export class PatientListComponent implements OnInit {
  private patientService = inject(PatientService);
  private searchSubject = new Subject<string>();

  patients: Patient[] = [];
  searchTerm: string = '';

  ngOnInit(): void {
    this.loadPatients();
    this.searchSubject
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((term) => {
        if (term.trim()) {
          this.patientService.search(term).subscribe({
            next: (data) => (this.patients = data),
            error: (err) =>
              Swal.fire('Error', 'No se pudo realizar la búsqueda.', 'error'),
          });
        } else {
          this.loadPatients();
        }
      });
  }

  loadPatients(): void {
    this.patientService.getAll().subscribe({
      next: (data) => (this.patients = data),
      error: (err) =>
        Swal.fire('Error', 'No se pudieron cargar los pacientes.', 'error'),
    });
  }

  onSearch(): void {
    this.searchSubject.next(this.searchTerm);
  }

  deletePatient(id: number): void {
    const patient = this.patients.find((p) => p.id === id);
    const patientName = patient
      ? `${patient.firstName} ${patient.lastName}`
      : 'este paciente';

    Swal.fire({
      title: '¿Estás seguro?',
      text: `¿Deseas eliminar a ${patientName}? Esta acción no se puede deshacer.`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#6c757d',
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.isConfirmed) {
        this.patientService.delete(id).subscribe({
          next: () => {
            this.loadPatients();
            Swal.fire({
              title: '¡Eliminado!',
              text: 'El paciente ha sido eliminado correctamente.',
              icon: 'success',
              timer: 2000,
              showConfirmButton: false,
            });
          },
          error: (err) => {
            const errorMessage =
              typeof err.error === 'string'
                ? err.error
                : err.error?.message ||
                  'No se pudo eliminar al paciente. Es posible que tenga citas registradas.';

            Swal.fire({
              title: 'Error al eliminar',
              text: errorMessage,
              icon: 'error',
              confirmButtonColor: '#0d6efd',
            });

            console.error('Error al eliminar paciente:', err);
          },
        });
      }
    });
  }
}

import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Appointment } from '../models/appointment.model';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AppointmentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Appointments`;

  // Obtener lista completa
  getAll(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(`${this.apiUrl}/List`);
  }

  // Obtener por ID
  getById(id: number): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.apiUrl}/GetById/${id}`);
  }

  // Crear cita
  create(appointment: Appointment): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/Create`, appointment);
  }

  // Actualizar cita
  update(id: number, appointment: Appointment): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Update/${id}`, appointment);
  }

  // Atender cita (Para el modal)
  attend(
    id: number,
    attendData: { id: number; diagnosis: string; treatment: string },
  ): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Attend/${id}`, attendData);
  }

  // Eliminar cita
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete/${id}`);
  }
}

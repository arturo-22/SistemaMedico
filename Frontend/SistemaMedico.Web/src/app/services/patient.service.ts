import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient } from '../models/patient.model';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class PatientService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Patients`;

  // Obtener todos los pacientes
  getAll(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.apiUrl}/List`);
  }

  // Obtener un paciente por ID
  getById(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.apiUrl}/GetById/${id}`);
  }

  // Buscar pacientes por nombre o apellido
  search(term: string): Observable<Patient[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<Patient[]>(`${this.apiUrl}/Search`, { params });
  }
  // Crear un nuevo paciente
  create(patient: Patient): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/Create`, patient);
  }

  // Actualizar un paciente existente
  update(id: number, patient: Patient): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/Update/${id}`, patient);
  }

  // Eliminar un paciente
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Delete/${id}`);
  }
}

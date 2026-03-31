export interface Appointment {
  id: number;
  patientId: number;
  patientFullName: string;
  doctorName: string;
  appointmentDate: string;
  status: number;
  reason: string;
  diagnosis?: string;
  treatment?: string;
}
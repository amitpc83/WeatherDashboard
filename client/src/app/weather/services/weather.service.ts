import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { IWeatherData } from '../IWeatherData';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {
  private baseUrl = 'https://localhost:7107/api/Weather';

  constructor(private http: HttpClient) { }

  getWeatherData(cities: string): Observable<IWeatherData[]> {
    return this.http.get<IWeatherData[]>(`${this.baseUrl}/weatherData?cities=${cities}`, { withCredentials: true })
      .pipe(
        catchError(this.handleError)
      );
  }

  getTemperatureUnit(): Observable<{ unit: 'metric' | 'imperial' }> {
    return this.http.get<{ unit: 'metric' | 'imperial' }>(`${this.baseUrl}/temperatureUnit`, { withCredentials: true })
      .pipe(
        catchError(this.handleError)
      );
  }

  setTemperatureUnit(unit: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/temperatureUnit`, { unit }, { withCredentials: true })
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred.
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    return throwError(() => new Error(errorMessage));
  }
}


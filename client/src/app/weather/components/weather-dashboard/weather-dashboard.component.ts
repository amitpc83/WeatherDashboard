import { Component, OnInit } from '@angular/core';
import { WeatherService } from '../../services/weather.service';
import { IWeatherData } from '../../IWeatherData';

@Component({
  selector: 'app-weather-dashboard',
  templateUrl: './weather-dashboard.component.html',
  styleUrls: ['./weather-dashboard.component.css'],
  standalone:false
})
export class WeatherDashboardComponent implements OnInit {
  locations: string[] = ['London', 'Vienna', 'Ljubljana', 'Belgrade', 'Valletta'];
  weatherData: IWeatherData[] = [];
  errorMessage: string | null = null;
  unit: 'metric' | 'imperial' = 'metric';
  displayUnit: 'metric' | 'imperial' = 'metric'; // For frontend-only toggling
  showSunrise: boolean = true;

  constructor(private service: WeatherService) { }

  ngOnInit(): void {
    this.getUserUnitPreference();
  }

  getUserUnitPreference(): void {
    this.service.getTemperatureUnit().subscribe({
      next: response => {
        this.unit = response.unit;
        this.displayUnit = this.unit;
        this.getWeatherData();
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error);
      }
    });

  }

  getWeatherData(): void {
    const cities = this.locations.join(',');
    this.service.getWeatherData(cities).subscribe({
      next: data => {
        this.weatherData = data;
        this.errorMessage = null;
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error);
      }
    });

  }

  setUserUnitPreference(): void {
    this.service.setTemperatureUnit(this.unit).subscribe({
      next: () => {
        this.displayUnit = this.unit;
        this.getWeatherData();
        this.errorMessage = null;
      },
      error: error => {
        this.errorMessage = this.getErrorMessage(error);
      }
    });
  }

  setUserPreferredUnit(unit: 'metric' | 'imperial'): void {
    this.unit = unit;
    this.setUserUnitPreference();
  }

  setDisplayUnit(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.displayUnit = inputElement.checked ? 'imperial' : 'metric';
  }

  toggleSunriseSunset(showSunrise: boolean): void {
    this.showSunrise = showSunrise;
  }

  private getErrorMessage(error: any): string {
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      return `Error: ${error.error.message}`;
    } else {
      // Server-side error
      return `Error Code: ${error.status}\nMessage: ${error.message}`;
    }
  }
}

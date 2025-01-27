import { Component, Input } from '@angular/core';
import { IWeatherData } from '../../IWeatherData';
import { faSun, faMoon } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-weather-card',
  templateUrl: './weather-card.component.html',
  styleUrls: ['./weather-card.component.css'],
  standalone:false
})
export class WeatherCardComponent {
  @Input() weather: IWeatherData | undefined;
  @Input() showSunrise: boolean = true;
  @Input() displayUnit: 'metric' | 'imperial' = 'metric';

  faSun = faSun;
  faMoon = faMoon;
}

using System;
using Newtonsoft.Json;

namespace api
{
    public class WeatherForecast
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        
        public String Location { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}

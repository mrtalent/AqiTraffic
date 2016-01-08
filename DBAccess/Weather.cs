using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AqiTraffic.DataAccess
{
    public class Weather
    {
        public double rain;
        public double temperature;
        public double windSpeed;
        public int label;
        public Weather(double rain,double temperature, double windSpeed, int label)
        {
            this.rain = rain;
            this.temperature = temperature;
            this.windSpeed = windSpeed;
            this.label = label;
        }
        public Weather() { }
    }
}

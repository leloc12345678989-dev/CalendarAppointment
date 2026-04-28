using System;
using Newtonsoft.Json;

namespace CalendarApp
{
    public class Appointment
    {
        public string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:HH:mm}: {Title}";
        }
    }
}
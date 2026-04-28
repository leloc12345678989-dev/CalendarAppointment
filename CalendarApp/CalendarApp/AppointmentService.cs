using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace CalendarApp
{
    public static class AppointmentService
    {
        private static readonly string FilePath = "appointments.json";

        public static bool ValidateTime(Appointment appt)
        {
            return appt.StartTime < appt.EndTime;
        }

        public static bool IsOverlap(Appointment newAppt, List<Appointment> existing)
        {
            return existing.Any(e => !(newAppt.EndTime <= e.StartTime || newAppt.StartTime >= e.EndTime));
        }

        public static void SaveToFile(List<Appointment> appointments)
        {
            string json = JsonConvert.SerializeObject(appointments, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static List<Appointment> LoadFromFile()
        {
            if (!File.Exists(FilePath)) return new List<Appointment>();
            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<Appointment>>(json) ?? new List<Appointment>();
        }
    }
}
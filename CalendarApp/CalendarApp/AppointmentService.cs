using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using Timer = System.Timers.Timer;

namespace CalendarApp
{
    public enum ValidationResult
    {
        VALID,
        EMPTY_TITLE,
        NEGATIVE_DURATION
    }

    public class CalendarData
    {
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<GroupMeeting> GroupMeetings { get; set; } = new List<GroupMeeting>();
        public List<Reminder> Reminders { get; set; } = new List<Reminder>();
    }

    public static class AppointmentService
    {
        private static readonly string FilePath = "calendar_data.json";
        private static Timer reminderTimer;
        private static CalendarData currentData;

        static AppointmentService()
        {
            StartReminderTimer();
        }

        public static ValidationResult ValidateAppointment(Appointment appt)
        {
            if (string.IsNullOrWhiteSpace(appt.Title))
                return ValidationResult.EMPTY_TITLE;
            if (appt.StartTime >= appt.EndTime)
                return ValidationResult.NEGATIVE_DURATION;
            return ValidationResult.VALID;
        }

        public static List<Appointment> CheckOverlap(Appointment newAppt, List<Appointment> existing)
        {
            return existing.Where(e => newAppt.IsOverlap(e)).ToList();
        }

        public static GroupMeeting FindMatchingGroupMeeting(Appointment appt, List<GroupMeeting> meetings)
        {
            return meetings.FirstOrDefault(m => m.IsSimilarTo(appt));
        }

        public static void SaveToFile(CalendarData data)
        {
            currentData = data;
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        public static CalendarData LoadFromFile()
        {
            if (!File.Exists(FilePath)) return new CalendarData();
            string json = File.ReadAllText(FilePath);
            currentData = JsonConvert.DeserializeObject<CalendarData>(json) ?? new CalendarData();
            return currentData;
        }

        private static void StartReminderTimer()
        {
            reminderTimer = new Timer(60000); // Check every minute
            reminderTimer.Elapsed += (s, e) => CheckReminders();
            reminderTimer.Start();
        }

        public static void CheckReminders()
        {
            if (currentData == null) return;
            foreach (var reminder in currentData.Reminders)
            {
                reminder.Trigger();
            }
            SaveToFile(currentData);
        }

        public static void AddReminder(string appointmentId, string title, DateTime reminderTime)
        {
            if (currentData == null) currentData = LoadFromFile();
            currentData.Reminders.Add(new Reminder
            {
                AppointmentId = appointmentId,
                AppointmentTitle = title,
                ReminderTime = reminderTime
            });
            SaveToFile(currentData);
        }
    }
}
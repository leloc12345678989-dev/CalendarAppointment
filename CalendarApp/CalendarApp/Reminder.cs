using System;
using System.Windows.Forms;

namespace CalendarApp
{
    public class Reminder
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AppointmentId { get; set; }
        public string AppointmentTitle { get; set; }
        public DateTime ReminderTime { get; set; }
        public bool IsTriggered { get; set; } = false;

        public void Trigger()
        {
            if (!IsTriggered && DateTime.Now >= ReminderTime)
            {
                IsTriggered = true;
                MessageBox.Show($"🔔 NHẮC NHỞ: {AppointmentTitle}\nThời gian: {ReminderTime:HH:mm}",
                    "Calendar Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
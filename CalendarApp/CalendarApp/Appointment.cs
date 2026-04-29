using System;

namespace CalendarApp
{
    public class Appointment
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public int ReminderMinutes { get; set; } = 0;

        public TimeSpan Duration => EndTime - StartTime;

        public virtual bool IsOverlap(Appointment other)
        {
            return !(EndTime <= other.StartTime || StartTime >= other.EndTime);
        }

        public virtual bool IsSimilarTo(Appointment other)
        {
            return Title.Equals(other.Title, StringComparison.OrdinalIgnoreCase) &&
                   Math.Abs((Duration - other.Duration).TotalMinutes) < 5;
        }

        public override string ToString()
        {
            return $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:HH:mm}: {Title} | {Location}";
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace CalendarApp
{
    public class GroupMeeting : Appointment
    {
        public List<string> Participants { get; set; } = new List<string>();
        public int MaxParticipants { get; set; } = 20;

        public bool AddParticipant(string userName)
        {
            if (Participants.Count >= MaxParticipants) return false;
            if (Participants.Contains(userName)) return false;
            Participants.Add(userName);
            return true;
        }

        public bool RemoveParticipant(string userName)
        {
            return Participants.Remove(userName);
        }

        public int GetParticipantCount() => Participants.Count;

        public override string ToString()
        {
            return base.ToString() + $" [GROUP: {Participants.Count}/{MaxParticipants}]";
        }
    }
}
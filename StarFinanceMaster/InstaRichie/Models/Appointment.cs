using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class Appointment
    {
        [PrimaryKey, AutoIncrement]
        public int AppointmentID { get; set; }

        [NotNull, Unique]
        public string EventName { get; set; }
        [NotNull]
        public string Location { get; set; }
        [NotNull]
        public string EventDate { get; set; }
        [NotNull]
        public string StartTime { get; set; }
        [NotNull]
        public string EndTime { get; set; }
    }
}

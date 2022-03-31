using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Attributes;

namespace StartFinance.Models
{
    class PersonalInfoDB
    {
        [PrimaryKey, AutoIncrement]
        public int PersonalID { get; set; }

        [NotNull]
        public string FirstName { get; set; }

        [NotNull]
        public string LastName{ get; set; }

        [NotNull]
        public string DateOfBirth { get; set; }

        [NotNull]
        public string Gender { get; set; }

        [NotNull]
        public string EmailAdress{ get; set; }

        [NotNull]
        public string PhoneNumber { get; set; }
    }
}

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public class Employees
    {
        public string Firstname;
        public string Lastname;
        public string Gender;
        public string Pin;
        public string Password;
        public Date BirthDate;
        public Date HireDate;
        public string Position;
    }
}

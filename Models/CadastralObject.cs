using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastralManagementSystem.Models
{
    public class CadastralObject
    {
        public int Id { get; set; }
        public string CadastralNumber { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }       
        public string Address { get; set; }
        public double? Area { get; set; }
        public string OwnerName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Status { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CadastralManagementSystem.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int ApplicantId { get; set; }
        public string ApplicantName { get; set; }   
        public int CadastralObjectTypeId { get; set; }
        public string CadastralObjectTypeName { get; set; }
        public string Address { get; set; }
        public double? Area { get; set; }
        public string ApplicantComment { get; set; }
        public string Status { get; set; }          
        public DateTime SubmissionDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string EmployeeComment { get; set; }
        public string ResultingCadastralNumber { get; set; }
    }
}
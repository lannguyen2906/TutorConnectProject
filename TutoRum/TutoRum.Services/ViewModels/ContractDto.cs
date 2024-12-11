using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TutoRum.Services.ViewModels
{
    public class ListContractDto
    {
        public int totalRecords { get; set; }
        public List<ContractDto> Contracts { get; set; }
    }


    public class ContractDto
    {
        public int ContractId { get; set; }
        public string ClassName { get; set; }
        public string TutorName { get; set; }
        public string ContractImg { get; set; }
        public decimal Rate { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? IsVerified { get; set; }
    }

    public class ContractFilterDto
    {
        public string? Search { get; set; }
        public bool? IsVerified { get; set;}
    }
}

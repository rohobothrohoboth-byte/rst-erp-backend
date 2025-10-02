using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Domain.Entities;

public class EmployeePhoto
{
    public Guid EmployeeId { get; set; } = default!; //Employee

    //******************************************//

    public Employee Employee { get; set; } = null!;
}
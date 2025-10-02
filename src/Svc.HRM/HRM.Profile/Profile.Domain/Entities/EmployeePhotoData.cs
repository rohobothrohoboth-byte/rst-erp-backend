using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profile.Domain.Entities;

public class EmployeePhotoData
{
    public Guid EmployeePhotoId { get; set; } = default!; //EmployeePhoto

    //******************************************//

    public EmployeePhoto EmployeePhoto { get; set; } = null!;
}

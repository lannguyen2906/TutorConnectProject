using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.IService
{
    public interface IQualificationLevelService
    {
        Task<IEnumerable<QualificationLevelDto>> GetAllQualificationLevelsAsync();


    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Data.Infrastructure;
using TutoRum.Data.IRepositories;
using TutoRum.Data.Models;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;

namespace TutoRum.Services.Service
{
    public class QualificationLevelService : IQualificationLevelService
    {
      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QualificationLevelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QualificationLevelDto>> GetAllQualificationLevelsAsync()
        {
            var levels = await _unitOfWork.QualificationLevel.GetAllQualificationLevelsAsync();
            return _mapper.Map<IEnumerable<QualificationLevelDto>>(levels);
        }

       
    }
}
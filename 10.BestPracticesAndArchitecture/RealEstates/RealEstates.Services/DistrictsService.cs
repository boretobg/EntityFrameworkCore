using RealEstates.Data;
using RealEstates.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstates.Services
{
    public class DistrictsService : IDistricsService
    {
        private readonly ApplicationDbContext dbContext;

        public DistrictsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IEnumerable<DistrictInfoDto> GetMostExpensiveDistricts(int count)
        {
            return new List<DistrictInfoDto>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Services
{
    public interface ISpecialDataExportExcelHelper
    {
        IQueryable<SpecialData> GetSpecialDataForSpcConId(int specialConfigId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Services
{
    public class SpecialDataExportExcelHelper : ISpecialDataExportExcelHelper
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IQueryable<SpecialData> GetSpecialDataForSpcConId(int specialConfigId)
        {
            return db.SpecialDatas.Where(x => x.SpecialConfigId == specialConfigId).OrderBy(x => x.SpecialDataTypeId).ThenBy(x => x.Part);
        }
    }
}
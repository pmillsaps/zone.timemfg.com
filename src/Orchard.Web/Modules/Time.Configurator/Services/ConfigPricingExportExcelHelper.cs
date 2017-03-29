using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Services
{
    public class ConfigPricingExportExcelHelper : IConfigPricingExportExcelHelper
    {
        private ConfiguratorEntities db = new ConfiguratorEntities();

        public IQueryable<ConfigPricing> GetConfigPricingForCfgName(string configName)
        {
            return db.ConfigPricings.Where(x => x.ConfigID == configName);
        }
    }
}
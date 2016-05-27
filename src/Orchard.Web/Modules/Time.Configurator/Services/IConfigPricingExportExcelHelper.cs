using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.Configurator;

namespace Time.Configurator.Services
{
    public interface IConfigPricingExportExcelHelper
    {
        IQueryable<ConfigPricing> GetConfigPricingForCfgName(string configName);
    }
}
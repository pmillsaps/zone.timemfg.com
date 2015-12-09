using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Data.Models
{
    public class GetSetting
    {
        public static string String(string settingName)
        {
            string returnValue = "";
            using (var db = new TimeMFGEntities())
            {
                var setting = db.Settings.Single(x => x.Name == settingName);
                if (setting != null) returnValue = setting.Value;
                else throw new ConfigurationErrorsException();
            }
            return returnValue;
        }

        public static int Int(string settingName)
        {
            int returnValue = 0;
            using (var db = new TimeMFGEntities())
            {
                var setting = db.Settings.Single(x => x.Name == settingName);
                if (setting != null && setting.Int == true) returnValue = Convert.ToInt32(setting.Value);
                else throw new ConfigurationErrorsException();
            }
            return returnValue;
        }
    }
}
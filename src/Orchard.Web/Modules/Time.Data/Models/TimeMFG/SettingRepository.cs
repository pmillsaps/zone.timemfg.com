using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Time.Data.EntityModels.TimeMFG;

namespace Time.Data.Models.TimeMFG
{

    public class SettingRepository : IDisposable
    {
        private TimeMFGEntities db = new TimeMFGEntities();

        public string GetSettings(string name)
        {
            string result = String.Empty;
            var setting = db.Settings.FirstOrDefault(x => x.Name == name);
            if (setting != null && setting.String) result = setting.Value;
            return result;
        }

        //public int GetSettings(string name)
        //{
        //    int result = 0;
        //    var setting = db.Settings.FirstOrDefault(x => x.Name == name);
        //    if (setting != null && setting.Int) result = Convert.ToInt16(setting.Value);
        //    return result;
        //}

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}
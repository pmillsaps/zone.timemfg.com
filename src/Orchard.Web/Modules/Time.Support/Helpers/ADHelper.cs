using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;

namespace Time.Support.Helpers
{
    public static class ADHelper
    {
        public static List<string> GetGroupNames(string userName)
        {
            List<string> result = new List<string>();

            if (userName.ToUpper().StartsWith("TIME"))
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "timemfg.prv"))
                {
                    using (PrincipalSearchResult<Principal> src = UserPrincipal.FindByIdentity(pc, userName).GetGroups(pc))
                    {
                        src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                    }
                }
            }
            else
            {
                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, "versaliftsouthwest.prv"))
                {
                    using (PrincipalSearchResult<Principal> src = UserPrincipal.FindByIdentity(pc, userName).GetGroups(pc))
                    {
                        src.ToList().ForEach(sr => result.Add(sr.SamAccountName));
                    }
                }
            }

            return result;
        }
    }
}
using System.Web;
using System.Web.Mvc;
using GAPT.CustomActionFilters;

namespace GAPT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
            filters.Add(new RequireHttpsAttribute());
            //filters.Add(new SessionTimeoutAttribute());
        }
    }
}

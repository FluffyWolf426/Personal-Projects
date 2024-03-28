using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using WestWindSystem.DAL;
using WestWindSystem.Entities;
#endregion

namespace WestWindSystem.BLL
{
    public class RegionServices
    {
        #region setup the context connection variable and class constructor
        //this is connection variable to be used within this class
        private readonly WestWindContext _context;

        //constructor to be used in the creation of the instance of this class
        //the registered reference for the context connection (database connection)
        //  will be passed from the IServiceCollection registered services
        internal RegionServices(WestWindContext registeredcontext)
        {
            _context = registeredcontext;
        }
        #endregion

        //Services

        //Queries

        public List<Region> Region_GetList()
        {
            IEnumerable<Region> info = _context.Regions.OrderBy(r => r.RegionDescription);

            return info.ToList();
        }
        public Region Region_GetByID(int regionid)
        {
            //validation??? none
            Region info = _context.Regions.Where(r => r.RegionID == regionid).FirstOrDefault();
            //Regions info = Regions.FirstOrDefault(r => r.RegionID == regionid );
            return info;
        }
    }
}

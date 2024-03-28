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
    public class ShipmentServices
    {
        #region setup the context connection variable and class constructor
        //this is connection variable to be used within this class
        private readonly WestWindContext _context;

        //constructor to be used in the creation of the instance of this class
        //the registered reference for the context connection (database connection)
        //  will be passed from the IServiceCollection registered services
        internal ShipmentServices(WestWindContext registeredcontext)
        {
            _context = registeredcontext;
        }
        #endregion

        //Services

        //Queries

        //query to retrieve a set of Shipments within a date range
        public List<Shipment> Shipment_GetByYearMonth(DateTime yearmontharg)
        {
            //138 records using 2018-01-01
            IEnumerable<Shipment> info = _context.Shipments
                                            .Where(s => s.ShippedDate.Year == yearmontharg.Year
                                                    && s.ShippedDate.Month == yearmontharg.Month)
                                            .OrderBy(s => s.ShippedDate);
            return info.ToList();
        }
    }
}

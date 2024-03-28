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
    public class ShipperServices
    {
        #region setup the context connection variable and class constructor
        //this is connection variable to be used within this class
        private readonly WestWindContext _context;

        //constructor to be used in the creation of the instance of this class
        //the registered reference for the context connection (database connection)
        //  will be passed from the IServiceCollection registered services
        internal ShipperServices(WestWindContext registeredcontext)
        {
            _context = registeredcontext;
        }
        #endregion

        //Services

        //Queries

        public List<Shipper> Shipper_GetList()
        {
            //on this Where we are asking for ONLY shippers who
            //  have been used to ship a shipment
            //we are comparing two lists against each other and
            //  accepting only items that appear in both lists

            //query actions
            //a) get the shipper list
            //b) compare the shipper list to shipments
            //c) take the compare results and remove duplicate (.Distinct() method)
            //d) order the unique list
            IEnumerable<Shipper> info = _context.Shippers
                        .Where(outShipper => _context.Shipments.Any(innerShipment =>
                                        innerShipment.ShipVia == outShipper.ShipperID))
                        .Distinct()
                        .OrderBy(x => x.CompanyName);
            return info.ToList();
        }
    }
}

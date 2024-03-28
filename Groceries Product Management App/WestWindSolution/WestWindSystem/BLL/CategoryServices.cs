﻿using System;
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
    public class CategoryServices
    {
        #region setup the context connection variable and class constructor
        private readonly WestWindContext _context;

        //constructor to be used in the creation of the instance of this class
        //the registered reference for the context connection (database connection)
        //  will be passed from the IServiceCollection registered services
        internal CategoryServices(WestWindContext registeredcontext)
        {
            _context = registeredcontext;
        }
        #endregion

        //Queries

        public List<Category> Category_GetList()
        {
            return _context.Categories.OrderBy(c => c.CategoryName).ToList();
        }
    }
}

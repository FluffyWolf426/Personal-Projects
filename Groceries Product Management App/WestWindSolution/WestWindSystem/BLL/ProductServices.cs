using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using WestWindSystem.DAL;
using WestWindSystem.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
#endregion

namespace WestWindSystem.BLL
{
    public class ProductServices
    {
        #region setup the context connection variable and class constructor
        private readonly WestWindContext _context;

        //constructor to be used in the creation of the instance of this class
        //the registered reference for the context connection (database connection)
        //  will be passed from the IServiceCollection registered services
        internal ProductServices(WestWindContext registeredcontext)
        {
            _context = registeredcontext;
        }
        #endregion

        //Queries

        public Product Product_GetByID(int productid)
        {
            return _context.Products
               // .AsNoTracking()
                        .FirstOrDefault(p => p.ProductID == productid)
                        ;

        }
        public List<Product> Product_GetByCategory(int categoryid)
        {
            return _context.Products
                        .Where(p => p.CategoryID == categoryid)
                        .OrderBy(p => p.ProductName)
                        // .AsNoTracking()
                        .ToList();

        }

        public List<Product> Product_GetByProductName(string partialname)
        {
            if (string.IsNullOrWhiteSpace(partialname))
            {
                throw new ArgumentNullException("Missing product name to search");
            }
            return _context.Products
                        .Where(p => p.ProductName.Contains(partialname))
                        .OrderBy(p => p.ProductName)
                       //  .AsNoTracking()
                        .ToList();

        }
        // CRUD ADD, UPDATE, DELETE

        //ADD
        //Adding a record to your database MAY require your to
        //  verify that the data does not already exist on the database
        //  verify that the incoming data is valid (do not trust the front end)

        //these actions are referred to as Business Logic (Business Rules)

        //always check if you have data actually coming into your service

        //How to do the validation in your service : C# conditional logic
        //                                           Linq query

        //Example: for this product insertion we will verify that there is
        //          no product record on the database with the same name
        //          and same quantity per unit from the same supplier
        //          if so, throw an exception

        //other key points
        // you must know whether the table has an identity pkey or not
        //Why?
        //  if the table pkey is NOT an identity, then, you MUST ensure
        //      that the incoming instance of the entity HAS a pkey value
        //      and that the value does NOT already exist
        //  if the table pkey is an identity, then, the database WILL generate
        //      the pkey value and make it accessable to you AFTER the data
        //      has been commited on the database

        // for our Add, the Product table has an identity pkey
        //  we will retrieve and return the identity value AFTER commiting
        //      the new record
        //  THIS ACTION IS OPTIONAL AND A DESIGN CONSIDERATION  

        public int Product_Add(Product item)
        {
            //did the service receive data
            if (item == null)
            {
                throw new ArgumentNullException("No data supplied for the new product");
            }

            //Business Rule
            Product exists = _context.Products
                                .Where(p => p.ProductName.ToUpper().Equals(item.ProductName.ToUpper())
                                         && p.QuantityPerUnit.ToUpper().Equals(item.QuantityPerUnit.ToUpper())
                                         && p.SupplierID == item.SupplierID)
                                 .AsNoTracking()
                                .FirstOrDefault();
            if (exists != null)
            {
                throw new ArgumentException($"Product {item.ProductName} from {exists.Supplier.CompanyName} already on file");
            }

            //any other validation / business rules

            //at this point in the process we can assume that the data is valid
            //we can now add the data to the database

            //there are two steps in the adding of your record to the database
            //these are called Staging and Commit

            //Staging: staging is the act of placing your data into local memory
            //          for eventual processing on the database
            // a) know the DbSet
            // b) know the act
            // c) indicate the data involve

            //IMPORTANT: the data is in LOCAL MEMORY
            //           the data has NOT!!!! yet been sent to the database
            //THERFORE: at this time, there is NO!!!! primary key value
            //          on the intance (except for the default of the datatype)
            //UNLESS: You have placed a value in the NON_IDENTITY key field

            _context.Products.Add(item);

            //Commit the LOCAL STAGED data to the database

            //IF there are validation annotation on your Entity defintion
            //  that will be EXECUTED during the saving of your staged actions
            //  to the database
            //So, if you violate a validation annotation, the data DOES NOT
            //  go to the database and the system throws an EXception
            //IF a validation (constraint) on the database fails, then the
            //  database will throw an error and the data will not be
            //  saved on the database (Rollback)

            _context.SaveChanges();

            //AFTER the successful commit to the database, your new product
            //  will have a generated pkey value
            //You can NOW access that value and return it

            return item.ProductID;
        }

        //update

        public int Product_Update(Product item)
        {
            //did the service receive data
            if (item == null)
            {
                throw new ArgumentNullException("No data supplied to update product");
            }

            //does the record being updated exist
            //.Any uses the delegagte and returns either a true or false
            bool isThere = _context.Products.Any(p => p.ProductID==item.ProductID);

            if (!isThere)
            {
                throw new ArgumentException($"{item.ProductName} (id:{item.ProductID})" +
                    $" is no longer on file.");
            }

            //Business Rule
            //in the update, the original product CAN have the
            //  incoming name, size and supplier
            //what CANNOT exist is the name, size and supplier
            //  on some other product
            //add another condition to the where
            //  check only other products
            Product exists = _context.Products
                                .Where(p => p.ProductName.ToUpper().Equals(item.ProductName.ToUpper())
                                         && p.QuantityPerUnit.ToUpper().Equals(item.QuantityPerUnit.ToUpper())
                                         && p.SupplierID == item.SupplierID
                                         && p.ProductID != item.ProductID)
                                .FirstOrDefault();
            if (exists != null)
            {
                throw new ArgumentException($"Product {item.ProductName} from {exists.Supplier.CompanyName} already on file. Unable to update.");
            }

            //any other validation / business rules

            //at this point in the process we can assume that the data is valid
            //we can now add the data to the database

            //Staging
            //recommended by MS Doc
            EntityEntry<Product> updating = _context.Entry(item);
            updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            //Commit the LOCAL STAGED data to the database
            //the return value of an update is the number of rows affected

            return _context.SaveChanges();
        }

        //delete

        //there are two types of deleting: physical and logical
        //whether you have a physical or logically delete is determined WHEN
        //  the database is designed

        //physical delete
        //you physically remove the record from the database
        //IF this record has any child record possible (parent - child relationship)
        //   you cannot just delete the parent record as there may be child records
        //   on the database
        //you CAN remove the record from the database IF you check to see
        //   if there are any child records and if not, you will be allow to delete
        //   the parent record without sql putting up a fuss.
        //HOWEVER: this is a design decision. if the design say NO delete then it does
        //   not matter if there are or are not child records.

        //Staging
        //EntityEntry<Product> deleting = _context.Entry(item);
        //deleting.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

        //logical delete
        //you update the record setting the designed attribute which indicates the
        //      record as logically deleted
        //For the Product record we have a designed attribute called Discontinued
        //This field will be altered to indicate the product is discontinued
        //You can have the responsibility for setting this value done by the user
        //   but it is better to do the action within your logic in case the
        //   user forgot to change the field on the form

        //Staging
        //EntityEntry<Product> updating = _context.Entry(item);
        //updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

        public int Product_LogicalDelete(Product item)
        {
            //did the service receive data
            if (item == null)
            {
                throw new ArgumentNullException("No data supplied to discontinued the product");
            }

            //logical Delete
            //does the record being updated exist
            //.Any uses the delegagte and returns either a true or false
            Product exists = _context.Products
                .FirstOrDefault(p => p.ProductID==item.ProductID);

            if (exists == null)
            {
                throw new ArgumentException($"{item.ProductName} (id:{item.ProductID})" +
                    $" is no longer on file.");
            }

            //at this point in the process we can assume that the data is valid
            //we can now add the data to the database

            //What if, we wish to handle BOTH logical delete AND a logical activate
            //test the current state of your record and reverse

            if (exists.Discontinued)
            {
                //set the logical delete field to its activated state

                exists.Discontinued = false;
            }
            else
            {
                //set the logical delete field to its deleted state (discontinued)

                exists.Discontinued = true;
            }

           

            //Staging
            //recommended by MS Doc
            //Due to the fact that the only field that should be changed
            //  is the "delete" field on the record
            //NO other fields of the record should be altered
            //Altering field should be the responsibility of the Update
            //NOTE: this could change depending on the design of the system
            EntityEntry<Product> updating = _context.Entry(exists);
            updating.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            //Commit the LOCAL STAGED data to the database
            //the return value of an update is the number of rows affected

            return _context.SaveChanges();
        }

        public int Product_PhysicalDelete(Product item)
        {
            //did the service receive data
            if (item == null)
            {
                throw new ArgumentNullException("No data supplied to remove the product");
            }

            //logical Delete
            //does the record being updated exist
            //.Any uses the delegagte and returns either a true or false
            /*Product exists = _context.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == item.ProductID);*/

            Product exists = _context.Products.FirstOrDefault(p => p.ProductID==item.ProductID);


            if (exists == null)
            {
                throw new ArgumentException($"{item.ProductName} (id:{item.ProductID})" +
                    $" is no longer on file.");
            }

            //at this point in the process we can assume that the data is valid
            //we can now add the data to the database


            //Staging
            //recommended by MS Doc
            //Depending on the system design you many need to check if the record has
            //  any "children" on the database
            //If so, AND you are not doing a cascade delete, then you cannot remove the
            //  record from the database

            int existingChildren = exists.ManifestItems.Count();
            existingChildren += exists.OrderDetails.Count();

            if (existingChildren > 0)
            {
                throw new ArgumentException($"{item.ProductName} (id:{item.ProductID})" +
                   $" can not be removed. Related data exists.");
            }

            EntityEntry<Product> updating = _context.Entry(exists);
            updating.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            //Commit the LOCAL STAGED data to the database
            //the return value of an update is the number of rows affected

            return _context.SaveChanges();
        }

    }
}

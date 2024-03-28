// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : James Thompson
// Created          : 06-01-2023
//
// Last Modified By : James Thompson
// Last Modified On : 06-19-2023
// ***********************************************************************
// <copyright file="CustomerService.cs" project="HogWildSystem">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#nullable disable
using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;

namespace HogWildSystem.BLL
{
    /// <summary>
    /// Class CustomerService.
    /// </summary>
    public class CustomerService
    {
        #region Fields
        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerService" /> class.
        /// </summary>
        /// <param name="hogWildContext">The hog wild context.</param>
        internal CustomerService(HogWildContext hogWildContext)
        {
            _hogWildContext = hogWildContext;
        }

        //	get customers
        /// <summary>
        /// Gets the customers.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <param name="phone">The phone.</param>
        /// <returns>List&lt;CustomerSearchView&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide either a last name and/or phone number</exception>
        public List<CustomerSearchView> GetCustomers(string lastName, string phone)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	both last name phone number cannot be empty
            //		rule: 	RemoveFromViewFlag must be false

            if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentNullException("Please provide either a last name and/or phone number");
            }

            //  need to update parameters so we are not searching on an empty value.
            //	Otherwise, an empty string will return all records
            if (string.IsNullOrWhiteSpace(lastName))
            {
                lastName = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrWhiteSpace(phone))
            {
                phone = Guid.NewGuid().ToString();
            }

            //  return customers that meet our criteria
            return _hogWildContext.Customers
                .Where(x => (x.LastName.Contains(lastName)
                             || x.Phone.Contains(phone))
                            && !x.RemoveFromViewFlag)
                .Select(x => new CustomerSearchView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    City = x.City,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    TotalSales = x.Invoices.Sum(x => x.SubTotal + x.Tax)
                }).OrderBy(x => x.LastName)
                .ToList();
        }

        //	get customer
        /// <summary>
        /// Gets the customer.
        /// </summary>
        /// <param name="customerID">The customer identifier.</param>
        /// <returns>CustomerEditView.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide a customer</exception>
        public CustomerEditView GetCustomer(int customerID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	customerID must be valid 

            if (customerID == 0)
            {
                throw new ArgumentNullException("Please provide a customer");
            }

            return _hogWildContext.Customers
                .Where(x => (x.CustomerID == customerID
                             && x.RemoveFromViewFlag == false))
                .Select(x => new CustomerEditView
                {
                    CustomerID = x.CustomerID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address1 = x.Address1,
                    Address2 = x.Address2,
                    City = x.City,
                    ProvStateID = x.ProvStateID,
                    CountryID = x.CountryID,
                    PostalCode = x.PostalCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    StatusID = x.StatusID,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }

        /// <summary>
        /// Adds the edit customer.
        /// </summary>
        /// <param name="editCustomer">The edit customer.</param>
        /// <exception cref="System.ArgumentNullException">No customer was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add or edit customer. Please check error message(s)</exception>
        public CustomerEditView AddEditCustomer(CustomerEditView editCustomer)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data

            //		rule:	customer cannot be null	
            if (editCustomer == null)
            {
                throw new ArgumentNullException("No customer was supply");
            }

            //		rule: 	first name, last name, phone number and email are required (not empty)
            if (string.IsNullOrWhiteSpace(editCustomer.FirstName))
            {
                errorList.Add(new Exception("First name is required"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.LastName))
            {
                errorList.Add(new Exception("Last name is required"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.Email))
            {
                errorList.Add(new Exception("Email is required"));
            }

            if (string.IsNullOrWhiteSpace(editCustomer.Phone))
            {
                errorList.Add(new Exception("Phone is required"));
            }

            //		rule: 	first name, last name and phone number cannot be duplicated (found more than once)
            if (editCustomer.CustomerID == 0)
            {
                bool customerExist = _hogWildContext.Customers
                                .Where(x => x.FirstName == editCustomer.FirstName
                                            && x.LastName == editCustomer.LastName
                                            && x.Phone == editCustomer.Phone)
                                .Any();

                if (customerExist)
                {
                    errorList.Add(new Exception("Customer already exist in the database and cannot be enter again"));
                }
            }
            #endregion

            Customer customer =
                _hogWildContext.Customers.Where(x => x.CustomerID == editCustomer.CustomerID)
                    .Select(x => x).FirstOrDefault();

            //  new customer
            if (customer == null)
            {
                customer = new Customer();
            }

            //  Please review AddEditEmployee.cs for a simpler methods using the pattern below.
            //	DataHelper dataHelper = new DataHelper();
            //	dataHelper.CopyItemPropertyValues(editCustomer, customer, "CustomerID");
            customer.FirstName = editCustomer.FirstName;
            customer.LastName = editCustomer.LastName;
            customer.Address1 = editCustomer.Address1;
            customer.Address2 = editCustomer.Address2;
            customer.City = editCustomer.City;
            customer.ProvStateID = editCustomer.ProvStateID;
            customer.CountryID = editCustomer.CountryID;
            customer.PostalCode = editCustomer.PostalCode;
            customer.Email = editCustomer.Email;
            customer.Phone = editCustomer.Phone;
            customer.StatusID = editCustomer.StatusID;
            customer.RemoveFromViewFlag = editCustomer.RemoveFromViewFlag;

            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add or edit customer. Please check error message(s)", errorList);
            }
            else
            {
                //  new customer
                if (customer.CustomerID == 0)
                    _hogWildContext.Customers.Add(customer);
                else
                    _hogWildContext.Customers.Update(customer);
                _hogWildContext.SaveChanges();
            }
            //  can return current editCustomer
            editCustomer.CustomerID = customer.CustomerID;
            return editCustomer;
        }

        /// <summary>
        /// Gets the full name of the customer.
        /// </summary>
        /// <param name="customerID">The customer identifier.</param>
        /// <returns>System.String.</returns>
        public string GetCustomerFullName(int customerID)
        {
            return _hogWildContext.Customers
                .Where(x => x.CustomerID == customerID)
                .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
        }
    }
}

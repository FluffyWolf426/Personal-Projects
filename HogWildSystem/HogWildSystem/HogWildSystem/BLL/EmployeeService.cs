// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : JTHOMPSON
// Created          : 06-05-2023
//
// Last Modified By : JTHOMPSON
// Last Modified On : 08-23-2023
// ***********************************************************************
// <copyright file="EmployeeService.cs" company="HogWildSystem">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.ViewModels;

namespace HogWildSystem.BLL
{
    /// <summary>
    /// Class EmployeeService.
    /// </summary>
    public class EmployeeService
    {
        #region Fields
        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext? _hogWildContext;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="hogWildContext">The hog wild context.</param>
        internal EmployeeService(HogWildContext hogWildContext)
        {
            _hogWildContext = hogWildContext;
        }

        //	get employees
        /// <summary>
        /// Gets the employees.
        /// </summary>
        /// <param name="lastName">The last name.</param>
        /// <param name="phone">The phone.</param>
        /// <returns>List&lt;EmployeeSearchView&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide either a last name and/or phone number</exception>
        public List<EmployeeSearchView> GetEmployees(string lastName, string phone)
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

            //  return employees that meet our criteria
            return _hogWildContext.Employees
                .Where(x => (x.LastName.Contains(lastName)
                             || x.Phone.Contains(phone))
                            && !x.RemoveFromViewFlag)
                .Select(x => new EmployeeSearchView
                {
                    EmployeeID = x.EmployeeID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    City = x.City,
                    Phone = x.Phone,
                    Email = x.Email,
                    RoleID = x.RoleID
                }).OrderBy(x => x.LastName)
                .ToList();
        }

        //	get employee
        /// <summary>
        /// Gets the employee.
        /// </summary>
        /// <param name="employeeID">The employee identifier.</param>
        /// <returns>EmployeeEditView.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide a employee</exception>
        public EmployeeEditView GetEmployee(int employeeID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	employeeID must be valid 

            if (employeeID == 0)
            {
                throw new ArgumentNullException("Please provide a employee");
            }

            return _hogWildContext.Employees
                .Where(x => (x.EmployeeID == employeeID
                             && x.RemoveFromViewFlag == false))
                .Select(x => new EmployeeEditView
                {
                    EmployeeID = x.EmployeeID,
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
                    RoleID = x.RoleID,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }

        /// <summary>
        /// Adds the edit employee.
        /// </summary>
        /// <param name="editEmployee">The edit employee.</param>
        /// <exception cref="System.ArgumentNullException">No employee was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add or edit employee. Please check error message(s)</exception>
        public void AddEditEmployee(EmployeeEditView editEmployee)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data

            //		rule:	employee cannot be null	
            if (editEmployee == null)
            {
                throw new ArgumentNullException("No employee was supply");
            }

            //		rule: 	first name, last name, phone number and email are required (not empty)
            if (string.IsNullOrWhiteSpace(editEmployee.FirstName))
            {
                errorList.Add(new Exception("First name is required"));
            }

            if (string.IsNullOrWhiteSpace(editEmployee.LastName))
            {
                errorList.Add(new Exception("Last name is required"));
            }

            if (string.IsNullOrWhiteSpace(editEmployee.Email))
            {
                errorList.Add(new Exception("Email is required"));
            }

            if (string.IsNullOrWhiteSpace(editEmployee.Phone))
            {
                errorList.Add(new Exception("Phone is required"));
            }

            //		rule: 	first name, last name and phone number cannot be duplicated (found more than once)
            if (editEmployee.EmployeeID == 0)
            {
                bool employeeExist = _hogWildContext.Employees
                                .Where(x => x.FirstName == editEmployee.FirstName
                                            && x.LastName == editEmployee.LastName
                                            && x.Phone == editEmployee.Phone)
                                .Any();

                if (employeeExist)
                {
                    errorList.Add(new Exception("Employee already exist in the database and cannot be enter again"));
                }
            }
            #endregion

            Employee employee =
                _hogWildContext.Employees.Where(x => x.EmployeeID == editEmployee.EmployeeID)
                    .Select(x => x).FirstOrDefault();

            //  new employee
            if (employee == null)
            {
                employee = new Employee();
            }

            //  Please review AddEditEmployee.cs for a simpler methods using the pattern below.
            //	DataHelper dataHelper = new DataHelper();
            //	dataHelper.CopyItemPropertyValues(editEmployee, employee, "EmployeeID");
            employee.FirstName = editEmployee.FirstName;
            employee.LastName = editEmployee.LastName;
            employee.Address1 = editEmployee.Address1;
            employee.Address2 = editEmployee.Address2;
            employee.City = editEmployee.City;
            employee.ProvStateID = editEmployee.ProvStateID;
            employee.CountryID = editEmployee.CountryID;
            employee.PostalCode = editEmployee.PostalCode;
            employee.Email = editEmployee.Email;
            employee.Phone = editEmployee.Phone;
            employee.RoleID = editEmployee.RoleID;
            employee.RemoveFromViewFlag = editEmployee.RemoveFromViewFlag;

            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add or edit employee. Please check error message(s)", errorList);
            }
            else
            {
                //  new employee
                if (employee.EmployeeID == 0)
                    _hogWildContext.Employees.Add(employee);
                else
                    _hogWildContext.Employees.Update(employee);
                _hogWildContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the full name of the employee.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns>System.String.</returns>
        public string GetEmployeeFullName(int employeeId)
        {
            {
                return _hogWildContext.Employees
                    .Where(x => x.EmployeeID == employeeId)
                    .Select(x => $"{x.FirstName} {x.LastName}").FirstOrDefault();
            }
        }
    }
}
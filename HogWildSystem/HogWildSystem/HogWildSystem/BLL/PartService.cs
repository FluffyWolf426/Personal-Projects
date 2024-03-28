// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : JTHOMPSON
// Created          : 08-21-2023
//
// Last Modified By : JTHOMPSON
// Last Modified On : 08-22-2023
// ***********************************************************************
// <copyright file="PartService.cs" company="HogWildSystem">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.HelperClasses;
using HogWildSystem.Paginator;
using HogWildSystem.ViewModels;

namespace HogWildSystem.BLL
{
    /// <summary>
    /// Class PartService.
    /// </summary>
    public class PartService
    {
        #region Fields
        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext _hogWildContext;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PartService" /> class.
        /// </summary>
        /// <param name="hogWildContext">The hog wild context.</param>
        internal PartService(HogWildContext hogWildContext)
        {
            _hogWildContext = hogWildContext;
        }

        /// <summary>
        /// Gets the parts.
        /// </summary>
        /// <param name="partCategoryID">The part category identifier.</param>
        /// <param name="description">The description.</param>
        /// <param name="page">The page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>Task&lt;PagedResult&lt;PartView&gt;&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide either a category and/or description</exception>
        public Task<PagedResult<PartView>> GetParts(int partCategoryID, string description, int page, int pageSize, string sortColumn, string direction)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	both part id must be valid and/or description  cannot be empty
            //		rule: 	RemoveFromViewFlag must be false

            if (partCategoryID == 0 && string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException("Please provide either a category and/or description");
            }

            //  need to update parameters so we are not searching on an empty value.
            //	this will return all records
            Guid tempGuild = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(description))
            {
                description = tempGuild.ToString();
            }
            //  Task.FromResult() creates a finished Task that holds a value in its Result property
            return Task.FromResult(_hogWildContext.Parts
                .Where(x => description.Length > 0 && description != tempGuild.ToString() && partCategoryID > 0 
                            ? (x.Description.Contains(description) && x.PartCategoryID == partCategoryID) 
                            : (x.Description.Contains(description) || x.PartCategoryID == partCategoryID)
                                                                      && !x.RemoveFromViewFlag)
                .Select(x => new PartView
                {
                    PartID = x.PartID,
                    PartCategoryID = x.PartCategoryID,
                    CategoryName = x.PartCategory.Name,
                    Description = x.Description,
                    Cost = x.Cost,
                    Price = x.Price,
                    ROL = x.ROL,
                    QOH = x.QOH,
                    Taxable = x.Taxable,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).OrderBy(x => x.Description)
                .AsQueryable()
                .OrderBy(sortColumn, direction)
                .ToPagedResult(page, pageSize)); 
        }

        //	get part
        /// <summary>
        /// Gets the part.
        /// </summary>
        /// <param name="partID">The part identifier.</param>
        /// <returns>PartEditView.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide a part</exception>
        public PartView GetPart(int partID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	partID must be valid 

            if (partID == 0)
            {
                throw new ArgumentNullException("Please provide a part");
            }

            return _hogWildContext.Parts
                .Where(x => (x.PartID == partID
                             && !x.RemoveFromViewFlag))
                .Select(x => new PartView
                {
                    PartID = x.PartID,
                    PartCategoryID = x.PartCategoryID,
                    Description = x.Description,
                    Cost = x.Cost,
                    Price = x.Price,
                    ROL = x.ROL,
                    QOH = x.QOH,
                    Taxable = x.Taxable,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).FirstOrDefault();
        }

        /// <summary>
        /// Adds the edit part.
        /// </summary>
        /// <param name="editPart">The edit part.</param>
        /// <exception cref="System.ArgumentNullException">No part was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add or edit part. Please check error message(s)</exception>
        public void AddEditPart(PartView editPart)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	part cannot be null	
            //		rule: 	partCategoryID and description are required
            if (editPart == null)
            {
                throw new ArgumentNullException("No part was supply");
            }

            if (editPart.PartCategoryID == 0)
            {
                errorList.Add(new Exception("Part Category is required"));
            }

            if (string.IsNullOrWhiteSpace(editPart.Description))
            {
                errorList.Add(new Exception("Description is required"));
            }
            //  cost cannot be less than zero
            if (editPart.Cost < 0)
            {
                errorList.Add(new Exception("Cost cannot be less than zero!"));
            }
            //  price cannot be less than zero
            if (editPart.Price < 0)
            {
                errorList.Add(new Exception("Price cannot be less than zero!"));
            }
            //  rol cannot be less than zero
            if (editPart.ROL < 0)
            {
                errorList.Add(new Exception("Reorder Level cannot be less than zero!"));
            }
            //  qoh cannot be less than zero
            if (editPart.QOH < 0)
            {
                errorList.Add(new Exception("Quantity on Hand cannot be less than zero!"));
            }

            //		rule: 	parts cannot be duplicated (found more than once)
            if (editPart.PartID == 0)
            {
                bool partExist = _hogWildContext.Parts
                                .Where(x => x.PartCategoryID == editPart.PartCategoryID
                                            && x.Description == editPart.Description)
                                .Any();

                if (partExist)
                {
                    errorList.Add(new Exception("Part already exist in the database and cannot be enter again"));
                }
            }
            #endregion

            Part part =
                _hogWildContext.Parts.Where(x => x.PartID == editPart.PartID)
                    .Select(x => x).FirstOrDefault();

            //  new part
            if (part == null)
            {
                part = new Part();
            }

            //  the following is a simple method to copy all of the fields from the 
            //		employee view model to the employee object
            DataHelper dataHelper = new DataHelper();
            dataHelper.CopyItemPropertyValues(editPart, part, "PartID");
            part.RemoveFromViewFlag = editPart.RemoveFromViewFlag;

            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add or edit part. Please check error message(s)", errorList);
            }
            else
            {
                //  new part
                if (part.PartID == 0)
                    _hogWildContext.Parts.Add(part);
                else
                    _hogWildContext.Parts.Update(part);
                _hogWildContext.SaveChanges();
            }
        }

    }
}

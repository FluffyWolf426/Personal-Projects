// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : JTHOMPSON
// Created          : 08-21-2023
//
// Last Modified By : JTHOMPSON
// Last Modified On : 08-23-2023
// ***********************************************************************
// <copyright file="CategoryLookupService.cs" company="HogWildSystem">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.DAL;
using HogWildSystem.Entities;
using HogWildSystem.HelperClasses;
using HogWildSystem.ViewModels;

namespace HogWildSystem.BLL
{
    /// <summary>
    /// Class CategoryLookupService.
    /// </summary>
    public class CategoryLookupService
    {
        #region Fields
        /// <summary>
        /// The hog wild context
        /// </summary>
        private readonly HogWildContext? _hogWildContext;
        #endregion
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryLookupService"/> class.
        /// </summary>
        /// <param name="hogWildContext">The hog wild context.</param>
        internal CategoryLookupService(HogWildContext hogWildContext)
        {
            _hogWildContext = hogWildContext;
        }


        #region Category
        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns>System.Nullable&lt;List&lt;CategoryView&gt;&gt;.</returns>
        public List<CategoryView>? GetCategories()
        {
            return _hogWildContext.Categories
                .Where(x => x.RemoveFromViewFlag == false)
                .OrderBy(x => x.CategoryName)
                .Select(x => new CategoryView
                    {
                        CategoryID = x.CategoryID,
                        CategoryName = x.CategoryName,
                        RemoveFromViewFlag = x.RemoveFromViewFlag
                    }
                )
                .ToList();
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <param name="categoryId">The category identifier.</param>
        /// <returns>CategoryView.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide a category</exception>
        public CategoryView GetCategory(int categoryId)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	categoryID must be valid 

            if (categoryId == 0)
            {
                throw new ArgumentNullException("Please provide a category");
            }

            return _hogWildContext.Categories
                .Where(x => x.CategoryID == categoryId
                            && x.RemoveFromViewFlag == false)
                .Select(x => new CategoryView
                    {
                        CategoryID = x.CategoryID,
                        CategoryName = x.CategoryName,
                        RemoveFromViewFlag = x.RemoveFromViewFlag
                    }
                ).FirstOrDefault();
        }

        /// <summary>
        /// Adds the edit category.
        /// </summary>
        /// <param name="categoryView">The category view.</param>
        /// <exception cref="System.ArgumentNullException">No category was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add or edit category. Please check error message(s)</exception>
        public void AddEditCategory(CategoryView categoryView)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	categoryView cannot be null	
            //		rule: 	category name is required
            if (categoryView == null)
            {
                throw new ArgumentNullException("No category was supply");
            }


            if (string.IsNullOrWhiteSpace(categoryView.CategoryName))
            {
                errorList.Add(new Exception("Category name is required"));
            }

            //		rule:	category cannot be duplicated (found more than once)
            if (categoryView.CategoryID == 0)
            {
                bool categoryExist = _hogWildContext.Categories
                                .Where(x => x.CategoryName == categoryView.CategoryName)
                                .Any();

                if (categoryExist)
                {
                    errorList.Add(new Exception("Category already exist in the database and cannot be enter again"));
                }
            }
            #endregion

            Category category =
                _hogWildContext.Categories.Where(x => x.CategoryID == categoryView.CategoryID)
                    .Select(x => x).FirstOrDefault();

            //  new category
            if (category == null)
            {
                category = new Category();
            }
            //  the following is a simple method to copy all of the fields from the 
            //		category view model to the category object
            DataHelper dataHelper = new DataHelper();
            dataHelper.CopyItemPropertyValues(categoryView, category, "CategoryID");
            category.RemoveFromViewFlag = categoryView.RemoveFromViewFlag;

            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add or edit category. Please check error message(s)", errorList);
            }
            else
            {
                //  new category
                if (category.CategoryID == 0)
                    _hogWildContext.Categories.Add(category);
                else
                    _hogWildContext.Categories.Update(category);
                _hogWildContext.SaveChanges();
            }
        }
        #endregion

        #region Lookup
        /// <summary>
        /// Gets the lookups.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns>List&lt;LookupView&gt;.</returns>
        public List<LookupView> GetLookups(string categoryName)
        {
            return _hogWildContext.Lookups
                .Where(x => x.Category.CategoryName == categoryName)
                .OrderBy(x => x.Name)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupID,
                    CategoryID = x.CategoryID,
                    Name = x.Name,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).ToList();
        }

        /// <summary>
        /// Gets the lookups.
        /// </summary>
        /// <param name="categoryID">The category identifier.</param>
        /// <returns>List&lt;LookupView&gt;.</returns>
        public List<LookupView> GetLookups(int categoryID)
        {
            return _hogWildContext.Lookups
                .Where(x => x.CategoryID == categoryID)
                .OrderBy(x => x.Name)
                .Select(x => new LookupView
                {
                    LookupID = x.LookupID,
                    CategoryID = x.CategoryID,
                    Name = x.Name,
                    RemoveFromViewFlag = x.RemoveFromViewFlag
                }).ToList();
        }

        /// <summary>
        /// Gets the lookup.
        /// </summary>
        /// <param name="lookupID">The lookup identifier.</param>
        /// <returns>LookupView.</returns>
        /// <exception cref="System.ArgumentNullException">Please provide a lookup id</exception>
        public LookupView GetLookup(int lookupID)
        {
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	lookup id must be valid 

            if (lookupID == 0)
            {
                throw new ArgumentNullException("Please provide a lookup id");
            }

            return _hogWildContext.Lookups
                .Where(x => x.LookupID == lookupID
                            && !x.RemoveFromViewFlag)
                .Select(x => new LookupView
                    {
                        CategoryID = x.CategoryID,
                        LookupID = x.LookupID,
                        Name = x.Name,
                        RemoveFromViewFlag = x.RemoveFromViewFlag
                    }
                ).FirstOrDefault();
        }

        /// <summary>
        /// Adds the lookup.
        /// </summary>
        /// <param name="lookupView">The lookup view.</param>
        /// <exception cref="System.ArgumentNullException">No lookup was supply</exception>
        /// <exception cref="System.ArgumentNullException">No category id was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add lookup. Please check error message(s)</exception>
        public void AddLookup(LookupView lookupView)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	lookupView cannot be null	
            //		rule: 	category id is required
            //		rule: 	lookup name is required

            if (lookupView == null)
            {
                throw new ArgumentNullException("No lookup was supply");
            }

            if (lookupView.CategoryID == 0)
            {
                throw new ArgumentNullException("No category id was supply");
            }

            if (string.IsNullOrWhiteSpace(lookupView.Name))
            {
                errorList.Add(new Exception("Lookup name is required"));
            }

            //		rule:	lookup name cannot be duplicated for the category id that is provided

            bool lookupExist = _hogWildContext.Lookups
                .Where(x => x.CategoryID == lookupView.CategoryID &&
                            x.Name == lookupView.Name)
                .Any();

            if (lookupExist)
            {
                errorList.Add(new Exception("Lookup already exist in the database and cannot be enter again"));
            }

            #endregion

            //  new lookup
            Lookup lookup = new Lookup()
            {
                CategoryID = lookupView.CategoryID,
                Name = lookupView.Name,
                RemoveFromViewFlag = false
            };


            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add lookup. Please check error message(s)", errorList);
            }
            else
            {
                _hogWildContext.Lookups.Add(lookup);
                _hogWildContext.SaveChanges();
            }
        }

        /// <summary>
        /// Edits the lookup.
        /// </summary>
        /// <param name="lookupView">The lookup view.</param>
        /// <exception cref="System.ArgumentNullException">No lookup was supply</exception>
        /// <exception cref="System.ArgumentNullException">No lookup id was supply</exception>
        /// <exception cref="System.AggregateException">Unable to add lookup. Please check error message(s)</exception>
        public void EditLookup(LookupView lookupView)
        {
            #region Business Logic and Parameter Exceptions
            //	create a list<Exception> to contain all discovered errors
            List<Exception> errorList = new List<Exception>();
            //  Business Rules
            //	These are processing rules that need to be satisfied
            //		for valid data
            //		rule:	lookupView cannot be null	
            //		rule: 	lookup id is required
            //		rule: 	lookup name is required

            if (lookupView == null)
            {
                throw new ArgumentNullException("No lookup was supply");
            }

            if (lookupView.LookupID == 0)
            {
                throw new ArgumentNullException("No lookup id was supply");
            }

            if (string.IsNullOrWhiteSpace(lookupView.Name))
            {
                errorList.Add(new Exception("Lookup name is required"));
            }

            //		rule:	lookup name cannot be duplicated for the caterory id that is provided

            int currrentCategoryID = _hogWildContext.Lookups
                .Where(x => x.LookupID == lookupView.LookupID)
                .Select(x => x.LookupID)
                .FirstOrDefault();

            bool lookupExist = _hogWildContext.Lookups
                .Where(x => x.CategoryID == lookupView.CategoryID
                            && x.LookupID != lookupView.LookupID  //  check that we are not just changing the letter case ie:  proper case.
                            && x.Name == lookupView.Name)
                .Any();

            if (lookupExist)
            {
                errorList.Add(new Exception("Lookup already exist in the database and cannot be enter again"));
            }

            #endregion

            //  new lookup
            Lookup lookup = _hogWildContext.Lookups
                .Where(x => x.LookupID == lookupView.LookupID)
                .Select(x => x)
                .FirstOrDefault();
            lookup.Name = lookupView.Name;
            lookup.RemoveFromViewFlag = lookupView.RemoveFromViewFlag;


            if (errorList.Count > 0)
            {
                //  we need to clear the "track changes" otherwise we leave our entity system in flux
                _hogWildContext.ChangeTracker.Clear();
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to add lookup. Please check error message(s)", errorList);
            }
            else
            {
                _hogWildContext.Lookups.Update(lookup);
                _hogWildContext.SaveChanges();
            }
        }
        #endregion
    }
}

// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 06-22-2023
//
// Last Modified By : James Thompson
// Last Modified On : 06-23-2023
// ***********************************************************************
// <copyright file="PartList.razor.cs" project="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.Paginator;
using HogWildWebApp.Shared;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;

namespace HogWildWebApp.Pages.HogWild
{
    /// <summary>
    /// Class PartList.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class PartList
    {
        #region Fields

        /// <summary>
        /// The description
        /// </summary>
        private string description;
        /// <summary>
        /// The category identifier
        /// </summary>
        private int categoryID;
        /// <summary>
        /// The partCategories
        /// </summary>
        private List<LookupView> partCategories;
        #endregion
        #region Feedback & Messages

        //  placeholder for feedback message
        /// <summary>
        /// The feedback message
        /// </summary>
        private string feedbackMessage;
        //  placeholder for error messasge
        /// <summary>
        /// The error message
        /// </summary>
        private string errorMessage;

        //  properties that return the result of the lambda action
        /// <summary>
        /// Gets a value indicating whether this instance has feedback.
        /// </summary>
        /// <value><c>true</c> if this instance has feedback; otherwise, <c>false</c>.</value>
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
        /// <summary>
        /// Gets a value indicating whether this instance has error.
        /// </summary>
        /// <value><c>true</c> if this instance has error; otherwise, <c>false</c>.</value>
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        //  used to display any collection of errors on web page
        //  whether the errors are generated locally or come from the class library
        /// <summary>
        /// The error details
        /// </summary>
        private List<string> errorDetails = new();
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the part service.
        /// </summary>
        /// <value>The part service.</value>
        [Inject]
        protected PartService PartService { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        /// <value>The navigation manager.</value>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        /// <summary>
        /// Gets or sets the category lookup service.
        /// </summary>
        /// <value>The category lookup service.</value>
        [Inject]
        protected CategoryLookupService CategoryLookupService { get; set; }

        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>The parts.</value>
        protected List<PartView> Parts { get; set; } = new();

        #endregion

        #region Paginator
        // Desired current page size
        private const int PAGE_SIZE = 10;

        // sort column used with the paginator
        protected string SortField { get; set; } = "Owner";

        // sort direction for the paginator
        protected string Direction { get; set; } = "desc";

        //  current page for the paginator
        protected int CurrentPage { get; set; } = 1;

        //paginator collection of tracks selection view
        protected PagedResult<PartView> PaginatorParts { get; set; } = new();

        private async void Sort(string column)
        {
            Direction = SortField == column ? Direction == "asc" ? "desc"
                : "asc" : "asc";
            SortField = column;
            await Search();
        }

        //  sets css class to display up and down arrows
        private string GetSortColumn(string x)
        {
            return x == SortField ? Direction == "desc" ? "desc" : "asc" : "";
        }

        #endregion

        /// <summary>
        /// On initialized as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                partCategories = CategoryLookupService.GetLookups("Part Categories");
                await InvokeAsync(StateHasChanged);
            }
            catch (ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                //  have a collection of errors
                //  each error should be place into a separate line
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                errorMessage = $"{errorMessage}Unable to search for parts";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <exception cref="System.ArgumentException">Please provide either a category and/or description</exception>
        private async Task Search()
        {
            try
            {
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  clear the part list before we do our search
                Parts.Clear();

                if (categoryID == 0 && string.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Please provide either a category and/or description");
                }

                //  search for our parts
                PaginatorParts = await PartService.GetParts(categoryID, description, CurrentPage, PAGE_SIZE, SortField, Direction);
                await InvokeAsync(StateHasChanged);

                if (PaginatorParts.Results.Length> 0)
                {
                    feedbackMessage = "Search for part(s) was successful";
                }
                else
                {
                    feedbackMessage = "No part were found for your search criteria";
                }


            }
            catch (ArgumentNullException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (ArgumentException ex)
            {
                errorMessage = BlazorHelperClass.GetInnerException(ex).Message;
            }
            catch (AggregateException ex)
            {
                //  have a collection of errors
                //  each error should be place into a separate line
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    errorMessage = $"{errorMessage}{Environment.NewLine}";
                }
                errorMessage = $"{errorMessage}Unable to search for part";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        /// <summary>
        /// Sets the sort icon.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        private string SetSortIcon(string columnName)
        {
            if (SortField != columnName)
            {
                return "fa fa-sort";
            }
            if (Direction == "asc")
            {
                return "fa fa-sort-up";
            }
            else
            {
                return "fa fa-sort-down";
            }
        }


        /// <summary>
        /// News this instance.
        /// </summary>
        private void New()
        {
            NavigationManager.NavigateTo($"/HogWild/PartEdit/0");
        }

        /// <summary>
        /// Edits the part.
        /// </summary>
        /// <param name="partId">The part identifier.</param>
        private void EditPart(int partId)
        {
            NavigationManager.NavigateTo($"/HogWild/PartEdit/{partId}");
        }
    }
}

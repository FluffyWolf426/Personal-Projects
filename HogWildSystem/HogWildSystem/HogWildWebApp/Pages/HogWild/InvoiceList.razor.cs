// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 08-22-2023
//
// Last Modified By : James Thompson
// Last Modified On : 08-22-2023
// ***********************************************************************
// <copyright file="InvoiceList.razor.cs" company="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.BLL;
using HogWildSystem.Paginator;
using HogWildSystem.ViewModels;
using HogWildWebApp.Shared;
using Microsoft.AspNetCore.Components;

namespace HogWildWebApp.Pages.HogWild
{
    /// <summary>
    /// Class InvoiceList.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class InvoiceList
    {
        #region Fields
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
        /// Gets or sets the invoice service.
        /// </summary>
        /// <value>The invoice service.</value>
        [Inject]
        protected InvoiceService InvoiceService { get; set; }

        /// <summary>
        /// Gets or sets the navigation manager.
        /// </summary>
        /// <value>The navigation manager.</value>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }


        #endregion

        #region Paginator
        //  Desired CurrentPage size
        /// <summary>
        /// The page size
        /// </summary>
        private const int PAGE_SIZE = 10;

        //  sort column used with the paginator
        /// <summary>
        /// Gets or sets the sort field.
        /// </summary>
        /// <value>The sort field.</value>
        protected string SortField { get; set; } = "Owner";

        //  sort direction for the paginator
        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        protected string Direction { get; set; } = "desc";

        //  current page for the paginator
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        protected int CurrentPage { get; set; } = 1;

        //  paginator collection of track selection view
        /// <summary>
        /// Gets or sets the paginator invoice.
        /// </summary>
        /// <value>The paginator invoice.</value>
        protected PagedResult<InvoiceView> PaginatorInvoice { get; set; } = new();

        /// <summary>
        /// Sorts the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        private async void Sort(string column)
        {
            Direction = SortField == column ? Direction == "asc" ? "desc" : "asc" : "asc";
            SortField = column;

            await GetInvoice();
        }

        // sets css class to display up and down arrows
        /// <summary>
        /// Gets the sort column.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>System.String.</returns>
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
               await GetInvoice();
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

                errorMessage = $"{errorMessage}Unable to retrieve invoices";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        /// <summary>
        /// Gets the invoice.
        /// </summary>
        private async Task GetInvoice()
        {
            
            PaginatorInvoice = await InvoiceService.GetInvoices(CurrentPage, PAGE_SIZE, SortField, Direction);        
            //Note that the following line is necessary because otherwise
            //Blazor would not recognize the state change and not refresh the UI
            await InvokeAsync(StateHasChanged);
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
        /// Creates new invoice.
        /// </summary>
        private void NewInvoice()
        {
            //  NOTE:  we will need to update the employee ID (1) to use the security
            NavigationManager.NavigateTo($"/HogWild/SimpleInvoiceEdit/0/0/1");
        }

        /// <summary>
        /// Simple edit invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="customerID">The customer identifier.</param>
        private void SimpleEditInvoice(int invoiceID, int customerID)
        {
            //  NOTE:  we will need to update the employee ID (1) to use the security
            NavigationManager.NavigateTo($"/HogWild/SimpleInvoiceEdit/{invoiceID}/{customerID}/1");
        }

        /// <summary>
        /// Edits the invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        /// <param name="customerID">The customer identifier.</param>
        private void EditInvoice(int invoiceID, int customerID)
        {
            //  NOTE:  we will need to update the employee ID (1) to use the security
            NavigationManager.NavigateTo($"/HogWild/InvoiceEdit/{invoiceID}/{customerID}/1");
        }


    }
}
#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using HogWildWebApp.Shared;
using HogWildSystem.Paginator;
using System.IO;
using HogWildWebApp.Components;

namespace HogWildWebApp.Pages.HogWild
{
    public partial class SimpleInvoiceEdit
    {
        #region Field
        /// <summary>
        /// The last name
        /// </summary>
        private string lastName;
        /// <summary>
        /// The phone number
        /// </summary>
        private string phoneNumber;
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
        /// <summary>
        /// The invoice
        /// </summary>
        private InvoiceView invoice;

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
        /// Gets or sets the navigation manager.
        /// </summary>
        /// <value>The navigation manager.</value>
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        /// <summary>
        /// Gets or sets the invoice service.
        /// </summary>
        /// <value>The invoice service.</value>
        [Inject]
        protected InvoiceService InvoiceService { get; set; }

        /// <summary>
        /// Gets or sets the customer service.
        /// </summary>
        /// <value>The customer service.</value>
        [Inject]
        protected CustomerService CustomerService { get; set; }

        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        /// <value>The customers.</value>
        protected List<CustomerSearchView> Customers { get; set; } = new();

        /// <summary>
        /// Gets or sets the part service.
        /// </summary>
        /// <value>The part service.</value>
        [Inject]
        protected PartService PartService { get; set; }
        /// <summary>
        /// Gets or sets the parts.
        /// </summary>
        /// <value>The parts.</value>
        protected List<PartView> Parts { get; set; } = new();
        /// <summary>
        /// Gets or sets the category lookup service.
        /// </summary>
        /// <value>The category lookup service.</value>
        [Inject]
        protected CategoryLookupService CategoryLookupService { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        [Inject]
        protected IDialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        /// <value>The invoice identifier.</value>
        [Parameter] public int InvoiceID { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        [Parameter] public int CustomerID { get; set; }
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        /// <value>The employee identifier.</value>
        [Parameter] public int EmployeeID { get; set; }
        #endregion

        #region Paginator
        // Desired current page size
        private const int PAGE_SIZE = 5;

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
            await SearchParts();
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
                //  get categories
                partCategories = CategoryLookupService.GetLookups("Part Categories");

                //  get the invoice
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);

                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

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

                errorMessage = $"{errorMessage}Unable to search for invoice";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <exception cref="System.ArgumentException">Please provide either a last name and/or phone number</exception>
        private void SearchCustomer()
        {
            try
            {
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  clear the customer list before we do our search
                Customers.Clear();

                if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new ArgumentException("Please provide either a last name and/or phone number");
                }

                //  search for our customers
                Customers = CustomerService.GetCustomers(lastName, phoneNumber);

                if (Customers.Count == 0)
                {
                    feedbackMessage = "No customer were found for your search criteria";
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
                errorMessage = $"{errorMessage}Unable to search for customer";
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
        private async Task SearchParts()
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

                if (PaginatorParts.Results.Length > 0)
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

        private void SelectCustomer(int customerID)
        {
            invoice.CustomerID = customerID;
            invoice.CustomerName = CustomerService.GetCustomerFullName(customerID);
        }

        private async Task DeleteInvoiceLine(int partID)
        {
            InvoiceLineView invoiceLine = invoice.InvoiceLines
                .Where(x => x.PartID == partID)
                .Select(x => x).FirstOrDefault();
            var parameters = new DialogParameters();
            parameters.Add("ContentText", $"Do you want to remove invoice line {invoiceLine.Description}?");
            parameters.Add("ButtonText", "OK");
            parameters.Add("Color", Color.Warning);

            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<ConfirmDialog>("Delete Invoice Line", parameters, options);
            var result = await dialog.Result;
            if (result.Data != null && ((bool)result.Data))
            {
                invoice.InvoiceLines.Remove(invoiceLine);
            }
        }

        private void UpdateSubtotalAndTax()
        {
            invoice.SubTotal = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Quantity * x.Price);
            invoice.Tax = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Taxable ? x.Quantity * x.Price * 0.05m : 0);
        }

        private async Task AddPart(int partID)
        {
            PartView part = PartService.GetPart(partID);
            if (invoice.InvoiceLines.Any(x => x.PartID == partID))
            {
                errorMessage = $"Part \"{part.Description}\" already exists in the invoice list!";
            }
            else
            {
                InvoiceLineView invoiceLine = new InvoiceLineView();
                invoiceLine.PartID = partID;
                invoiceLine.Description = part.Description;
                invoiceLine.Price = part.Price;
                invoiceLine.Taxable = part.Taxable;
                invoiceLine.Quantity = 0;
                invoice.InvoiceLines.Add(invoiceLine);
                await InvokeAsync(StateHasChanged);
            }
        }

        private void Save()
        {
            try
            {
                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;
                InvoiceService.Save(invoice);
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
                errorMessage = $"{errorMessage}Unable to save invoice";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        private async Task CancelClose()
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", $"Are you sure you wish to close the invoice screen?  There maybe un-save changes");
            parameters.Add("ButtonText", "OK");
            parameters.Add("Color", Color.Warning);

            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<ConfirmDialog>("CancelClose Invoice", parameters, options);
            var result = await dialog.Result;
            if (result.Data != null && ((bool)result.Data))
            {
                NavigationManager.NavigateTo($"/HogWild/InvoiceList/");
            }
        }

        private void Close()
        {
            NavigationManager.NavigateTo($"/HogWild/InvoiceList/");
        }
    }
}

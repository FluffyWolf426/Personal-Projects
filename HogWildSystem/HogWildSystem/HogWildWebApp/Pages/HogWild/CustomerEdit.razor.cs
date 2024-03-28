// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 06-02-2023
//
// Last Modified By : James Thompson
// Last Modified On : 06-21-2023
// ***********************************************************************
// <copyright file="CustomerEdit.razor.cs" project="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#nullable disable
using HogWildSystem.BLL;
using Microsoft.AspNetCore.Components;
using HogWildWebApp.Shared;
using MudBlazor;
using HogWildWebApp.Components;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

namespace HogWildWebApp.Pages.HogWild
{
    /// <summary>
    /// Class CustomerEdit.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class CustomerEdit
    {
        #region Validation
        //  Holds metadata related to a data editing process,
        //      such as flags to indicate which fields have been modified.
        //      and the current set of validation messages
        /// <summary>
        /// The edit context
        /// </summary>
        private EditContext editContext;

        //  Used to store the validation messages
        /// <summary>
        /// The message store
        /// </summary>
        private ValidationMessageStore messageStore;
        #endregion
        #region Field
        /// <summary>
        /// The invoices
        /// </summary>
        private List<InvoiceView> invoices = new List<InvoiceView>();
        /// <summary>
        /// The close button text
        /// </summary>
        private string closeButtonText = "Close";
        /// <summary>
        /// The close button color
        /// </summary>
        private Color closeButtonColor = Color.Default;
        /// <summary>
        /// Gets a value indicating whether [disable save button].
        /// </summary>
        /// <value><c>true</c> if [disable save button]; otherwise, <c>false</c>.</value>
        private bool disableSaveButton => !editContext.IsModified() || !editContext.Validate();
        /// <summary>
        /// Gets a value indicating whether [disable view button].
        /// </summary>
        /// <value><c>true</c> if [disable view button]; otherwise, <c>false</c>.</value>
        private bool disableViewButton => !disableSaveButton;
        /// <summary>
        /// The customer
        /// </summary>
        private CustomerEditView customer = new();
        /// <summary>
        /// The provinces
        /// </summary>
        private List<LookupView> provinces = new();
        /// <summary>
        /// The countries
        /// </summary>
        private List<LookupView> countries = new();
        /// <summary>
        /// The status lookup
        /// </summary>
        private List<LookupView> statusLookup = new();

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
        /// Gets or sets the customer service.
        /// </summary>
        /// <value>The customer service.</value>
        [Inject]
        protected CustomerService CustomerService { get; set; }

        /// <summary>
        /// Gets or sets the category lookup service.
        /// </summary>
        /// <value>The category lookup service.</value>
        [Inject]
        protected CategoryLookupService CategoryLookupService { get; set; }

        /// <summary>
        /// Gets or sets the invoice service.
        /// </summary>
        /// <value>The invoice service.</value>
        [Inject]
        protected InvoiceService InvoiceService { get; set; }

        /// <summary>
        /// Gets or sets the dialog service.
        /// </summary>
        /// <value>The dialog service.</value>
        [Inject]
        protected IDialogService DialogService { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [Parameter] public int CustomerID { get; set; }

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
                //  edit context needs to be setup after data has been initialized
                //  setup of the edit context to make use of the payment type property
                editContext = new EditContext(customer);
                //  set the validation to use the HandleValidationRequest event
                editContext.OnValidationRequested += HandleValidationRequested;
                //  setup the message store to track any validation messages
                messageStore = new ValidationMessageStore(editContext);
                //  this event will fire each time the data in a property has change.
                editContext.OnFieldChanged += EditContext_OnFieldChanged;

                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  check to see if we are navigating using a valid customer CustomerID.
                //      or are we going to create a new customer.
                if (CustomerID > 0)
                {
                    customer = CustomerService.GetCustomer(CustomerID);
                    invoices = InvoiceService.GetCustomerInvoices(CustomerID);
                }

                //  lookups
                provinces = CategoryLookupService.GetLookups("Province");
                countries = CategoryLookupService.GetLookups("Country");
                statusLookup = CategoryLookupService.GetLookups("Customer Status");


                await InvokeAsync(StateHasChanged);
                // feedbackMessage = "Search for customer(s) was successful";
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
        /// Handles the OnFieldChanged event of the EditContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FieldChangedEventArgs"/> instance containing the event data.</param>
        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            closeButtonText = editContext.IsModified() ? "Cancel" : "Close";
            closeButtonColor = editContext.IsModified() ? Color.Warning : Color.Default;
        }

        /// <summary>
        /// Handles the validation requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ValidationRequestedEventArgs" /> instance containing the event data.</param>
        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            //  clear the message store if there is any existing validation errors.
            messageStore?.Clear();

            //  custom validation logic
            //  first name is required
            if (string.IsNullOrWhiteSpace(customer.FirstName))
            {
                messageStore?.Add(() => customer.FirstName, "First Name is required!");
            }
            //  last name is required
            if (string.IsNullOrWhiteSpace(customer.LastName))
            {
                messageStore?.Add(() => customer.LastName, "Last Name is required!");
            }
            //  phone is required
            if (string.IsNullOrWhiteSpace(customer.Phone))
            {
                messageStore?.Add(() => customer.Phone, "Phone is required!");
            }
            //  email is required
            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                messageStore?.Add(() => customer.Email, "Email is required!");
            }
        }


        /// <summary>
        /// Saves this instance.
        /// </summary>
        private void Save()
        {
            //  reset the error detail list
            errorDetails.Clear();

            //  reset the error message to an empty string
            errorMessage = string.Empty;

            //  reset feedback message to an empty string
            feedbackMessage = String.Empty;
            try
            {
                if (editContext.Validate())
                {
                    customer = CustomerService.AddEditCustomer(customer);
                    feedbackMessage = "Data was successfully saved!";
                    editContext.MarkAsUnmodified();
                    EditContext_OnFieldChanged(this, null);
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

                errorMessage = $"{errorMessage}Unable to add or edit customer";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }


        // This is a Blazor method for handling cancelation.
        private async void Cancel()
        {
            // Check if there are any modifications in the current edit context.
            if (editContext.IsModified())
            {
                // Create a new dialog parameter object to specify the content and behavior of the upcoming dialog.
                var parameters = new DialogParameters();

                // Add a message text to inform the user about potential unsaved changes.
                parameters.Add("ContentText", "There maybe some unsaved changes. Are you sure that you wish to cancel changes?");

                // Add a button text for the dialog.
                parameters.Add("ButtonText", "OK");

                // Set the color of the dialog to indicate an error or warning scenario.
                parameters.Add("Color", Color.Error);

                // Define the display options for the dialog, in this case, setting its maximum width.
                var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

                // Show the dialog and await its result. The `ConfirmDialog` component is being used here.
                var dialog = await DialogService.ShowAsync<ConfirmDialog>("Discard Changes", parameters, options);
                var result = await dialog.Result;

                // If the result from the dialog indicates that changes should be discarded, navigate away from the current page.
                if (result.Data != null && ((bool)result.Data))
                {
                    NavigationManager.NavigateTo($"/HogWild/CustomerList/");
                }
            }
            else
            {
                // If there are no modifications, navigate away from the current page without any prompts.
                NavigationManager.NavigateTo($"/HogWild/CustomerList/");
            }
        }


        /// <summary>
        /// Views the invoice.
        /// </summary>
        /// <param name="invoiceID">The invoice identifier.</param>
        private void ViewInvoice(int invoiceID)
        {
            //  NOTE:   we will need to update the employee ID (1) to use the security
            //          we are passing in a negative 1 for customer ID to set the editor to read only.        
            NavigationManager.NavigateTo($"/HogWild/InvoiceEdit/{invoiceID}/-1/1");
        }
    }
}

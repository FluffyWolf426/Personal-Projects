// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 06-02-2023
//
// Last Modified By : James Thompson
// Last Modified On : 06-21-2023
// ***********************************************************************
// <copyright file="EmployeeEdit.razor.cs" project="HogWildWebApp">
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
    /// Class EmployeeEdit.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class EmployeeEdit
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


        private string closeButtonText = "Close";
        private Color closeButtonColor = Color.Success;

        private bool disableSaveButton => !editContext.IsModified() || !editContext.Validate();

        /// <summary>
        /// The employee
        /// </summary>
        private EmployeeEditView employee = new();
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
        /// Gets or sets the employee service.
        /// </summary>
        /// <value>The employee service.</value>
        [Inject]
        protected EmployeeService EmployeeService { get; set; }

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

        /// The employee identifier
        [Parameter] public int EmployeeID { get; set; }

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
                editContext = new EditContext(employee);
                //  set the validation to use the HandleValidationRequest event
                editContext.OnValidationRequested += HandleValidationRequested;
                //  setup the message store to track any validation messages
                messageStore = new ValidationMessageStore(editContext);
                editContext.OnFieldChanged += EditContext_OnFieldChanged; ;



                //  reset the error detail list
                errorDetails.Clear();

                //  reset the error message to an empty string
                errorMessage = string.Empty;

                //  reset feedback message to an empty string
                feedbackMessage = String.Empty;

                //  check to see if we are navigating using a valid employee CustomerID.
                //      or are we going to create a new employee.
                if (EmployeeID > 0)
                {
                    employee = EmployeeService.GetEmployee(EmployeeID);
                }

                //  lookups
                provinces = CategoryLookupService.GetLookups("Province");
                countries = CategoryLookupService.GetLookups("Country");
                statusLookup = CategoryLookupService.GetLookups("Employee Role");

                await InvokeAsync(StateHasChanged);
                // feedbackMessage = "Search for employee(s) was successful";
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

                errorMessage = $"{errorMessage}Unable to search for employee";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            closeButtonText = editContext.IsModified() ? "Cancel" : "Close";
            closeButtonColor = editContext.IsModified() ? Color.Warning : Color.Default;
        }

        /// <summary>
        /// Handles the validation requested.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ValidationRequestedEventArgs"/> instance containing the event data.</param>
        private void HandleValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            //  clear the message store if there is any existing validation errors.
            messageStore?.Clear();

            //  custom validation logic
            //  first name is required
            if (string.IsNullOrWhiteSpace(employee.FirstName))
            {
                messageStore?.Add(() => employee.FirstName, "First Name is required!");
            }
            //  last name is required
            if (string.IsNullOrWhiteSpace(employee.LastName))
            {
                messageStore?.Add(() => employee.LastName, "Last Name is required!");
            }
            //  phone is required
            if (string.IsNullOrWhiteSpace(employee.Phone))
            {
                messageStore?.Add(() => employee.Phone, "Phone is required!");
            }
            //  email is required
            if (string.IsNullOrWhiteSpace(employee.Email))
            {
                messageStore?.Add(() => employee.Email, "Email is required!");
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
                    EmployeeService.AddEditEmployee(employee);
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

                errorMessage = $"{errorMessage}Unable to add or edit employee";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }


        /// <summary>
        /// Cancels this instance.
        /// </summary>
        private async void Cancel()
        {
            if (editContext.IsModified())
            {
                var parameters = new DialogParameters();
                parameters.Add("ContentText", "There maybe some unsaved changes.  Are you sure that you wish to cancel changes");
                parameters.Add("ButtonText", "OK");
                parameters.Add("Color", Color.Error);

                var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

                var dialog = await DialogService.ShowAsync<ConfirmDialog>("Discard Changes", parameters, options);
                var result = await dialog.Result;
                if (result.Data != null && ((bool)result.Data))
                {
                    NavigationManager.NavigateTo($"/HogWild/EmployeeList/");
                }
            }
            else
            {
                NavigationManager.NavigateTo($"/HogWild/EmployeeList/");
            }
        }
    }
}

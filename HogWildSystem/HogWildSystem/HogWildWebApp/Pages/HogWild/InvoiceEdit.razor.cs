// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 08-22-2023
//
// Last Modified By : James Thompson
// Last Modified On : 08-24-2023
// ***********************************************************************
// <copyright file="InvoiceEdit.razor.cs" company="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using HogWildWebApp.Components;
using HogWildWebApp.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Diagnostics.Metrics;
using System.Reactive.Subjects;
using static MudBlazor.Icons;

namespace HogWildWebApp.Pages.HogWild
{
    /// <summary>
    /// Class InvoiceEdit.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class InvoiceEdit
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
        private bool enableSaveButton => !((editContext.IsModified() || isDirty) && !editContext.GetValidationMessages().Any());


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
        /// Gets or sets the part service.
        /// NOTE:  I have included the part service to add the method of creating invoice lines.
        /// </summary>
        /// <value>The part service.</value>
        [Inject]
        protected PartService PartService { get; set; }

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

        #region Reactive
        #region Fields
        /// <summary>
        /// The subjects (used with reactive)
        /// </summary>
        private List<IDisposable> subjects = new List<IDisposable>();

        /// <summary>
        /// The is dirty
        /// </summary>
        private bool isDirty;
        #endregion

        #region Methods

        /// <summary>
        /// Updates the is dirty reference using subject.
        /// </summary>
        /// <param name="isDirtyObservable">The is dirty observable.</param>
        private void UpdateIsDirtyReferenceUsingSubject(Subject<bool> isDirtyObservable)
        {
            subjects.Add(isDirtyObservable.Subscribe(InvoiceLinesHaveChange));
        }

        /// <summary>
        /// The invoice lines have change.
        /// </summary>
        /// <param name="s">if set to <c>true</c> [s].</param>
        private void InvoiceLinesHaveChange(bool s)
        {
            isDirty = true;
            DataHasChange();
        }

        #endregion
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
                //  get the invoice
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                foreach (var invoiceLine in invoice.InvoiceLines)
                {
                    UpdateIsDirtyReferenceUsingSubject(invoiceLine.IsDirtyObservable);
                }

                //  edit context needs to be setup after data has been initialized
                //  setup of the edit context to make use of the payment type property
                editContext = new EditContext(invoice);
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
        /// Handles the OnFieldChanged event of the EditContext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FieldChangedEventArgs" /> instance containing the event data.</param>
        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            DataHasChange();
        }

        /// <summary>
        /// Datas the has change.
        /// </summary>
        private void DataHasChange()
        {
            //  force a validation request
            HandleValidationRequested(this, null);
            closeButtonText = editContext.IsModified() || isDirty ? "Cancel" : "Close";
            closeButtonColor = editContext.IsModified() || isDirty ? Color.Warning : Color.Default;
            UpdateSubtotalAndTax();
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

            invoice.Validate(messageStore);
            editContext.NotifyValidationStateChanged();
        }


        /// <summary>
        /// Toggles the remove from view.
        /// </summary>
        /// <param name="invoiceLineID">The invoice line identifier.</param>
        private void ToggleRemoveFromView(int invoiceLineID)
        {
            var invoiceLine = invoice.InvoiceLines
                .Where(x => x.InvoiceLineID == invoiceLineID)
                .Select(x => x).FirstOrDefault();
            invoiceLine.RemoveFromViewFlag = !invoiceLine.RemoveFromViewFlag;
            UpdateSubtotalAndTax();
        }

        /// <summary>
        /// Updates the subtotal and tax.
        /// </summary>
        private void UpdateSubtotalAndTax()
        {
            invoice.SubTotal = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Quantity * x.Price);
            invoice.Tax = invoice.InvoiceLines
                .Where(x => !x.RemoveFromViewFlag)
                .Sum(x => x.Taxable ? x.Quantity * x.Price * 0.05m : 0);
        }

        /// <summary>
        /// Toggles the remove from view text.
        /// </summary>
        /// <param name="removeFromView">if set to <c>true</c> [remove from view].</param>
        /// <returns>System.String.</returns>
        private string ToggleRemoveFromViewText(bool removeFromView)
        {
            return removeFromView ? "Undo" : "Remove";
        }
        /// <summary>
        /// Rows the color of the background.
        /// </summary>
        /// <param name="removeFromView">if set to <c>true</c> [remove from view].</param>
        /// <returns>System.String.</returns>
        private string RowBackgroundColor(bool removeFromView)
        {
            return removeFromView ? "remove_from_View_color" : String.Empty;
        }

        /// <summary>
        /// Validates the quantity.
        /// </summary>
        /// <param name="invoiceLineID">The invoice line identifier.</param>
        /// <returns>System.String.</returns>
        private string ValidateQuantity(int invoiceLineID)
        {
            var invoiceLine = invoice.InvoiceLines
                .Where(x => x.InvoiceLineID == invoiceLineID)
                .Select(x => x).FirstOrDefault();
            editContext?.Validate();
            return invoiceLine.RemoveFromViewFlag ? "remove_from_View_color" :
                invoiceLine.Quantity < 1 ? "invalid_quantity_color" : String.Empty;

        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
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
                //  get the invoice
                invoice = InvoiceService.GetInvoice(InvoiceID, CustomerID, EmployeeID);
                //  we need to clear the subjects as the invoices lines no longer exists
                //  for the previous instance of invoice lines.
                subjects.Clear();
                foreach (var invoiceLine in invoice.InvoiceLines)
                {
                    UpdateIsDirtyReferenceUsingSubject(invoiceLine.IsDirtyObservable);
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
                errorMessage = $"{errorMessage}Unable to save invoice";
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
                    NavigationManager.NavigateTo($"/HogWild/InvoiceList/");
                }
            }
            else
            {
                NavigationManager.NavigateTo($"/HogWild/InvoiceList/");
            }
        }

        /// <summary>
        /// Adds the part.
        /// NOTE:   This method is included in-case you wish to add an invoice line.
        ///         PLEASE NOTE: that you need to UpdateIsDirtyReferenceUsingSubject
        ///         so that the save button will enable.
        /// </summary>
        /// <param name="partID">The part identifier.</param>
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
                if (invoice.InvoiceLines.Count == 0)
                {
                    invoiceLine.InvoiceLineID = 1;
                }
                else
                {
                    invoiceLine.InvoiceLineID = invoice.InvoiceLines.Max(x => x.InvoiceLineID) + 1;
                }
                invoiceLine.PartID = partID;
                invoiceLine.Description = part.Description;
                invoiceLine.Price = part.Price;
                invoiceLine.Taxable = part.Taxable;
                invoiceLine.Quantity = 0;
                UpdateIsDirtyReferenceUsingSubject(invoiceLine.IsDirtyObservable);
                invoiceLine.IsDirty = true;
                invoice.InvoiceLines.Add(invoiceLine);
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
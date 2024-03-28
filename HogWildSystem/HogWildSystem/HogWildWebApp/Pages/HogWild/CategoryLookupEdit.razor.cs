// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 08-21-2023
//
// Last Modified By : James Thompson
// Last Modified On : 08-21-2023
// ***********************************************************************
// <copyright file="CategoryLookupEdit.razor.cs" company="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using HogWildWebApp.Components;
using HogWildWebApp.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HogWildWebApp.Pages.HogWild
{
    /// <summary>
    /// Class CategoryLookupEdit.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class CategoryLookupEdit
    {
        #region Fields
        /// <summary>
        /// The partCategories
        /// </summary>
        private List<CategoryView> categories;

        /// <summary>
        /// The category identifier
        /// </summary>
        private int _categoryID;

        /// <summary>
        /// The lookup identifier
        /// </summary>
        private int lookupID;
        /// <summary>
        /// The lookups
        /// </summary>
        private List<LookupView> lookups;

        /// <summary>
        /// Gets a value indicating whether [disable edit].
        /// </summary>
        /// <value><c>true</c> if [disable edit]; otherwise, <c>false</c>.</value>
        private bool disableEdit => _categoryID == 0;

        private bool disableLookupEdit => _categoryID == 0 || lookupID == 0;

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
        /// The category identifier
        /// </summary>
        /// <value>The category identifier.</value>
        public int CategoryID
        {
            get
            {
                return _categoryID;
            }
            set
            {
                _categoryID = value;
                CategoryChange(_categoryID);
            }
        }

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
                categories = CategoryLookupService.GetCategories();
                lookups = new List<LookupView>();
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

                errorMessage = $"{errorMessage}Unable to retrieve categories";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }

        /// <summary>
        /// Categories the edit.
        /// </summary>
        private async void CategoryEdit()
        {

            var parameters = new DialogParameters();
            parameters.Add("Category", CategoryLookupService.GetCategory(CategoryID));
            parameters.Add("Lookup", null);

            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<CategoryLookupDialog>("Edit", parameters, options);
            var result = await dialog.Result;

            //  save button has been selected
            if ((bool)result.Data == true)
            {
                CategoryView categoryView = new CategoryView()
                {
                    CategoryID = CategoryID,
                    CategoryName = ((CategoryLookupDialog)dialog.Dialog).editValue,
                    RemoveFromViewFlag = false
                };
                CategoryLookupService.AddEditCategory(categoryView);
                categories = CategoryLookupService.GetCategories();
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Add Lookups  
        /// </summary>
        private async void LookupAdd()
        {
            var parameters = new DialogParameters();
            parameters.Add("Category", null);
            parameters.Add("Lookup", new LookupView());

            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<CategoryLookupDialog>("Add", parameters, options);
            var result = await dialog.Result;

            //  save button has been selected
            if ((bool)result.Data == true)
            {
                LookupView lookupView = new LookupView()
                {
                    CategoryID = CategoryID,
                    LookupID = 0,
                    Name = ((CategoryLookupDialog)dialog.Dialog).editValue,
                    RemoveFromViewFlag = false
                };
                CategoryLookupService.AddLookup(lookupView);
                lookups = CategoryLookupService.GetLookups(CategoryID);
                lookupID = lookups.Max(x => x.LookupID);
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Edit Lookups  
        /// </summary>
        private async void LookupEdit()
        {
            var parameters = new DialogParameters();
            parameters.Add("Category", null);
            parameters.Add("Lookup", CategoryLookupService.GetLookup(lookupID));

            var options = new DialogOptions() { MaxWidth = MaxWidth.ExtraSmall };

            var dialog = await DialogService.ShowAsync<CategoryLookupDialog>("Edit", parameters, options);
            var result = await dialog.Result;

            //  save button has been selected
            if ((bool)result.Data == true)
            {
                LookupView lookupView = new LookupView()
                {
                    CategoryID = CategoryID,
                    LookupID = lookupID,
                    Name = ((CategoryLookupDialog)dialog.Dialog).editValue,
                    RemoveFromViewFlag = false
                };
                CategoryLookupService.EditLookup(lookupView);
                lookups = CategoryLookupService.GetLookups(CategoryID);
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Categories the change.
        /// </summary>
        /// <param name="categoryID">The category identifier.</param>
        private async void CategoryChange(int categoryID)
        {
            if (categoryID == 0)
            {
                lookups.Clear();
            }
            else
            {
                lookups = CategoryLookupService.GetLookups(categoryID);
                lookupID = 0;
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}

// ***********************************************************************
// Assembly         : HogWildWebApp
// Author           : James Thompson
// Created          : 06-23-2023
//
// Last Modified By : James Thompson
// Last Modified On : 06-23-2023
// ***********************************************************************
// <copyright file="CategoryLookupDialog.razor.cs" project="HogWildWebApp">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HogWildWebApp.Components
{
    /// <summary>
    /// Class CategoryLookupDialog.
    /// Implements the <see cref="ComponentBase" />
    /// </summary>
    /// <seealso cref="ComponentBase" />
    public partial class CategoryLookupDialog
    {
        #region Properties

        public string editValue;
        

        #endregion
        /// <summary>
        /// Gets or sets the mud dialog.
        /// </summary>
        /// <editValue>The mud dialog.</editValue>
        [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

        /// <summary>
        /// Gets or sets the content text.
        /// </summary>
        /// <editValue>The content text.</editValue>
        [Parameter] public string? ContentText { get; set; }

        [Parameter] public CategoryView? Category { get; set; }
        [Parameter] public LookupView? Lookup { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (Category != null)
            {
                ContentText = Category.CategoryID > 0 ? "Category - Edit" : "Category - Add";
                editValue = Category.CategoryName;
            }
            else
            {
                ContentText = Lookup != null && Lookup.LookupID > 0 ? "Lookup - Edit" : "Lookup - Add";
                if (Lookup != null) editValue = Lookup.Name;
            }
        }

        /// <summary>
        /// Submits this instance.
        /// </summary>
        private void Submit() => MudDialog?.Close(DialogResult.Ok(true));
        /// <summary>
        /// Cancels this instance.
        /// </summary>
        private void Cancel() => MudDialog?.Cancel();
    }
}

// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : James Thompson
// Created          : 05-31-2023
//
// Last Modified By : James Thompson
// Last Modified On : 08-25-2023
// ***********************************************************************
// <copyright file="InvoiceView.cs" company="HogWildSystem">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components.Forms;

namespace HogWildSystem.ViewModels
{
    /// <summary>
    /// Class InvoiceView.
    /// </summary>
    public class InvoiceView
    {

        /// <summary>
        /// Gets or sets the invoice identifier.
        /// </summary>
        /// <value>The invoice identifier.</value>
        public int InvoiceID { get; set; }
        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        /// <value>The invoice date.</value>
        public DateTime InvoiceDate { get; set; }
        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        /// <value>The customer identifier.</value>
        public int CustomerID { get; set; }
        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        /// <value>The name of the customer.</value>
        public string CustomerName { get; set; }
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        /// <value>The employee identifier.</value>
        public int EmployeeID { get; set; }
        /// <summary>
        /// Gets or sets the name of the employee.
        /// </summary>
        /// <value>The name of the employee.</value>
        public string EmployeeName { get; set; }
        /// <summary>
        /// Gets or sets the sub total.
        /// </summary>
        /// <value>The sub total.</value>
        public decimal SubTotal { get; set; }
        /// <summary>
        /// Gets or sets the tax.
        /// </summary>
        /// <value>The tax.</value>
        public decimal Tax { get; set; }
        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <value>The total.</value>
        public decimal Total => SubTotal + Tax;
        /// <summary>
        /// Gets or sets the invoice lines.
        /// </summary>
        /// <value>The invoice lines.</value>
        public List<InvoiceLineView> InvoiceLines { get; set; } = new();
        /// <summary>
        /// Gets or sets a value indicating whether [remove from view flag].
        /// </summary>
        /// <value><c>true</c> if [remove from view flag]; otherwise, <c>false</c>.</value>
        public bool RemoveFromViewFlag { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty { get; set; }

        // https://stackoverflow.com/questions/75557130/how-to-add-a-validationmessage-for-a-specific-item-in-a-list-property-using-blaz
        /// <summary>
        /// Validates the specified validation store.
        /// </summary>
        /// <param name="validationStore">The validation store.</param>
        public void Validate(ValidationMessageStore? validationStore)
        {
            if (validationStore is null)
                return;

            // clear our section of the message store
            //validationStore.Clear();

            foreach (var invoiceLine in InvoiceLines)
            {
                // add any new messages using a FieldIdentifier instance to specify the object instance and the property name 
                if (invoiceLine.Quantity < 1)
                    validationStore.Add(new FieldIdentifier(invoiceLine, nameof(invoiceLine.Quantity)), $"{invoiceLine.Description}: Quantity must be greater than zero");
            }
        }      
    }
}

// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : James Thompson
// Created          : 05-31-2023
//
// Last Modified By : James Thompson
// Last Modified On : 08-25-2023
// ***********************************************************************
// <copyright file="InvoiceLineView.cs" company="HogWildSystem">
//     Copyright (c) Northern Alberta Institute of Technology. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Reactive.Subjects;

namespace HogWildSystem.ViewModels;

/// <summary>
///     Class InvoiceLineView.
/// </summary>
public class InvoiceLineView
{
    #region Fields

    /// <summary>
    ///     The remove from view flag
    /// </summary>
    private bool removeFromViewFlag;

    /// <summary>
    ///     The quantity
    /// </summary>
    private int quantity;

    /// <summary>
    ///     The is dirty
    /// </summary>
    private bool _isDirty;

    #endregion

    /// <summary>
    ///     Gets or sets the invoice line identifier.
    /// </summary>
    /// <value>The invoice line identifier.</value>
    public int InvoiceLineID { get; set; }

    /// <summary>
    ///     Gets or sets the invoice identifier.
    /// </summary>
    /// <value>The invoice identifier.</value>
    public int InvoiceID { get; set; }

    /// <summary>
    ///     Gets or sets the part identifier.
    /// </summary>
    /// <value>The part identifier.</value>
    public int PartID { get; set; }

    /// <summary>
    ///     Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the quantity.
    /// </summary>
    /// <value>The quantity.</value>
    public int Quantity
    {
        get => quantity;
        set
        {
            quantity = value;
            //  set IsDirty to true
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Gets or sets the price.
    /// </summary>
    /// <value>The price.</value>
    public decimal Price { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this <see cref="InvoiceLineView" /> is taxable.
    /// </summary>
    /// <value><c>true</c> if taxable; otherwise, <c>false</c>.</value>
    public bool Taxable { get; set; }

    /// <summary>
    ///     Gets the extent price.
    /// </summary>
    /// <value>The extent price.</value>
    public decimal ExtentPrice => Price * Quantity;

    /// <summary>
    ///     Gets or sets a value indicating whether [remove from view flag].
    /// </summary>
    /// <value><c>true</c> if [remove from view flag]; otherwise, <c>false</c>.</value>
    public bool RemoveFromViewFlag
    {
        get => removeFromViewFlag;
        set
        {
            removeFromViewFlag = value;
            //  set IsDirty to true
            IsDirty = true;
        }
    }


    #region Observable Pattern

    /// <summary>
    ///     Gets or sets the is dirty observable.
    /// </summary>
    /// <value>The is dirty observable.</value>
    public Subject<bool> IsDirtyObservable { get; set; } = new();
    
    /// <summary>
    ///     Gets or sets a value indicating whether this instance is dirty.
    /// </summary>
    /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            _isDirty = value;
            IsDirtyObservable.OnNext(true);
        }
    }

    #endregion


}
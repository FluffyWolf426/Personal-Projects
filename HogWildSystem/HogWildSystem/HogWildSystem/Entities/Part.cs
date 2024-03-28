﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HogWildSystem.Entities;

[Table("Part")]
internal partial class Part
{
    [Key]
    public int PartID { get; set; }

    public int PartCategoryID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string Description { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Cost { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Price { get; set; }

    public int ROL { get; set; }

    public int QOH { get; set; }

    [Required]
    public bool Taxable { get; set; }

    public bool RemoveFromViewFlag { get; set; }

    [InverseProperty("Part")]
    public virtual ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();

    [ForeignKey("PartCategoryID")]
    [InverseProperty("Parts")]
    public virtual Lookup PartCategory { get; set; }
}
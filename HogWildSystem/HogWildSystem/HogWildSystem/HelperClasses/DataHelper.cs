// ***********************************************************************
// Assembly         : HogWildSystem
// Author           : JTHOMPSON
// Created          : 08-21-2023
//
// Last Modified By : JTHOMPSON
// Last Modified On : 06-22-2023
// ***********************************************************************
// <copyright file="DataHelper.cs" company="HogWildSystem">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Reflection;

namespace HogWildSystem.HelperClasses
{
	/// <summary>
	/// Class DataHelper.
	/// </summary>
	public class DataHelper
	{
		//	Copies the fields from the source object (src) to the destination object (dest)
		//	The method will ignore any primary key.
		/// <summary>
		/// Copies the item property values.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dest">The dest.</param>
		/// <param name="key">The key.</param>
		public void CopyItemPropertyValues(object src, object dest, string key)
		{
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			PropertyInfo[] destFields = dest.GetType().GetProperties(flags);
			PropertyInfo[] srcFields = src.GetType().GetProperties(flags);

			PropertyInfo destField;
			foreach (PropertyInfo srcField in srcFields)
			{
				destField = destFields.FirstOrDefault(field => field.Name == srcField.Name
														&& field.Name != key);

				if (destField != null)
				{
					//  ensure that the 2 properties types are the same.
					if (srcField.PropertyType == destField.PropertyType)
					{
						destField.SetValue(dest, srcField.GetValue(src));
					}
				}
			}
		}

		//	Compares the fields of the source object (src) and the destination object (dest)
		//	The method will ignore any primary key.
		/// <summary>
		/// Compares the properties values.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="dest">The dest.</param>
		/// <param name="ignoreValues">The ignore values.</param>
		/// <returns>List&lt;CompareProperty&gt;.</returns>
		public List<CompareProperty> ComparePropertiesValues(object src, object dest, List<string> ignoreValues)
		{
			List<CompareProperty> compareProperties = new List<CompareProperty>();
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			PropertyInfo[] destFields = dest.GetType().GetProperties(flags);
			PropertyInfo[] srcFields = src.GetType().GetProperties(flags);

			PropertyInfo destField;
			foreach (PropertyInfo srcField in srcFields)
			{
				destField = destFields.FirstOrDefault(field => field.Name == srcField.Name
										&& !ignoreValues.Contains(field.Name));

				if (destField != null)
				{
					//  ensure that the 2 properties types are the same.
					if (srcField.PropertyType == destField.PropertyType)
					{
						object srcValue = srcField.GetValue(src, null);
						object destValue = destField.GetValue(dest, null);
						if (srcValue != destValue && (srcValue == null || !srcValue.Equals(destValue)))
						{
							CompareProperty compareProperty = new CompareProperty();
							compareProperty.ProperName = srcField.Name;
							compareProperty.SourceValue = srcField.GetValue(src).ToString();
							compareProperty.DestinationValue = destField.GetValue(dest).ToString();
							compareProperties.Add(compareProperty);
						}
					}
				}
			}
			return compareProperties;
		}

		/// <summary>
		/// Represents a property that has mismatched values between two objects.
		/// </summary>
		public class CompareProperty
		{
			public string ProperName { get; set; }
			public string SourceValue { get; set; }
			public string DestinationValue { get; set; }
		}
	}

}

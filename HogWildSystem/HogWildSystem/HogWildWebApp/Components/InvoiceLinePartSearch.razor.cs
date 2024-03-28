using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using HogWildWebApp.Shared;
using Microsoft.AspNetCore.Components;

namespace HogWildWebApp.Components
{
    public partial class InvoiceLinePartSearch
    {
        #region Fields
        /// <summary>
        /// The last name
        /// </summary>
        private string lastName;
        /// <summary>
        /// The phone number
        /// </summary>
        private string phoneNumber;
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
        #endregion

        /// <summary>
        /// Searches this instance.
        /// </summary>
        /// <exception cref="System.ArgumentException">Please provide either a last name and/or phone number</exception>
        private void Search()
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

                if (Customers.Count > 0)
                {
                    feedbackMessage = "Search for customer(s) was successful";
                }
                else
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
    }
}

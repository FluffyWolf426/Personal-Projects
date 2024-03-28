#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.ViewModels;
using HogWildWebApp.Shared;
using Microsoft.AspNetCore.Components;

namespace HogWildWebApp.Pages.HogWild
{
    public partial class EmployeeList
    {
        #region Fields

        private string lastName;
        private string phoneNumber;
        //  placeholder for feedback message
        private string feedbackMessage;
        //  placeholder for error messasge
        private string errorMessage;

        //  properties that return the result of the lambda action
        private bool hasFeedback => !string.IsNullOrWhiteSpace(feedbackMessage);
        private bool hasError => !string.IsNullOrWhiteSpace(errorMessage);

        //  used to display any collection of errors on web page
        //  whether the errors are generated locally or come from the class library
        private List<string> errorDetails = new();
        #endregion

        #region Properties
        [Inject]
        protected EmployeeService EmployeeService { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected List<EmployeeSearchView> Employees { get; set; } = new();

        #endregion

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

                //  clear the employee list before we do our search
                Employees.Clear();

                if (string.IsNullOrWhiteSpace(lastName) && string.IsNullOrWhiteSpace(phoneNumber))
                {
                    throw new ArgumentException("Please provide either a last name and/or phone number");
                }

                //  search for our employees
                Employees = EmployeeService.GetEmployees(lastName, phoneNumber);

                if (Employees.Count > 0)
                {
                    feedbackMessage = "Search for employee(s) was successful";
                }
                else
                {
                    feedbackMessage = "No employee were found for your search criteria";
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
                errorMessage = $"{errorMessage}Unable to search for employee";
                foreach (var error in ex.InnerExceptions)
                {
                    errorDetails.Add(error.Message);
                }
            }
        }


        private void New()
        {
            NavigationManager.NavigateTo($"/HogWild/EmployeeEdit/0");
        }

        private void EditEmployee(int employeeId)
        {
            NavigationManager.NavigateTo($"/HogWild/EmployeeEdit/{employeeId}");
        }
    }
}

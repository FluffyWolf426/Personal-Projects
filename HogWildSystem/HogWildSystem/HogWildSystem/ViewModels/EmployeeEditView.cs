namespace HogWildSystem.ViewModels
{
    public class EmployeeEditView
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int ProvStateID { get; set; }
        public int CountryID { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RoleID { get; set; }
        public bool RemoveFromViewFlag { get; set; }
    }
}

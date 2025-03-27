namespace achievoo.Requests.Employees;

public record GetEmployeeByIdRequest(int Id);

public record CreateEmployeeRequest(string FirstName, string LastName, string EmailAddress, string JobTitle, 
    string Department, string EmploymentType, string Location, DateTime JoinedCompany, string Role);

public record UpdateEmployeeRequest(int Id, string FirstName, string LastName, string EmailAddress, string JobTitle, 
    string Department, string EmploymentType, string Location, DateTime JoinedCompany, string Role);
    
public record DeleteEmployeeRequest(int Id, string Auth0Id);
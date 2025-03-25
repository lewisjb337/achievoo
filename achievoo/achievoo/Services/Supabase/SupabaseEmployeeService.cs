using achievoo.Models.Supabase;
using achievoo.Requests.Employees;
using achievoo.Services.Contracts.Supabase;
using Supabase;

namespace achievoo.Services.Supabase;

public class SupabaseEmployeeService(SupabaseService supabaseService) : ISupabaseEmployeeService
{
    private Client? _supabase = supabaseService.SupabaseClient;

    public async Task<IEnumerable<Employee>> GetEmployeesAsync()
    {
        var results = await _supabase!
            .From<Employee>()
            .Get();
        
        return results.Models;
    }

    public async Task<Employee> GetEmployeeByIdAsync(GetEmployeeByIdRequest request)
    {
        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Get();
        
        return result.Model!;
    }

    public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var model = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            EmailAddress = request.EmailAddress,
            JobTitle = request.JobTitle,
            Department = request.Department,
            EmploymentType = request.EmploymentType,
            Location = request.Location,
            JoinedCompany = request.JoinedCompany,
            Role = request.Role,
            DateCreated = DateTime.Now
        };

        var result = await _supabase!
            .From<Employee>()
            .Insert(model);
        
        // TODO: Create an entry into Auth0 for this user too, with appropriate role and accepted status
        
        return result.ResponseMessage!.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request)
    {
        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Set(x => x.FirstName, request.FirstName)
            .Set(x => x.LastName, request.LastName)
            .Set(x => x.EmailAddress, request.EmailAddress)
            .Set(x => x.JobTitle, request.JobTitle)
            .Set(x => x.Department, request.Department)
            .Set(x => x.EmploymentType, request.EmploymentType)
            .Set(x => x.Location, request.Location)
            .Set(x => x.JoinedCompany, request.JoinedCompany)
            .Set(x => x.Role, request.Role)
            .Set(x => x.LastModified, DateTime.Now)
            .Update();
        
        // TODO: Update Auth0 entry for this user when changes made
        
        return result.ResponseMessage!.IsSuccessStatusCode;
    }

    public Task<bool> DeleteEmployeeAsync(DeleteEmployeeRequest request)
    {
        var result = _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Delete();
        
        // TODO: Delete Auth0 entry for this user when changes made
        
        return Task.FromResult(result.IsCompleted);
    }
}
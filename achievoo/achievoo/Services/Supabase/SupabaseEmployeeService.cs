using achievoo.Models.Supabase;
using achievoo.Requests.Employees;
using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using Supabase;

namespace achievoo.Services.Supabase;

public class SupabaseEmployeeService(SupabaseService supabaseService, IAuth0Service auth0Service) : ISupabaseEmployeeService
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Client? _supabase = supabaseService.SupabaseClient;

    public async Task<IEnumerable<Employee>?> GetEmployeesInOrganizationAsync()
    {
        var organizationId = await auth0Service.GetCurrentUserOrganizationIdAsync();
        
        var results = await _supabase!
            .From<Employee>()
            .Where(x => x.OrganizationId == organizationId)
            .Get();
        
        return results.Models;
    }

    public async Task<Employee> GetEmployeeByIdAsync(GetEmployeeByIdRequest request)
    {
        var organizationId = await auth0Service.GetCurrentUserOrganizationIdAsync();

        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.OrganizationId == organizationId)
            .Get();
        
        return result.Model!;
    }

    public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var organizationId = await auth0Service.GetCurrentUserOrganizationIdAsync();

        var success = organizationId != null && await auth0Service.InviteUserToOrganizationAsync(organizationId, request.EmailAddress);
        
        if (!success)
        {
            return false;
        }

        if (organizationId == null)
        {
            return false;
        }
        
        var model = new Employee
        {
            OrganizationId = organizationId,
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

        var result = await _supabase!.From<Employee>().Insert(model);
    
        return result.ResponseMessage!.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request)
    {
        var organizationId = await auth0Service.GetCurrentUserOrganizationIdAsync();

        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.OrganizationId == organizationId)
            .Set(x => x.FirstName, request.FirstName)
            .Set(x => x.LastName, request.LastName)
            .Set(x => x.JobTitle, request.JobTitle)
            .Set(x => x.Department, request.Department)
            .Set(x => x.EmploymentType, request.EmploymentType)
            .Set(x => x.Location, request.Location)
            .Set(x => x.JoinedCompany, request.JoinedCompany)
            .Set(x => x.Role, request.Role)
            .Set(x => x.LastModified, DateTime.Now)
            .Update();
        
        return result.ResponseMessage!.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteEmployeeAsync(DeleteEmployeeRequest request)
    {
        var organizationId = await auth0Service.GetCurrentUserOrganizationIdAsync();

        var result = _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.OrganizationId == organizationId)
            .Delete();

        await auth0Service.DeleteUserByEmailAsync(request.EmailAddress);
        
        return result.IsCompleted;
    }
}
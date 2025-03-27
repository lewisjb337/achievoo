using achievoo.Models.Supabase;
using achievoo.Requests.Auth0;
using achievoo.Requests.Employees;
using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using Microsoft.AspNetCore.Components;
using Supabase;

namespace achievoo.Services.Supabase;

public class SupabaseEmployeeService(SupabaseService supabaseService, IAuth0Service auth0Service) : ISupabaseEmployeeService
{
    // ReSharper disable once FieldCanBeMadeReadOnly.Local
    private Client? _supabase = supabaseService.SupabaseClient;

    public async Task<IEnumerable<Employee>?> GetEmployeesAsync()
    {
        var companyId = await auth0Service.GetOnboardingGuidAsync();
        
        var results = await _supabase!
            .From<Employee>()
            .Where(x => x.CompanyGuid == companyId)
            .Get();
        
        return results.Models;
    }

    public async Task<Employee> GetEmployeeByIdAsync(GetEmployeeByIdRequest request)
    {
        var companyId = await auth0Service.GetOnboardingGuidAsync();
        
        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.CompanyGuid == companyId)
            .Get();
        
        return result.Model!;
    }

    public async Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        var companyId = await auth0Service.GetOnboardingGuidAsync();

        var userId = await auth0Service.CreateUserAsync(new Auth0CreateUserRequest
        {
            Email = request.EmailAddress,
            FirstName = request.FirstName,
            LastName = request.LastName,
            JobTitle = request.JobTitle,
            Role = request.Role
        });

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
            CompanyGuid = companyId!,
            Auth0Id = userId,
            DateCreated = DateTime.Now
        };

        var result = await _supabase!
            .From<Employee>()
            .Insert(model);
        
        return result.ResponseMessage!.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request)
    {
        var companyId = await auth0Service.GetOnboardingGuidAsync();

        var result = await _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.CompanyGuid == companyId)
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
        var companyId = await auth0Service.GetOnboardingGuidAsync();

        var result = _supabase!
            .From<Employee>()
            .Where(x => x.Id == request.Id)
            .Where(x => x.CompanyGuid == companyId)
            .Delete();

        await auth0Service.DeleteUserAsync(request.Auth0Id);
        
        return result.IsCompleted;
    }
}
using achievoo.Models.Supabase;
using achievoo.Requests.Employees;

namespace achievoo.Services.Contracts.Supabase;

public interface ISupabaseEmployeeService
{
    Task<IEnumerable<Employee>?> GetEmployeesAsync();
    
    Task<Employee> GetEmployeeByIdAsync(GetEmployeeByIdRequest request);
    
    Task<bool> CreateEmployeeAsync(CreateEmployeeRequest request);

    Task<bool> UpdateEmployeeAsync(UpdateEmployeeRequest request);

    Task<bool> DeleteEmployeeAsync(DeleteEmployeeRequest request);
}
using achievoo.Components.Modules;
using achievoo.Requests.Employees;
using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages.Modals;

public partial class CreateEmployeeModal : ComponentBase
{
    
    [Inject]
    public IValidationService? ValidationService { get; set; }
    
    [Inject]
    public ISupabaseEmployeeService? SupabaseEmployeeService { get; set; }
    
    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    private Modal? _modal;
    
    private bool _isDisabled = false;
    private bool _isAddAttempted = false;
    
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _emailAddress = string.Empty;
    private string _jobTitle = string.Empty;
    private string _department = string.Empty;
    private string _employmentType = string.Empty;
    private string _location = string.Empty;
    private DateTime _joinedCompany = DateTime.Today;
    private string _role = string.Empty;
    
    public Task Open()
    {
        _modal!.ShowModal();

        ClearFields();
        
        StateHasChanged();
        
        return Task.CompletedTask;
    }

    public async Task CreateEmployee()
    {
        _isAddAttempted = true;
        
        var isValid = !string.IsNullOrWhiteSpace(_firstName) &&
                      !string.IsNullOrWhiteSpace(_lastName) &&
                      ValidationService!.IsValidEmail(_emailAddress) &&
                      !string.IsNullOrWhiteSpace(_jobTitle) &&
                      !string.IsNullOrWhiteSpace(_department) &&
                      !string.IsNullOrWhiteSpace(_employmentType) &&
                      !string.IsNullOrWhiteSpace(_location) &&
                      !string.IsNullOrWhiteSpace(_role);
        
        _isDisabled = true;

        if (isValid)
        {
            await SupabaseEmployeeService!.CreateEmployeeAsync(new CreateEmployeeRequest(
                _firstName,
                _lastName,
                _emailAddress,
                _jobTitle,
                _department,
                _employmentType,
                _location,
                _joinedCompany,
                _role
            ));
        
            ClearFields();

            await _modal!.Close();

            StateHasChanged();
        }

        _isDisabled = false;
    }
    
    private void ClearFields()
    {
        _firstName = string.Empty;
        _lastName = string.Empty;
        _emailAddress = string.Empty;
        _jobTitle = string.Empty;
        _department = string.Empty;
        _employmentType = string.Empty;
        _location = string.Empty;
        _joinedCompany = DateTime.Today;
        _role = string.Empty;
    }
}
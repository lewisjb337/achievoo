using achievoo.Models.Supabase;
using achievoo.Requests.Employees;
using achievoo.Services.Contracts.Supabase;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages;

public partial class EmployeeDetails : ComponentBase
{
    [Inject]
    public ISupabaseEmployeeService? SupabaseEmployeeService { get; set; }
    
    private Employee? Employee { get; set; }
    
    [Parameter]
    public int Id { get; set; }
    
    private bool _isLoading = true;
    
    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        await LoadData();
        _isLoading = false;
    }

    private async Task LoadData()
    {
        Employee = await SupabaseEmployeeService!.GetEmployeeByIdAsync(new GetEmployeeByIdRequest(Id));
    }
}
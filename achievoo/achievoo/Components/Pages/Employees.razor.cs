using achievoo.Components.Pages.Modals;
using achievoo.Models.Supabase;
using achievoo.Services.Contracts.Supabase;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages;

public partial class Employees : ComponentBase
{
    [Inject]
    public ISupabaseEmployeeService SupabaseEmployeeService { get; set; }
    
    private IEnumerable<Employee>  _employees { get; set; }
    
    private bool _isLoading = true;
    private string SearchText { get; set; } = string.Empty;
    
    private CreateEmployeeModal? Create { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        await LoadData();
        _isLoading = false;
    }

    private async Task LoadData()
    {
        _employees = await SupabaseEmployeeService.GetEmployeesAsync();
    }
    
    protected async Task Refresh()
    {
        await LoadData();

        StateHasChanged();
    }
}
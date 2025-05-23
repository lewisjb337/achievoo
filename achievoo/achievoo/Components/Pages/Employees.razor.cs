﻿using achievoo.Components.Pages.Modals;
using achievoo.Models.Supabase;
using achievoo.Services.Contracts;
using achievoo.Services.Contracts.Supabase;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages;

public partial class Employees : ComponentBase
{
    [Inject]
    public ISupabaseEmployeeService? SupabaseEmployeeService { get; set; }
    
    [Inject]
    public IAuth0Service? Auth0Service { get; set; }
    
    private IEnumerable<Employee>?  EmployeeCollection { get; set; }
    
    private bool _isLoading = true;
    private bool _organizationExists = false;
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
        EmployeeCollection = await SupabaseEmployeeService!.GetEmployeesInOrganizationAsync();

        if (Auth0Service != null)
        {
            _organizationExists = await Auth0Service.CheckIfUserHasOrganizationAsync();
        }
    }
    
    protected async Task Refresh()
    {
        await LoadData();

        StateHasChanged();
    }
}
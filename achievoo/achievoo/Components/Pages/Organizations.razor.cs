using achievoo.Components.Pages.Modals;
using achievoo.Models.Auth0.Organization;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages;

public partial class Organizations : ComponentBase
{
    [Inject]
    public IAuth0Service? Auth0Service { get; set; }
    
    private IEnumerable<Auth0Organization>?  OrganizationCollection { get; set; }
    
    private bool _isLoading = true;
    private bool _organizationExists = false;
    private string SearchText { get; set; } = string.Empty;
    
    private CreateOrganizationModal? Create { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        
        await LoadData();
        
        _isLoading = false;
    }

    private async Task LoadData()
    {
        OrganizationCollection = await Auth0Service!.GetUserOrganizationsAsync();

        _organizationExists = await Auth0Service.CheckIfUserHasOrganizationAsync();
    }
    
    protected async Task Refresh()
    {
        await LoadData();

        StateHasChanged();
    }
}
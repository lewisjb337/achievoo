using achievoo.Components.Modules;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages.Modals;

public partial class DeleteOrganizationModal : ComponentBase
{
    [Inject]
    public IAuth0Service? Auth0Service { get; set; }
        
    
    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    private Modal? _modal;

    private string _organizationId;
    
    public bool IsDisabled;

    public Task Open(string organizationId)
    {
        _organizationId = organizationId;
        
        _modal!.ShowModal();

        StateHasChanged();
        
        return Task.CompletedTask;
    }

    public async Task Delete()
    {
        IsDisabled = true;

        if (Auth0Service != null)
        {
            await Auth0Service.DeleteOrganizationAsync(_organizationId);
        }

        await _modal!.Close();

        await OnModalClosed.InvokeAsync(true);

        IsDisabled = false;
    }
}
using achievoo.Components.Modules;
using achievoo.Requests.Auth0;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages.Modals;

public partial class CreateOrganizationModal : ComponentBase
{
    [Inject]
    public IValidationService? ValidationService { get; set; }
    
    [Inject]
    public IAuth0Service? Auth0Service { get; set; }
    
    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    private Modal? _modal;
    
    private bool _isDisabled = false;
    private bool _isAddAttempted = false;
    
    private string _name = string.Empty;
    private string _displayName = string.Empty;
    private string _logoUrl = string.Empty;
    private string _primaryColor = string.Empty;
    private string _backgroundColor = string.Empty;
    
    public Task Open()
    {
        _modal!.ShowModal();

        ClearFields();
        
        StateHasChanged();
        
        return Task.CompletedTask;
    }

    public async Task CreateOrganization()
    {
        _isAddAttempted = true;
        
        var isValid = !string.IsNullOrWhiteSpace(_name) &&
                      !string.IsNullOrWhiteSpace(_displayName) &&
                      !string.IsNullOrWhiteSpace(_logoUrl) &&
                      !string.IsNullOrWhiteSpace(_primaryColor) &&
                      !string.IsNullOrWhiteSpace(_backgroundColor);
        
        _isDisabled = true;

        if (isValid)
        {
            await Auth0Service!.CreateOrganizationAsync(new Auth0CreateOrganizationRequest()
            {
                Name = _name,
                DisplayName = _displayName,
                LogoUrl = _logoUrl,
                PrimaryColor = _primaryColor,
                BackgroundColor = _backgroundColor
            });
        
            ClearFields();

            await _modal!.Close();

            StateHasChanged();
        }

        _isDisabled = false;
    }
    
    private void ClearFields()
    {
        _name = string.Empty;
        _displayName = string.Empty;
        _logoUrl = string.Empty;
        _primaryColor = string.Empty;
        _backgroundColor = string.Empty;
    }
}
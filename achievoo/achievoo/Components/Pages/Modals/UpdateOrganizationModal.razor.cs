using achievoo.Components.Modules;
using achievoo.Models.Auth0.Organization;
using achievoo.Requests.Auth0;
using achievoo.Services.Contracts;
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Pages.Modals;

public partial class UpdateOrganizationModal : ComponentBase
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

    private Auth0Organization? _organization;
    private string _organizationId;
    
    private string _name = string.Empty;
    private string _displayName = string.Empty;
    private string _logoUrl = string.Empty;
    private string _primaryColor = string.Empty;
    private string _backgroundColor = string.Empty;
    
    public async Task Open(string organizationId)
    {
        _organizationId =  organizationId;
        
        await LoadData();
            
        _modal!.ShowModal();

        StateHasChanged();
    }

    private async Task LoadData()
    {
        if (Auth0Service != null)
        {
            _organization = await Auth0Service.GetOrganizationByIdAsync(_organizationId);

            if (_organization != null)
            {
                _name = _organization.Name;
                _displayName = _organization.DisplayName;
                _logoUrl = _organization.Branding.LogoUrl;
                _primaryColor = _organization.Branding.Colors.Primary;
                _backgroundColor = _organization.Branding.Colors.PageBackground;
            }
        }

        StateHasChanged();
    }

    public async Task UpdateOrganization()
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
            await Auth0Service!.UpdateOrganizationAsync(new Auth0UpdateOrganizationRequest
            {
                OrganizationId = _organizationId,
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
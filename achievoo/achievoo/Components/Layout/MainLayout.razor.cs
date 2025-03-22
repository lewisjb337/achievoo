using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace achievoo.Components.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    private string _username = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationState is not null)
        {
            var state = await AuthenticationState;

            _username = state?.User?.Identity?.Name ?? string.Empty;
        }
    }
}
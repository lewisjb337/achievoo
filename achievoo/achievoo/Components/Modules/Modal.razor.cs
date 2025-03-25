using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace achievoo.Components.Modules;

public partial class Modal
{
    [Parameter]
    public EventCallback<bool> OnModalClosed { get; set; }

    [Parameter]
    public EventCallback<MouseEventArgs> OnAction { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? BodyContent { get; set; }

    [Parameter]
    public string ButtonClass { get; set; } = string.Empty;

    [Parameter]
    public string ActionName { get; set; } = string.Empty;

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public bool UpdateState { get; set; }

    [Parameter] 
    public bool ShowSubmit { get; set; } = true;

    private bool _showBackdrop = false;
    private string _modalDisplay = "none";
    private string _modalClass = string.Empty;

    public void ShowModal()
    {
        _modalDisplay = "block";
        _modalClass = "show";
        _showBackdrop = true;
    }

    public async Task Close()
    {
        _modalDisplay = "none";
        _modalClass = string.Empty;
        _showBackdrop = false;

        if (UpdateState)
        {
            StateHasChanged();
            await OnModalClosed.InvokeAsync();
        }
    }
}
using Microsoft.AspNetCore.Components;

namespace achievoo.Components.Modules;

public partial class EmployeeCard : ComponentBase
{
    [Parameter] 
    public int Id { get; set; }
    
    [Parameter] 
    public string FirstName { get; set; } = string.Empty;
    
    [Parameter] 
    public string LastName { get; set; } = string.Empty;
    
    [Parameter] 
    public string JobTitle { get; set; } = string.Empty;
    
    [Parameter] 
    public string Email { get; set; } = string.Empty;
    
    [Parameter] 
    public string Location { get; set; } = string.Empty;
    
    [Parameter] 
    public string Department { get; set; } = string.Empty;
    
    [Parameter] 
    public string EmploymentType { get; set; } = string.Empty;
    
    [Parameter] 
    public DateTime Joined { get; set; }
}
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace achievoo.Models.Supabase;

[Table("Employees")]
public class Employee : BaseModel
{
    [PrimaryKey("Id")] 
    public int Id { get; set; }
    
    [Column("FirstName")] 
    public string FirstName { get; set; } = string.Empty;
    
    [Column("LastName")] 
    public string LastName { get; set; } = string.Empty;
    
    [Column("EmailAddress")] 
    public string EmailAddress { get; set; } = string.Empty;
    
    [Column("JobTitle")] 
    public string JobTitle { get; set; } = string.Empty;
    
    [Column("Department")] 
    public string Department { get; set; } = string.Empty;
    
    [Column("EmploymentType")] 
    public string EmploymentType { get; set; } = string.Empty;
    
    [Column("Location")] 
    public string Location { get; set; } = string.Empty;
    
    [Column("JoinedCompany")] 
    public DateTime JoinedCompany { get; set; } = DateTime.Now;
    
    [Column("Role")] 
    public string Role { get; set; } = string.Empty;
    
    [Column("OrganizationId")] 
    public string OrganizationId { get; set; } = string.Empty;
    
    [Column("DateCreated")] 
    public DateTime DateCreated { get; set; } = DateTime.Now;
    
    [Column("LastModified")] 
    public DateTime LastModified { get; set; } = DateTime.Now;
}
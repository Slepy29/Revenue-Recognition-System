namespace Revenue_Recognition_System.DTOs;

public class AddClientDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsCompany { get; set; }
    public string PESEL { get; set; }
    public string KRS { get; set; }
}
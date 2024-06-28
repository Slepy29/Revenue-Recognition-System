namespace Revenue_Recognition_System.Models;

public class Client
{
    public int ClientId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber{ get; set; }
    public bool IsCompany { get; set; }
    public string? PESEL { get; set; }
    public string? KRS { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<Payment> Payments { get; set; }
    public ICollection<Contract> Contracts { get; set; }
}
namespace Revenue_Recognition_System.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public int ContractId { get; set; }
    public Contract Contract { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }
}
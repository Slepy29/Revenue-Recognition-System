namespace Revenue_Recognition_System.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public int ClientId { get; set; }
    public int SoftwareId { get; set; }
    public Software Software { get; set; }
    public DateTime PurchaseDate { get; set; }
    public DateTime UpdateRightsEndDate { get; set; }
    public int? DiscountId { get; set; }
    public Discount Discount { get; set; }
    public Client Client { get; set; }
}
namespace Revenue_Recognition_System.Models;

public class Software
{
    public int SoftwareId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal BasePrice { get; set; }
    public int CurrentVersionId { get; set; }
    public SoftwareVersion CurrentVersion { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
    public ICollection<Contract> Contracts { get; set; }
    public ICollection<Discount> Discounts { get; set; }
}
namespace Revenue_Recognition_System.Models;

public class Discount
{
    public int DiscountId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Percentage { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ICollection<Software> Softwares { get; set; }
}
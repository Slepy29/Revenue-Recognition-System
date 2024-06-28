namespace Revenue_Recognition_System.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public ICollection<Software> Softwares { get; set; }
}
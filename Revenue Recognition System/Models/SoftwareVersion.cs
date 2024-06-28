namespace Revenue_Recognition_System.Models;

public class SoftwareVersion
{
    public int VersionId { get; set; }
    public string VersionNumber { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ICollection<Software> Softwares { get; set; }
}
namespace WebApplication1;

public class Visit
{
    public int Id { get; set; }
    public int AnimalId { get; set; } // Foreign key reference to Animal
    public DateTime DateOfVisit { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}

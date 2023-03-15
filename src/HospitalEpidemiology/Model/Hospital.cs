namespace HospitalEpidemiology.Model;

public class Hospital
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<Unit> Units { get; set; } = new();
}
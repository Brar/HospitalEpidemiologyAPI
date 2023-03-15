namespace HospitalEpidemiology.Model;

public class BedLocation
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public required Room Room { get; set; }
    public List<PatientBedLocationStay> PatientStays { get; set; } = new();
}
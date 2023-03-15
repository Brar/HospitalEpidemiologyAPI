namespace HospitalEpidemiology.Model;

public class Room
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }

    public required Unit Unit { get; set; }
    public List<BedLocation> BedLocations { get; set; } = new();
    public List<PatientRoomStay> PatientStays { get; set; } = new();
}
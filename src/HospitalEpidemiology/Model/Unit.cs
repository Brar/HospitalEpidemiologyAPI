namespace HospitalEpidemiology.Model;

/// <summary>
/// A unit can be a ward but also an outpatient or emergency department
/// </summary>
public class Unit
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public string? Name { get; set; }

    public required Hospital Hospital { get; set; }
    public List<Room> Rooms { get; set; } = new();
    public List<PatientUnitStay> PatientStays { get; set; } = new();
}
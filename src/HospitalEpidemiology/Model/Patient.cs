namespace HospitalEpidemiology.Model;

public class Patient
{
    public int Id { get; set; }
    // Person names are a complicated topic: https://www.w3.org/International/questions/qa-personal-names
    public required string FamilyName { get; set; }
    public required string GivenName { get; set; }
    public required DateOnly Birthdate { get; set; }
    
    public List<PatientBedLocationStay> BedLocationStays { get; set; } = new();
    public List<PatientRoomStay> RoomStays { get; set; } = new();
    public List<PatientUnitStay> UnitStays { get; set; } = new();
}
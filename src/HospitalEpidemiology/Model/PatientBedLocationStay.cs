using NpgsqlTypes;

namespace HospitalEpidemiology.Model;

/// <summary>
/// Reflects the ideal situation when a patient is assigned a bed location
/// </summary>
public class PatientBedLocationStay
{
    int id;
    public required Patient Patient { get; set; }
    public required BedLocation BedLocation { get; set; }
    public required NpgsqlRange<DateTime> Stay { get; set; }
}
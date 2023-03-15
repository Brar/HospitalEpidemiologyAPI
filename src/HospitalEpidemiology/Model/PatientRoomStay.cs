using NpgsqlTypes;

namespace HospitalEpidemiology.Model;

/// <summary>
/// Reflects the situation when a patient is assigned a room but information about the bed location is missing
/// </summary>
public class PatientRoomStay
{
    int id;
    public required Patient Patient { get; set; }
    public required Room Room { get; set; }
    public required NpgsqlRange<DateTime> Stay { get; set; }
}
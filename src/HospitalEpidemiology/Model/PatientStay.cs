using NpgsqlTypes;

namespace HospitalEpidemiology.Model;

public sealed class PatientStay
{
#pragma warning disable CS0169
    // ReSharper disable once InconsistentNaming
    int id;
#pragma warning restore CS0169
    public required Patient Patient { get; set; }
    public required HisLocation HisLocation { get; set; }
    public required NpgsqlRange<DateTime> Stay { get; set; }
}

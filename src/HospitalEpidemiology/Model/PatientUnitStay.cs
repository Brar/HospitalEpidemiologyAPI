using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace HospitalEpidemiology.Model;

/// <summary>
/// Reflects the situation when a patient is admitted to a unit but not assigned a room
/// </summary>
public class PatientUnitStay
{
    int id;
    public required Patient Patient { get; set; }
    public required Unit Unit { get; set; }
    public required NpgsqlRange<DateTime> Stay { get; set; }
}
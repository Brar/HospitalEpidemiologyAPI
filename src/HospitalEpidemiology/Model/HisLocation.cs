namespace HospitalEpidemiology.Model;

public abstract class HisLocation
{
    public int Id { get; set; }
    public HisLocationType Type { get; }

    private protected HisLocation(HisLocationType type) => Type = type;
}

public sealed class HisHospitalLocation : HisLocation
{
    public required Hospital Hospital { get; set; }

    public HisHospitalLocation() : base(HisLocationType.Hospital) { }
}

public sealed class HisUnitLocation : HisLocation
{
    public required Unit Unit { get; set; }

    public HisUnitLocation() : base(HisLocationType.Unit) { }
}

public sealed class HisRoomLocation : HisLocation
{
    public required Room Room { get; set; }

    public HisRoomLocation() : base(HisLocationType.Room)
    {
    }
}

public sealed class HisBedLocation : HisLocation
{
    public required BedLocation BedLocation { get; set; }

    public HisBedLocation() : base(HisLocationType.BedLocation)
    {
    }
}

/// <summary>
/// Reflects the different levels at which we have to capture patient stays.
/// This results from the fact that you typically cannot enforce business rules in patient care
/// (e.g. it would be infeasible if you cannot treat patients unless they've been properly assigned to a bed location
/// or if you cannot admit patients if all bed locations in the hospital information system are occupied). 
/// </summary>
public enum HisLocationType
{
    /// <summary>
    /// The most common case where a patient is assigned to an actual bed location in a patient room on the ward
    /// </summary>
    BedLocation,
    /// <summary>
    /// The case where either the only the room but not the bed location was assigned in the hospital information system
    /// or the patient's bed actually was put into a room where all the bed locations were already occupied (overcapacity mode). 
    /// </summary>
    Room,
    /// <summary>
    /// The case where a patient was admitted to a unit but not even assigned a room.
    /// This could be a bed in the hallway because of overcapacity or just lack of proper data entry in the hospital information system.
    /// </summary>
    Unit,
    /// <summary>
    /// The case where a patient was admitted to a hospital but any other information is missing.
    /// </summary>
    Hospital
}

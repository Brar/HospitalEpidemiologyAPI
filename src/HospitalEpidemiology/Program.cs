using HospitalEpidemiology;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<HospitalEpidemiologyDb>(
    opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("HospitalEpidemiology"))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .UseSnakeCaseNamingConvention()
        .EnableThreadSafetyChecks(false));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<HospitalEpidemiologyDb>();

var indexPatientId = 8;
var minimumCumulativeContactDuration = TimeSpan.FromMinutes(90);
var riskRange = new NpgsqlRange<DateTime>(
    new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    new(2020, 1, 3, 12, 0, 0, DateTimeKind.Utc));

var query =
    from overlappingPatients in
        // Start with the index patient's stays, filtering out any which are completely outside the risk range
        from indexPatientStay in context.PatientRoomStays
        where indexPatientStay.PatientId == indexPatientId
              && indexPatientStay.Stay.Overlaps(riskRange)
        // Join with other patient stays that have the same room and overlapping stays
        from patientStay in context.PatientRoomStays
        where patientStay.PatientId != indexPatientId
              && patientStay.RoomId == indexPatientStay.RoomId
              && patientStay.Stay.Overlaps(indexPatientStay.Stay)
        // Group by the patient, intersecting all their stays with the risk range, projecting the intersected stays
        // to the TimeSpan duration, and aggregating that duration
        group patientStay.Stay by patientStay.PatientId
        into patientStayGroup
        select new
        {
            PatientId = patientStayGroup.Key,
            AggregateOverlapTime = EF.Functions.Sum(patientStayGroup.Select(s => s.Intersect(riskRange))
                .Select(s => s.UpperBound - s.LowerBound))
        }
    // From the above subquery, filter out patients which which didn't overlap for enough tmie
    where overlappingPatients.AggregateOverlapTime > minimumCumulativeContactDuration
    select overlappingPatients;

_ = query.ToArray();

Environment.Exit(0);

app.MapGet("/roomContacts", async ([FromQuery] int indexPatientId, [FromQuery] DateTime? riskFromInclusive, [FromQuery] DateTime? riskToExclusive, [FromQuery] TimeSpan minimumCumulativeContactDuration, HospitalEpidemiologyDb db) =>
{
    var riskRange = new NpgsqlRange<DateTime>(
        riskFromInclusive ?? default, true, !riskFromInclusive.HasValue,
        riskToExclusive ?? default, false, !riskToExclusive.HasValue);

    // ToDo: Query patients that have shared rooms during riskRange for at least minimumCumulativeContactDuration
    var query = from indexPatientStays in db.PatientBedLocationStays
        join indexPatientBedLocations in db.BedLocations on indexPatientStays.BedLocation equals
            indexPatientBedLocations
        join contactPatientBedLocations in db.BedLocations on indexPatientBedLocations.Room equals
            contactPatientBedLocations.Room
        join contactPatientStays in db.PatientBedLocationStays on contactPatientBedLocations equals
            contactPatientStays.BedLocation
        where indexPatientStays.Patient.Id == indexPatientId
              && contactPatientStays.Patient.Id != indexPatientId
              && indexPatientStays.Stay.Overlaps(riskRange)
              && contactPatientStays.Stay.Overlaps(riskRange)
              && indexPatientStays.Stay.Overlaps(contactPatientStays.Stay)
              && indexPatientBedLocations.Room == contactPatientBedLocations.Room
        select new
        {
            contactPatientStays.Patient.GivenName,
            contactPatientStays.Patient.FamilyName,
            contactPatientStays.Stay.LowerBound,
            contactPatientStays.Stay.UpperBound,
            BedCode = contactPatientStays.BedLocation.Code,
            RoomCode = contactPatientStays.BedLocation.Room.Code,
            UnitCode = contactPatientStays.BedLocation.Room.Unit.Code
        };
                
    return await query.ToListAsync();
});

app.MapGet("/unitContacts", async ([FromQuery] int indexPatientId, [FromQuery] DateTime? riskFromInclusive, [FromQuery] DateTime? riskToExclusive, [FromQuery] TimeSpan minimumCumulativeContactDuration, HospitalEpidemiologyDb db) =>
{
    var riskRange = new NpgsqlRange<DateTime>(
        riskFromInclusive ?? default, true, !riskFromInclusive.HasValue,
        riskToExclusive ?? default, false, !riskToExclusive.HasValue);

    // ToDo: Query patients that were in the same units during riskRange for at least minimumCumulativeContactDuration
    var query = db.PatientBedLocationStays;
    return await query.ToListAsync();
});

app.Run();

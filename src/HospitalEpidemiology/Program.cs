using HospitalEpidemiology;
using HospitalEpidemiology.Model;
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
var context = scope.ServiceProvider.GetService<HospitalEpidemiologyDb>()!;

var indexPatientId = 8;
var minimumCumulativeContactDuration = TimeSpan.FromMinutes(90);
var riskRange = new NpgsqlRange<DateTime>(
    new(2020, 1, 1, 12, 0, 0, DateTimeKind.Utc),
    new(2020, 1, 3, 12, 0, 0, DateTimeKind.Utc));

var query =
    from overlappingPatients in
        // Start with the index patient's stays, filtering out any which are completely outside the risk range
        from indexPatientStay in context.PatientStays
        join indexPatientStayLocation in context.HisLocations on indexPatientStay.HisLocation equals indexPatientStayLocation
        where indexPatientStay.Patient.Id == indexPatientId
              && (indexPatientStayLocation.Type == HisLocationType.Room || indexPatientStayLocation.Type == HisLocationType.BedLocation)
              && indexPatientStay.Stay.Overlaps(riskRange)
        // Join with other patient stays that have the same room and overlapping stays
        from patientStay in context.PatientStays
        join patientStayLocation in context.HisLocations on patientStay.HisLocation equals patientStayLocation
        where patientStay.Patient.Id != indexPatientId
              && patientStay.Stay.Overlaps(indexPatientStay.Stay)
              && (
                  (
                      indexPatientStayLocation.Type == HisLocationType.Room
                      &&
                      (
                          (
                              patientStayLocation.Type == HisLocationType.Room
                              &&
                              ((HisRoomLocation)indexPatientStayLocation).Room == ((HisRoomLocation)patientStayLocation).Room
                          )
                          ||
                          (
                              patientStayLocation.Type == HisLocationType.BedLocation
                              &&
                              ((HisRoomLocation)indexPatientStayLocation).Room == ((HisBedLocation)patientStayLocation).BedLocation.Room
                          )
                      )
                  )
                  ||
                  (
                      indexPatientStayLocation.Type == HisLocationType.BedLocation
                      &&
                      (
                          (
                              patientStayLocation.Type == HisLocationType.Room
                              &&
                              ((HisBedLocation)indexPatientStayLocation).BedLocation.Room == ((HisRoomLocation)patientStayLocation).Room
                          )
                          ||
                          (
                              patientStayLocation.Type == HisLocationType.BedLocation
                              &&
                              ((HisBedLocation)indexPatientStayLocation).BedLocation.Room == ((HisBedLocation)patientStayLocation).BedLocation.Room
                          )
                      )
                  )
              )
        // Group by the patient, intersecting all their stays with the risk range, projecting the intersected stays
        // to the TimeSpan duration, and aggregating that duration
        group patientStay.Stay by patientStay.Patient.Id
        into patientStayGroup
        select new
        {
            PatientId = patientStayGroup.Key,
            AggregateOverlapTime = EF.Functions.Sum(patientStayGroup.Select(s => s.Intersect(riskRange))
                .Select(s => s.UpperBound - s.LowerBound))
        }
    // From the above sub-query, filter out patients which which didn't overlap for enough time
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
    var query = db.Patients;
    return await query.ToListAsync();
});

app.MapGet("/unitContacts", async ([FromQuery] int indexPatientId, [FromQuery] DateTime? riskFromInclusive, [FromQuery] DateTime? riskToExclusive, [FromQuery] TimeSpan minimumCumulativeContactDuration, HospitalEpidemiologyDb db) =>
{
    var riskRange = new NpgsqlRange<DateTime>(
        riskFromInclusive ?? default, true, !riskFromInclusive.HasValue,
        riskToExclusive ?? default, false, !riskToExclusive.HasValue);

    // ToDo: Query patients that were in the same units during riskRange for at least minimumCumulativeContactDuration
    var query = db.Patients;
    return await query.ToListAsync();
});

app.Run();

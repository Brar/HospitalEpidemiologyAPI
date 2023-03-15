using HospitalEpidemiology;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextPool<HospitalEpidemiologyDb>(
    opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("HospitalEpidemiology"))
        .UseSnakeCaseNamingConvention()
        .EnableThreadSafetyChecks(false));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

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

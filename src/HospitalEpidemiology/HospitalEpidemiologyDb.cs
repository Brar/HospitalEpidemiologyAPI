using HospitalEpidemiology.Model;
using Microsoft.EntityFrameworkCore;

namespace HospitalEpidemiology;

public class HospitalEpidemiologyDb : DbContext
{
    public HospitalEpidemiologyDb(DbContextOptions<HospitalEpidemiologyDb> options)
        : base(options) { }

    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BedLocation> BedLocations => Set<BedLocation>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientBedLocationStay> PatientBedLocationStays => Set<PatientBedLocationStay>();
    public DbSet<PatientRoomStay> PatientRoomStays => Set<PatientRoomStay>();
    public DbSet<PatientUnitStay> PatientUnitStays => Set<PatientUnitStay>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.Entity<PatientUnitStay>()
            .HasKey("id");
        modelBuilder.Entity<PatientRoomStay>()
            .HasKey("id");
        modelBuilder.Entity<PatientBedLocationStay>()
            .HasKey("id");
    }
}
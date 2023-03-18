using HospitalEpidemiology.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace HospitalEpidemiology;

public class HospitalEpidemiologyDb : DbContext
{
    public HospitalEpidemiologyDb(DbContextOptions<HospitalEpidemiologyDb> options)
        : base(options) { }
    static HospitalEpidemiologyDb()
#pragma warning disable CS0618
        => NpgsqlConnection.GlobalTypeMapper.MapEnum<HisLocationType>();
#pragma warning restore CS0618

    public DbSet<Hospital> Hospitals => Set<Hospital>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<BedLocation> BedLocations => Set<BedLocation>();
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<PatientStay> PatientStays => Set<PatientStay>();
    public DbSet<HisLocation> HisLocations => Set<HisLocation>();
    public DbSet<HisHospitalLocation> HisHospitalLocations => Set<HisHospitalLocation>();
    public DbSet<HisUnitLocation> HisUnitLocations => Set<HisUnitLocation>();
    public DbSet<HisRoomLocation> HisRoomLocations => Set<HisRoomLocation>();
    public DbSet<HisBedLocation> HisBedLocations => Set<HisBedLocation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseIdentityAlwaysColumns();
        modelBuilder.HasPostgresEnum<HisLocationType>();
//        modelBuilder.HasPostgresExtension("btree_gist");
        modelBuilder.Entity<PatientStay>()
            .HasKey("id");
        modelBuilder.Entity<Patient>()
            .HasMany(e => e.Stays)
            .WithOne(e => e.Patient);
        modelBuilder.Entity<HisLocation>(e =>
        {
            e.ToTable("his_locations");
            e.Property(p => p.Type);
            e.HasDiscriminator(p => p.Type)
                .HasValue<HisHospitalLocation>(HisLocationType.Hospital)
                .HasValue<HisUnitLocation>(HisLocationType.Unit)
                .HasValue<HisRoomLocation>(HisLocationType.Room)
                .HasValue<HisBedLocation>(HisLocationType.BedLocation)
                .IsComplete();
        });
    }
}
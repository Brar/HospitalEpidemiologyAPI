using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalEpidemiology.Migrations
{
    /// <inheritdoc />
    public partial class ExclusionConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:btree_gist", ",,");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays ADD CONSTRAINT patient_bed_location_stays_bed_location_id_stay_excl EXCLUDE USING GIST (bed_location_id WITH =, stay WITH &&)");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays ADD CONSTRAINT patient_bed_location_stays_patient_id_stay_excl EXCLUDE USING GIST (patient_id WITH =, stay WITH &&)");
            migrationBuilder.Sql("ALTER TABLE patient_room_stays ADD CONSTRAINT patient_room_stays_room_id_stay_excl EXCLUDE USING GIST (room_id WITH =, stay WITH &&)");
            migrationBuilder.Sql("ALTER TABLE patient_room_stays ADD CONSTRAINT patient_room_stays_patient_id_stay_excl EXCLUDE USING GIST (patient_id WITH =, stay WITH &&)");
            migrationBuilder.Sql("ALTER TABLE patient_unit_stays ADD CONSTRAINT patient_unit_stays_unit_id_stay_excl EXCLUDE USING GIST (unit_id WITH =, stay WITH &&)");
            migrationBuilder.Sql("ALTER TABLE patient_unit_stays ADD CONSTRAINT patient_unit_stays_patient_id_stay_excl EXCLUDE USING GIST (patient_id WITH =, stay WITH &&)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_unit_stays_patient_id_stay_excl");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_unit_stays_unit_id_stay_excl");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_room_stays_patient_id_stay_excl");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_room_stays_room_id_stay_excl");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_bed_location_stays_patient_id_stay_excl");
            migrationBuilder.Sql("ALTER TABLE patient_bed_location_stays DROP CONSTRAINT patient_bed_location_stays_bed_location_id_stay_excl");
            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:btree_gist", ",,");
        }
    }
}

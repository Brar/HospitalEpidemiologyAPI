using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using NpgsqlTypes;

#nullable disable

namespace HospitalEpidemiology.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hospitals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hospitals", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    family_name = table.Column<string>(type: "text", nullable: false),
                    given_name = table.Column<string>(type: "text", nullable: false),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    hospital_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_units_hospitals_hospital_id",
                        column: x => x.hospital_id,
                        principalTable: "hospitals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_unit_stays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    unit_id = table.Column<int>(type: "integer", nullable: false),
                    stay = table.Column<NpgsqlRange<DateTime>>(type: "tstzrange", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_unit_stays", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_unit_stays_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_patient_unit_stays_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    unit_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rooms", x => x.id);
                    table.ForeignKey(
                        name: "fk_rooms_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bed_locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    room_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bed_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_bed_locations_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_room_stays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    stay = table.Column<NpgsqlRange<DateTime>>(type: "tstzrange", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_room_stays", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_room_stays_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_patient_room_stays_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patient_bed_location_stays",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    patient_id = table.Column<int>(type: "integer", nullable: false),
                    bed_location_id = table.Column<int>(type: "integer", nullable: false),
                    stay = table.Column<NpgsqlRange<DateTime>>(type: "tstzrange", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_patient_bed_location_stays", x => x.id);
                    table.ForeignKey(
                        name: "fk_patient_bed_location_stays_bed_locations_bed_location_id",
                        column: x => x.bed_location_id,
                        principalTable: "bed_locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_patient_bed_location_stays_patients_patient_id",
                        column: x => x.patient_id,
                        principalTable: "patients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_bed_locations_room_id",
                table: "bed_locations",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_bed_location_stays_bed_location_id",
                table: "patient_bed_location_stays",
                column: "bed_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_bed_location_stays_patient_id",
                table: "patient_bed_location_stays",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_room_stays_patient_id",
                table: "patient_room_stays",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_room_stays_room_id",
                table: "patient_room_stays",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_unit_stays_patient_id",
                table: "patient_unit_stays",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "ix_patient_unit_stays_unit_id",
                table: "patient_unit_stays",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_unit_id",
                table: "rooms",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_units_hospital_id",
                table: "units",
                column: "hospital_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patient_bed_location_stays");

            migrationBuilder.DropTable(
                name: "patient_room_stays");

            migrationBuilder.DropTable(
                name: "patient_unit_stays");

            migrationBuilder.DropTable(
                name: "bed_locations");

            migrationBuilder.DropTable(
                name: "patients");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "hospitals");
        }
    }
}

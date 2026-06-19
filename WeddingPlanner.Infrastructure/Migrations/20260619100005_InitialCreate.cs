using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeddingPlanner.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Partneri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kontakt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Provizija = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DodatniPodaci = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partneri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoviVjencanja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoviVjencanja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bendovi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    JeDJ = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bendovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bendovi_Partneri_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partneri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cvjecare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cvjecare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cvjecare_Partneri_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partneri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantSaloni",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: false),
                    TipUsluge = table.Column<int>(type: "int", nullable: false),
                    BrojStolova = table.Column<int>(type: "int", nullable: false),
                    MjestaPoBrojStolu = table.Column<int>(type: "int", nullable: false),
                    CijenaPoOsobi = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CijenaSale = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantSaloni", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantSaloni_Partneri_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partneri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slasticarnice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slasticarnice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slasticarnice_Partneri_PartnerId",
                        column: x => x.PartnerId,
                        principalTable: "Partneri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dogadaji",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NazivPara = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatumDogadaja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipVjencanjaId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Napomena = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dogadaji", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dogadaji_TipoviVjencanja_TipVjencanjaId",
                        column: x => x.TipVjencanjaId,
                        principalTable: "TipoviVjencanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CijeneBendova",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BendId = table.Column<int>(type: "int", nullable: false),
                    Kategorija = table.Column<int>(type: "int", nullable: false),
                    TrajanjeH = table.Column<int>(type: "int", nullable: false),
                    Iznos = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CijeneBendova", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CijeneBendova_Bendovi_BendId",
                        column: x => x.BendId,
                        principalTable: "Bendovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playliste",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BendId = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Zanr = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playliste", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playliste_Bendovi_BendId",
                        column: x => x.BendId,
                        principalTable: "Bendovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CvjecarskiAranzmani",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CvjecaraId = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvjecarskiAranzmani", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CvjecarskiAranzmani_Cvjecare_CvjecaraId",
                        column: x => x.CvjecaraId,
                        principalTable: "Cvjecare",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlasticarskeStavke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlasticarnicaId = table.Column<int>(type: "int", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategorija = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlasticarskeStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlasticarskeStavke_Slasticarnice_SlasticarnicaId",
                        column: x => x.SlasticarnicaId,
                        principalTable: "Slasticarnice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RacunStavke",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DogadajId = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    IznosOsnovni = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Provizija = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacunStavke", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RacunStavke_Dogadaji_DogadajId",
                        column: x => x.DogadajId,
                        principalTable: "Dogadaji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DogadajId = table.Column<int>(type: "int", nullable: false),
                    BendId = table.Column<int>(type: "int", nullable: false),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Potvrdena = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Bendovi_BendId",
                        column: x => x.BendId,
                        principalTable: "Bendovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Dogadaji_DogadajId",
                        column: x => x.DogadajId,
                        principalTable: "Dogadaji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bendovi_PartnerId",
                table: "Bendovi",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CijeneBendova_BendId",
                table: "CijeneBendova",
                column: "BendId");

            migrationBuilder.CreateIndex(
                name: "IX_Cvjecare_PartnerId",
                table: "Cvjecare",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CvjecarskiAranzmani_CvjecaraId",
                table: "CvjecarskiAranzmani",
                column: "CvjecaraId");

            migrationBuilder.CreateIndex(
                name: "IX_Dogadaji_TipVjencanjaId",
                table: "Dogadaji",
                column: "TipVjencanjaId");

            migrationBuilder.CreateIndex(
                name: "IX_Playliste_BendId",
                table: "Playliste",
                column: "BendId");

            migrationBuilder.CreateIndex(
                name: "IX_RacunStavke_DogadajId",
                table: "RacunStavke",
                column: "DogadajId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantSaloni_PartnerId",
                table: "RestaurantSaloni",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_BendId",
                table: "Rezervacije",
                column: "BendId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_DogadajId",
                table: "Rezervacije",
                column: "DogadajId");

            migrationBuilder.CreateIndex(
                name: "IX_Slasticarnice_PartnerId",
                table: "Slasticarnice",
                column: "PartnerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlasticarskeStavke_SlasticarnicaId",
                table: "SlasticarskeStavke",
                column: "SlasticarnicaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CijeneBendova");

            migrationBuilder.DropTable(
                name: "CvjecarskiAranzmani");

            migrationBuilder.DropTable(
                name: "Playliste");

            migrationBuilder.DropTable(
                name: "RacunStavke");

            migrationBuilder.DropTable(
                name: "RestaurantSaloni");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "SlasticarskeStavke");

            migrationBuilder.DropTable(
                name: "Cvjecare");

            migrationBuilder.DropTable(
                name: "Bendovi");

            migrationBuilder.DropTable(
                name: "Dogadaji");

            migrationBuilder.DropTable(
                name: "Slasticarnice");

            migrationBuilder.DropTable(
                name: "TipoviVjencanja");

            migrationBuilder.DropTable(
                name: "Partneri");
        }
    }
}

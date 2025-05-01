using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBillSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calls",
                columns: table => new
                {
                    Call_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Costs = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    Paid = table.Column<bool>(type: "bit", nullable: false),
                    CallTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Bill_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Calls__19E7F093973A4759", x => x.Call_id);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    PhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    ProgramName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    ProgramPaid = table.Column<bool>(type: "bit", nullable: false),
                    ProgramEnds = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Phones__85FB4E392FAA14E1", x => x.PhoneNumber);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    ProgramName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    Charge = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Programs__4F925711C949785D", x => x.ProgramName);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    First_Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Last_Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Property = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__206A9DF8876E0A22", x => x.User_id);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Bill_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    BillPaid = table.Column<bool>(type: "bit", nullable: false),
                    Costs = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PhoneNumber1 = table.Column<string>(type: "varchar(15)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bills__CF6F718BF824A4E8", x => x.Bill_id);
                    table.ForeignKey(
                        name: "FK_Bills_Phones_PhoneNumber1",
                        column: x => x.PhoneNumber1,
                        principalTable: "Phones",
                        principalColumn: "PhoneNumber");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    Admin_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__4A311D2FB30E6136", x => x.Admin_id);
                    table.ForeignKey(
                        name: "FK__Admin__User_id__6E01572D",
                        column: x => x.User_id,
                        principalTable: "Users",
                        principalColumn: "User_id");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AFM = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    User_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Clients__75A2D320B56299E0", x => x.Client_id);
                    table.ForeignKey(
                        name: "FK__Clients__User_id__71D1E811",
                        column: x => x.User_id,
                        principalTable: "Users",
                        principalColumn: "User_id");
                });

            migrationBuilder.CreateTable(
                name: "Sellers",
                columns: table => new
                {
                    Seller_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sellers__016148B16AE97788", x => x.Seller_id);
                    table.ForeignKey(
                        name: "FK__Sellers__User_id__6B24EA82",
                        column: x => x.User_id,
                        principalTable: "Users",
                        principalColumn: "User_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_User_id",
                table: "Admin",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_PhoneNumber1",
                table: "Bills",
                column: "PhoneNumber1");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_User_id",
                table: "Clients",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Clients__85FB4E385F5B0B7A",
                table: "Clients",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_User_id",
                table: "Sellers",
                column: "User_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__536C85E43BDEACBF",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Calls");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Programs");

            migrationBuilder.DropTable(
                name: "Sellers");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

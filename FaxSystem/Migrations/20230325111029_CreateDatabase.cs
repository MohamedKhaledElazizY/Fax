using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FaxSystem.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AGENCIES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AGENCIES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "BRANCHES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRANCHES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DECISIONS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Opinion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OpinionVoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionCheck = table.Column<bool>(type: "bit", nullable: false),
                    PersonalReview = table.Column<bool>(type: "bit", nullable: false),
                    DecisionVoice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DECISIONS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LOGS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTakerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOGS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ROLES",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLES", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "USERS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchID = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USERS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_USERS_BRANCHES_BranchID",
                        column: x => x.BranchID,
                        principalTable: "BRANCHES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FAXBRANCHES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNum = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SenderBranchID = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    suspend = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DecisionID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAXBRANCHES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FAXBRANCHES_BRANCHES_SenderBranchID",
                        column: x => x.SenderBranchID,
                        principalTable: "BRANCHES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FAXBRANCHES_DECISIONS_DecisionID",
                        column: x => x.DecisionID,
                        principalTable: "DECISIONS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "FAXES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegistrationNum = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    suspend = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    SenderAgencyID = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    DecisionID = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAXES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FAXES_AGENCIES_SenderAgencyID",
                        column: x => x.SenderAgencyID,
                        principalTable: "AGENCIES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FAXES_DECISIONS_DecisionID",
                        column: x => x.DecisionID,
                        principalTable: "DECISIONS",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "USER_ROLES",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_ROLES", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_USER_ROLES_ROLES_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ROLES",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USER_ROLES_USERS_UserId",
                        column: x => x.UserId,
                        principalTable: "USERS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BRANCH_FAX_RECIVER",
                columns: table => new
                {
                    FaxID = table.Column<int>(type: "int", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BRANCH_FAX_RECIVER", x => new { x.BranchID, x.FaxID });
                    table.ForeignKey(
                        name: "FK_BRANCH_FAX_RECIVER_BRANCHES_BranchID",
                        column: x => x.BranchID,
                        principalTable: "BRANCHES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_BRANCH_FAX_RECIVER_FAXBRANCHES_FaxID",
                        column: x => x.FaxID,
                        principalTable: "FAXBRANCHES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FAXERECIVERS",
                columns: table => new
                {
                    FaxID = table.Column<int>(type: "int", nullable: false),
                    BranchID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAXERECIVERS", x => new { x.BranchID, x.FaxID });
                    table.ForeignKey(
                        name: "FK_FAXERECIVERS_BRANCHES_BranchID",
                        column: x => x.BranchID,
                        principalTable: "BRANCHES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FAXERECIVERS_FAXES_FaxID",
                        column: x => x.FaxID,
                        principalTable: "FAXES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FaxLinks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FaxId = table.Column<int>(type: "int", nullable: true),
                    FaxBetweenBranchesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaxLinks", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FaxLinks_FAXBRANCHES_FaxBetweenBranchesID",
                        column: x => x.FaxBetweenBranchesID,
                        principalTable: "FAXBRANCHES",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_FaxLinks_FAXES_FaxId",
                        column: x => x.FaxId,
                        principalTable: "FAXES",
                        principalColumn: "ID");
                });

            migrationBuilder.InsertData(
                table: "BRANCHES",
                columns: new[] { "ID", "Name" },
                values: new object[] { 1, "فرع النظم" });

            migrationBuilder.InsertData(
                table: "ROLES",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1, "Super Admin" },
                    { 2, "صلاحية تعديل المكاتبات" },
                    { 3, "صلاحية المتابعة" },
                    { 4, "صلاحية اتخاذ قرار" },
                    { 5, "صلاحية اضافة رأي" },
                    { 6, "رئيس فرع المتابعة" },
                    { 1006, "صلاحية التكويد" }
                });

            migrationBuilder.InsertData(
                table: "USERS",
                columns: new[] { "ID", "BranchID", "Password", "UserName" },
                values: new object[] { 1, 1, "Hossam@1983", "admin" });

            migrationBuilder.InsertData(
                table: "USER_ROLES",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_AGENCIES_Name",
                table: "AGENCIES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BRANCH_FAX_RECIVER_FaxID",
                table: "BRANCH_FAX_RECIVER",
                column: "FaxID");

            migrationBuilder.CreateIndex(
                name: "IX_BRANCHES_Name",
                table: "BRANCHES",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAXBRANCHES_DecisionID",
                table: "FAXBRANCHES",
                column: "DecisionID");

            migrationBuilder.CreateIndex(
                name: "IX_FAXBRANCHES_RegistrationNum",
                table: "FAXBRANCHES",
                column: "RegistrationNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAXBRANCHES_SenderBranchID",
                table: "FAXBRANCHES",
                column: "SenderBranchID");

            migrationBuilder.CreateIndex(
                name: "IX_FAXERECIVERS_FaxID",
                table: "FAXERECIVERS",
                column: "FaxID");

            migrationBuilder.CreateIndex(
                name: "IX_FAXES_DecisionID",
                table: "FAXES",
                column: "DecisionID");

            migrationBuilder.CreateIndex(
                name: "IX_FAXES_RegistrationNum",
                table: "FAXES",
                column: "RegistrationNum",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FAXES_SenderAgencyID",
                table: "FAXES",
                column: "SenderAgencyID");

            migrationBuilder.CreateIndex(
                name: "IX_FaxLinks_FaxBetweenBranchesID",
                table: "FaxLinks",
                column: "FaxBetweenBranchesID");

            migrationBuilder.CreateIndex(
                name: "IX_FaxLinks_FaxId",
                table: "FaxLinks",
                column: "FaxId");

            migrationBuilder.CreateIndex(
                name: "IX_USER_ROLES_RoleId",
                table: "USER_ROLES",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_BranchID",
                table: "USERS",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_USERS_UserName",
                table: "USERS",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BRANCH_FAX_RECIVER");

            migrationBuilder.DropTable(
                name: "FAXERECIVERS");

            migrationBuilder.DropTable(
                name: "FaxLinks");

            migrationBuilder.DropTable(
                name: "LOGS");

            migrationBuilder.DropTable(
                name: "USER_ROLES");

            migrationBuilder.DropTable(
                name: "FAXBRANCHES");

            migrationBuilder.DropTable(
                name: "FAXES");

            migrationBuilder.DropTable(
                name: "ROLES");

            migrationBuilder.DropTable(
                name: "USERS");

            migrationBuilder.DropTable(
                name: "AGENCIES");

            migrationBuilder.DropTable(
                name: "DECISIONS");

            migrationBuilder.DropTable(
                name: "BRANCHES");
        }
    }
}

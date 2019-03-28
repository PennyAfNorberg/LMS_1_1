using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS_1_1.Migrations
{
    public partial class addcolors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LMSUserId = table.Column<string>(nullable: true),
                    CourseId = table.Column<Guid>(nullable: true),
                    LMSActivityId = table.Column<Guid>(nullable: true),
                    AktivityTypeID = table.Column<int>(nullable: true),
                    Color = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorActivity_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ColorActivity_LMSActivity_LMSActivityId",
                        column: x => x.LMSActivityId,
                        principalTable: "LMSActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ColorActivity_AspNetUsers_LMSUserId",
                        column: x => x.LMSUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ColorModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LMSUserId = table.Column<string>(nullable: true),
                    ModuleId = table.Column<Guid>(nullable: true),
                    Color = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ColorModule_AspNetUsers_LMSUserId",
                        column: x => x.LMSUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ColorModule_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ColorActivity",
                columns: new[] { "Id", "AktivityTypeID", "Color", "CourseId", "LMSActivityId", "LMSUserId" },
                values: new object[,]
                {
                    { new Guid("cbf72f94-ef85-4b65-a5ee-3668902fcdc4"), 1, "#587aad", null, null, null },
                    { new Guid("d3693ad6-e8ea-4411-a24b-8b8c0e39b5c9"), 2, "#68c930", null, null, null },
                    { new Guid("15a3e14a-d3ba-424e-9eb9-ff49c9b19541"), 3, "#c95e30", null, null, null },
                    { new Guid("dea83bc8-857f-4094-a3a5-0a4bffbd023d"), 4, "#f45004", null, null, null },
                    { new Guid("38dac4b1-239d-45c0-8190-303792ec824a"), 5, "#fcfaf9", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ColorModule",
                columns: new[] { "Id", "Color", "LMSUserId", "ModuleId" },
                values: new object[] { new Guid("46b50676-6072-49ee-9c51-e9cff757112d"), "#dbad95", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_ColorActivity_CourseId",
                table: "ColorActivity",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorActivity_LMSActivityId",
                table: "ColorActivity",
                column: "LMSActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorActivity_LMSUserId",
                table: "ColorActivity",
                column: "LMSUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorModule_LMSUserId",
                table: "ColorModule",
                column: "LMSUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorModule_ModuleId",
                table: "ColorModule",
                column: "ModuleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorActivity");

            migrationBuilder.DropTable(
                name: "ColorModule");
        }
    }
}

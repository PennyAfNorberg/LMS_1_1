using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS_1_1.Migrations
{
    public partial class add_coursesettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("15a3e14a-d3ba-424e-9eb9-ff49c9b19541"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("38dac4b1-239d-45c0-8190-303792ec824a"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("cbf72f94-ef85-4b65-a5ee-3668902fcdc4"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("d3693ad6-e8ea-4411-a24b-8b8c0e39b5c9"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("dea83bc8-857f-4094-a3a5-0a4bffbd023d"));

            migrationBuilder.DeleteData(
                table: "ColorModule",
                keyColumn: "Id",
                keyValue: new Guid("46b50676-6072-49ee-9c51-e9cff757112d"));

            migrationBuilder.CreateTable(
                name: "CourseSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<string>(nullable: true),
                    StartLunch = table.Column<string>(nullable: true),
                    EndLunch = table.Column<string>(nullable: true),
                    EndTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseSettings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ColorActivity",
                columns: new[] { "Id", "AktivityTypeID", "Color", "CourseId", "LMSActivityId", "LMSUserId" },
                values: new object[,]
                {
                    { new Guid("cdd4f0c6-d395-4194-b2f8-e69b8a01c57e"), 1, "#587aad", null, null, null },
                    { new Guid("724a0244-c183-446e-8a96-60ca91d8aac3"), 2, "#68c930", null, null, null },
                    { new Guid("e050874b-27af-4870-8c2b-04e989081a5e"), 3, "#c95e30", null, null, null },
                    { new Guid("e61ff4dd-7ac6-4e86-a1e8-fd84d09f46ac"), 4, "#f45004", null, null, null },
                    { new Guid("3583af22-9b65-4420-865c-fa2ac9cd724b"), 5, "#fcfaf9", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ColorModule",
                columns: new[] { "Id", "Color", "LMSUserId", "ModuleId" },
                values: new object[] { new Guid("8b8f0562-90d2-498d-b9b5-796f5486d5ba"), "#dbad95", null, null });

            migrationBuilder.InsertData(
                table: "CourseSettings",
                columns: new[] { "Id", "CourseId", "Date", "EndLunch", "EndTime", "StartLunch", "StartTime" },
                values: new object[] { new Guid("3c2db520-45da-4775-827e-7bb03fa17aaf"), null, null, "13:00:00", "17:00:00", "12:00:00", "09:00:00" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSettings_CourseId",
                table: "CourseSettings",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSettings");

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("3583af22-9b65-4420-865c-fa2ac9cd724b"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("724a0244-c183-446e-8a96-60ca91d8aac3"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("cdd4f0c6-d395-4194-b2f8-e69b8a01c57e"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("e050874b-27af-4870-8c2b-04e989081a5e"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("e61ff4dd-7ac6-4e86-a1e8-fd84d09f46ac"));

            migrationBuilder.DeleteData(
                table: "ColorModule",
                keyColumn: "Id",
                keyValue: new Guid("8b8f0562-90d2-498d-b9b5-796f5486d5ba"));

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
        }
    }
}

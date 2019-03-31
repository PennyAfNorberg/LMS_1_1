using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS_1_1.Migrations
{
    public partial class changecoursettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DeleteData(
                table: "CourseSettings",
                keyColumn: "Id",
                keyValue: new Guid("3c2db520-45da-4775-827e-7bb03fa17aaf"));

            migrationBuilder.DropColumn(
                name: "EndLunch",
                table: "CourseSettings");

            migrationBuilder.DropColumn(
                name: "StartLunch",
                table: "CourseSettings");

            migrationBuilder.InsertData(
                table: "ColorActivity",
                columns: new[] { "Id", "AktivityTypeID", "Color", "CourseId", "LMSActivityId", "LMSUserId" },
                values: new object[,]
                {
                    { new Guid("b99ad5c1-07c4-4533-a2be-9fb94079acda"), 1, "#587aad", null, null, null },
                    { new Guid("bd50bf65-c056-4488-b78f-7f9eafd83f21"), 2, "#68c930", null, null, null },
                    { new Guid("e7c9bc2e-6301-4262-a731-c229462b2029"), 3, "#c95e30", null, null, null },
                    { new Guid("37f83384-4411-4a46-ac8a-c610821862f7"), 4, "#f45004", null, null, null },
                    { new Guid("1b2ad0dd-c7de-443d-9e75-5bb821299710"), 5, "#fcfaf9", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ColorModule",
                columns: new[] { "Id", "Color", "LMSUserId", "ModuleId" },
                values: new object[] { new Guid("1eb6e978-2fb3-4e70-812c-cba0d0114df3"), "#dbad95", null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("1b2ad0dd-c7de-443d-9e75-5bb821299710"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("37f83384-4411-4a46-ac8a-c610821862f7"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("b99ad5c1-07c4-4533-a2be-9fb94079acda"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("bd50bf65-c056-4488-b78f-7f9eafd83f21"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("e7c9bc2e-6301-4262-a731-c229462b2029"));

            migrationBuilder.DeleteData(
                table: "ColorModule",
                keyColumn: "Id",
                keyValue: new Guid("1eb6e978-2fb3-4e70-812c-cba0d0114df3"));

            migrationBuilder.AddColumn<string>(
                name: "EndLunch",
                table: "CourseSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartLunch",
                table: "CourseSettings",
                nullable: true);

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
        }
    }
}

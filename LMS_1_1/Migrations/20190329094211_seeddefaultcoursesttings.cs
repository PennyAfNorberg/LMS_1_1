using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LMS_1_1.Migrations
{
    public partial class seeddefaultcoursesttings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "ColorActivity",
                columns: new[] { "Id", "AktivityTypeID", "Color", "CourseId", "LMSActivityId", "LMSUserId" },
                values: new object[,]
                {
                    { new Guid("a621799a-47b2-45e8-8d1f-2f256fb68a0a"), 1, "#587aad", null, null, null },
                    { new Guid("02422a7d-99ec-40e2-bd1b-8bf4473a9cf2"), 2, "#68c930", null, null, null },
                    { new Guid("2c7b43b8-06aa-412d-8c09-5eb00b1425e4"), 3, "#c95e30", null, null, null },
                    { new Guid("b9b95bd2-7820-45d9-9354-8dd9437fd9bd"), 4, "#f45004", null, null, null },
                    { new Guid("450dbee0-90bd-4245-a45d-087881ceaa4d"), 5, "#fcfaf9", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ColorModule",
                columns: new[] { "Id", "Color", "LMSUserId", "ModuleId" },
                values: new object[] { new Guid("d9a5a7b1-5e72-43bc-839c-26a65c2869d7"), "#dbad95", null, null });

            migrationBuilder.InsertData(
                table: "CourseSettings",
                columns: new[] { "Id", "CourseId", "Date", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { new Guid("dee8ad3e-18fe-414d-8a1d-9461a5e00b84"), null, null, "12:00:00", "09:00:00" },
                    { new Guid("0ae1450b-86f5-45f6-8f2c-bfc0a2f3292a"), null, null, "17:00:00", "13:00:00" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("02422a7d-99ec-40e2-bd1b-8bf4473a9cf2"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("2c7b43b8-06aa-412d-8c09-5eb00b1425e4"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("450dbee0-90bd-4245-a45d-087881ceaa4d"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("a621799a-47b2-45e8-8d1f-2f256fb68a0a"));

            migrationBuilder.DeleteData(
                table: "ColorActivity",
                keyColumn: "Id",
                keyValue: new Guid("b9b95bd2-7820-45d9-9354-8dd9437fd9bd"));

            migrationBuilder.DeleteData(
                table: "ColorModule",
                keyColumn: "Id",
                keyValue: new Guid("d9a5a7b1-5e72-43bc-839c-26a65c2869d7"));

            migrationBuilder.DeleteData(
                table: "CourseSettings",
                keyColumn: "Id",
                keyValue: new Guid("0ae1450b-86f5-45f6-8f2c-bfc0a2f3292a"));

            migrationBuilder.DeleteData(
                table: "CourseSettings",
                keyColumn: "Id",
                keyValue: new Guid("dee8ad3e-18fe-414d-8a1d-9461a5e00b84"));

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
    }
}

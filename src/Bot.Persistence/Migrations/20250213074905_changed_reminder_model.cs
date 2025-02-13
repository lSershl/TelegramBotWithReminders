using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changed_reminder_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pre_reminder_job_id",
                table: "reminders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "reminder_job_id",
                table: "reminders",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pre_reminder_job_id",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "reminder_job_id",
                table: "reminders");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Profile.Utility.Migrations
{
    /// <inheritdoc />
    public partial class FixAppUserIdOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only fix the AppUserId column - don't recreate the whole schema
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    -- Check if the column exists and is of type text
                    IF EXISTS (
                        SELECT 1 FROM information_schema.columns
                        WHERE table_name='Employee' AND column_name='AppUserId'
                        AND data_type = 'text'
                    ) THEN
                        -- Clean up invalid data
                        UPDATE ""Employee""
                        SET ""AppUserId"" = NULL
                        WHERE ""AppUserId"" = '' OR ""AppUserId"" IS NULL;

                        -- Alter column with explicit casting
                        ALTER TABLE ""Employee""
                        ALTER COLUMN ""AppUserId"" TYPE uuid
                        USING ""AppUserId""::uuid;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback - change back to text
            migrationBuilder.Sql(@"
                ALTER TABLE ""Employee""
                ALTER COLUMN ""AppUserId"" TYPE text
                USING ""AppUserId""::text;
            ");
        }
    }
}
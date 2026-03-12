using Microsoft.EntityFrameworkCore.Migrations;

namespace Recruit.Utility.Migrations
{
    public partial class AddUniversalCodeGenerator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Create the code_counters table
            migrationBuilder.Sql(@"
            CREATE TABLE IF NOT EXISTS code_counters
            (
                module_name TEXT NOT NULL,
                year INT NULL,
                last_number BIGINT NOT NULL,
                PRIMARY KEY (module_name, year)
            );
        ");

            // 2️⃣ Create the universal generator function
            migrationBuilder.Sql(@"
            CREATE OR REPLACE FUNCTION generate_code(p_module TEXT, p_year INT DEFAULT NULL)
            RETURNS TEXT AS $$
            DECLARE
                next_number BIGINT;
                year_part TEXT := '';
            BEGIN
                -- include year part if provided
                IF p_year IS NOT NULL THEN
                    year_part := p_year::TEXT || '-';
                END IF;

                -- insert or update counter
                INSERT INTO code_counters(module_name, year, last_number)
                VALUES (p_module, p_year, 1)
                ON CONFLICT (module_name, year)
                DO UPDATE SET last_number = code_counters.last_number + 1
                RETURNING last_number INTO next_number;

                -- format code
                IF p_year IS NOT NULL THEN
                    RETURN p_module || '-' || year_part || LPAD(next_number::TEXT, 6, '0');
                ELSE
                    RETURN p_module || LPAD(next_number::TEXT, 5, '0');
                END IF;
            END;
            $$ LANGUAGE plpgsql;
        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop function first
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS generate_code(TEXT, INT);");

            // Then drop table
            migrationBuilder.Sql("DROP TABLE IF EXISTS code_counters;");
        }
    }
}

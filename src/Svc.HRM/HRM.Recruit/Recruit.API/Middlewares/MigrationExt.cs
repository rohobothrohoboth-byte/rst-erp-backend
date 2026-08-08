using Microsoft.EntityFrameworkCore;
using Recruit.Utility.Persistence;

namespace Recruit.API.Middlewares;

public static class MigrationExt
{
    public static void ApplyMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<HrmRecruitDbContext>();
        dbContext.Database.Migrate();

        // Create universal counter table + function
        var sql = @"        
        CREATE TABLE IF NOT EXISTS code_counters
        (
            module_name TEXT NOT NULL,
            year INT NULL,
            last_number BIGINT NOT NULL,
            PRIMARY KEY (module_name, year)
        );
        CREATE OR REPLACE FUNCTION generate_code(p_module TEXT, p_year INT DEFAULT NULL)
        RETURNS TEXT AS $$
        DECLARE
            next_number BIGINT;
            year_part TEXT := '';
        BEGIN

            IF p_year IS NOT NULL THEN
                year_part := p_year::TEXT || '-';
            END IF;

            INSERT INTO code_counters(module_name, year, last_number)
            VALUES (p_module, p_year, 1)
            ON CONFLICT (module_name, year)
            DO UPDATE SET last_number = code_counters.last_number + 1
            RETURNING last_number INTO next_number;

            IF p_year IS NOT NULL THEN
                RETURN p_module || '-' || year_part || LPAD(next_number::TEXT, 6, '0');
            ELSE
                RETURN p_module || LPAD(next_number::TEXT, 5, '0');
            END IF;

        END;
        $$ LANGUAGE plpgsql;
        ";

        dbContext.Database.ExecuteSqlRaw(sql);
    }
}
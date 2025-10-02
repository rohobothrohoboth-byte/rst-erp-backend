using Svc.Lup.Models;

namespace Svc.Lup.Persistence;

public static class LupSeeder
{
    public static async Task SeedAsync(LupDbContext context)
    {
        if (!context.AbsentReason.Any())
        {
            await context.AbsentReason.AddRangeAsync(
                new AbsentReason { Id = Guid.NewGuid(), Code = "L", Name = "Late" },
                new AbsentReason { Id = Guid.NewGuid(), Code = "D", Name = "DD" },
                new AbsentReason { Id = Guid.NewGuid(), Code = "A", Name = "Absent" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.AddressType.Any())
        {
            await context.AddressType.AddRangeAsync(
                new AddressType { Id = Guid.NewGuid(), Name = "Residence" },
                new AddressType { Id = Guid.NewGuid(), Name = "Work Place" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.AdmissionType.Any())
        {
            await context.AdmissionType.AddRangeAsync(
                new AdmissionType { Id = Guid.NewGuid(), Name = "Regular" },
                new AdmissionType { Id = Guid.NewGuid(), Name = "Extension" },
                new AdmissionType { Id = Guid.NewGuid(), Name = "Distance" },
                new AdmissionType { Id = Guid.NewGuid(), Name = "Unknown" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.AwardReason.Any())
        {
            await context.AwardReason.AddRangeAsync(
                new AwardReason { Id = Guid.NewGuid(), Name = "Good Manner" },
                new AwardReason { Id = Guid.NewGuid(), Name = "Better Service" },
                new AwardReason { Id = Guid.NewGuid(), Name = "Other" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.AwardType.Any())
        {
            await context.AwardType.AddRangeAsync(
                new AwardType { Id = Guid.NewGuid(), Name = "Certificate" },
                new AwardType { Id = Guid.NewGuid(), Name = "Cash" },
                new AwardType { Id = Guid.NewGuid(), Name = "Other" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.CommitteeRole.Any())
        {
            await context.CommitteeRole.AddRangeAsync(
                new CommitteeRole { Id = Guid.NewGuid(), Name = "Member" },
                new CommitteeRole { Id = Guid.NewGuid(), Name = "Chair Person" },
                new CommitteeRole { Id = Guid.NewGuid(), Name = "Secretary" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.CriterionType.Any())
        {
            await context.CriterionType.AddRangeAsync(
                new CriterionType { Id = Guid.NewGuid(), Name = "Education Type" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Practical Exam" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Work Experience" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Clean from any Disciplinary Case" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Armed Exam" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Interview" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Document" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Physical Appearance" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Education Level" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Written Exam" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Work Experience In Company" },
                new CriterionType { Id = Guid.NewGuid(), Name = "Performance" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.EmploymentNature.Any())
        {
            await context.EmploymentNature.AddRangeAsync(
                new EmploymentNature { Id = Guid.NewGuid(), Name = "Permanent" },
                new EmploymentNature { Id = Guid.NewGuid(), Name = "Contract" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.EmploymentType.Any())
        {
            await context.EmploymentType.AddRangeAsync(
                new EmploymentType { Id = Guid.NewGuid(), Name = "በለቀቁ ምትክ" },
                new EmploymentType { Id = Guid.NewGuid(), Name = "አዲስ በመከፈቱ" },
                new EmploymentType { Id = Guid.NewGuid(), Name = "ተጨማሪ በማስፈለጉ" },
                new EmploymentType { Id = Guid.NewGuid(), Name = "Old Employee" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.EducationLevel.Any())
        {
            await context.EducationLevel.AddRangeAsync(
                new EducationLevel { Id = Guid.NewGuid(), Name = "Preparatory" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "Collage" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "TVT" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "University" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "Elementary" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "None" },
                new EducationLevel { Id = Guid.NewGuid(), Name = "High School" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.HolidayCondition.Any())
        {
            await context.HolidayCondition.AddRangeAsync(
                new HolidayCondition { Id = Guid.NewGuid(), Name = "Half Day" },
                new HolidayCondition { Id = Guid.NewGuid(), Name = "Full Day" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.LanguageSkill.Any())
        {
            await context.LanguageSkill.AddRangeAsync(
                new LanguageSkill { Id = Guid.NewGuid(), Name = "Writing" },
                new LanguageSkill { Id = Guid.NewGuid(), Name = "Listening" },
                new LanguageSkill { Id = Guid.NewGuid(), Name = "Speaking" },
                new LanguageSkill { Id = Guid.NewGuid(), Name = "Reading" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.LeaveCondition.Any())
        {
            await context.LeaveCondition.AddRangeAsync(
                new LeaveCondition { Id = Guid.NewGuid(), Name = "With Half Salary" },
                new LeaveCondition { Id = Guid.NewGuid(), Name = "With No Salary" },
                new LeaveCondition { Id = Guid.NewGuid(), Name = "With Full Salary" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.LeaveType.Any())
        {
            await context.LeaveType.AddRangeAsync(
                new LeaveType { Id = Guid.NewGuid(), Name = "Annual Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Maternity Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Sick Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Paternity Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Court Case" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Mourning Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Wedding Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Prenatal Leave" },
                new LeaveType { Id = Guid.NewGuid(), Name = "Pregnancy Leave" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.LeaveUsage.Any())
        {
            await context.LeaveUsage.AddRangeAsync(
                new LeaveUsage { Id = Guid.NewGuid(), Name = "Half Day" },
                new LeaveUsage { Id = Guid.NewGuid(), Name = "Full Day" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.MaritalStatus.Any())
        {
            await context.MaritalStatus.AddRangeAsync(
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Not Married" },
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Single" },
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Married" },
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Widow/er" },
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Divorced" },
                new MaritalStatus { Id = Guid.NewGuid(), Name = "Not Mentioned" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.MeasureTaken.Any())
        {
            await context.MeasureTaken.AddRangeAsync(
                new MeasureTaken { Id = Guid.NewGuid(), Name = "Promoted" },
                new MeasureTaken { Id = Guid.NewGuid(), Name = "Salary Increment" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.MeasureType.Any())
        {
            await context.MeasureType.AddRangeAsync(
                new MeasureType { Id = Guid.NewGuid(), Name = "2 Days Salary" },
                new MeasureType { Id = Guid.NewGuid(), Name = "5 Days Salary" },
                new MeasureType { Id = Guid.NewGuid(), Name = "10 Days Salary" },
                new MeasureType { Id = Guid.NewGuid(), Name = "15 Days Salary" },
                new MeasureType { Id = Guid.NewGuid(), Name = "One Month Salary" },
                new MeasureType { Id = Guid.NewGuid(), Name = "Primary Written Warning" },
                new MeasureType { Id = Guid.NewGuid(), Name = "Secondary Written Warning" },
                new MeasureType { Id = Guid.NewGuid(), Name = "Final Written Warning" },
                new MeasureType { Id = Guid.NewGuid(), Name = "Transfer" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.PerformanceEvaluation.Any())
        {
            await context.PerformanceEvaluation.AddRangeAsync(
                new PerformanceEvaluation { Id = Guid.NewGuid(), Name = "Good" },
                new PerformanceEvaluation { Id = Guid.NewGuid(), Name = "V.Good" },
                new PerformanceEvaluation { Id = Guid.NewGuid(), Name = "Fair" },
                new PerformanceEvaluation { Id = Guid.NewGuid(), Name = "Excellent" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.PositionChangeReason.Any())
        {
            await context.PositionChangeReason.AddRangeAsync(
                new PositionChangeReason { Id = Guid.NewGuid(), Name = "Demotion" },
                new PositionChangeReason { Id = Guid.NewGuid(), Name = "Performance" },
                new PositionChangeReason { Id = Guid.NewGuid(), Name = "Computation" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.ProfessionType.Any())
        {
            await context.ProfessionType.AddRangeAsync(
                new ProfessionType { Id = Guid.NewGuid(), Name = "Professional" },
                new ProfessionType { Id = Guid.NewGuid(), Name = "Semi-Professional," },
                new ProfessionType { Id = Guid.NewGuid(), Name = "Non-Professional" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Quarter.Any())
        {
            await context.Quarter.AddRangeAsync(
                new Quarter { Id = Guid.NewGuid(), Name = "1st Quarter" },
                new Quarter { Id = Guid.NewGuid(), Name = "2nd Quarter" },
                new Quarter { Id = Guid.NewGuid(), Name = "3rd Quarter" },
                new Quarter { Id = Guid.NewGuid(), Name = "4th Quarter" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Rating.Any())
        {
            await context.Rating.AddRangeAsync(
                new Rating { Id = Guid.NewGuid(), Name = "V.Good" },
                new Rating { Id = Guid.NewGuid(), Name = "Good" },
                new Rating { Id = Guid.NewGuid(), Name = "Fair" },
                new Rating { Id = Guid.NewGuid(), Name = "Excellent" },
                new Rating { Id = Guid.NewGuid(), Name = "Unsatisfactory" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Region.Any())
        {
            await context.Region.AddRangeAsync(
                new Region { Id = Guid.NewGuid(), Name = "Amhara" },
                new Region { Id = Guid.NewGuid(), Name = "Diredawa" },
                new Region { Id = Guid.NewGuid(), Name = "Benishangul" },
                new Region { Id = Guid.NewGuid(), Name = "Somalia" },
                new Region { Id = Guid.NewGuid(), Name = "Gambella" },
                new Region { Id = Guid.NewGuid(), Name = "Benishangul" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.Relation.Any())
        {
            await context.Relation.AddRangeAsync(
                new Relation { Id = Guid.NewGuid(), Name = "Daughter" },
                new Relation { Id = Guid.NewGuid(), Name = "Father" },
                new Relation { Id = Guid.NewGuid(), Name = "Brother" },
                new Relation { Id = Guid.NewGuid(), Name = "Son" },
                new Relation { Id = Guid.NewGuid(), Name = "Aunt" },
                new Relation { Id = Guid.NewGuid(), Name = "Sister" },
                new Relation { Id = Guid.NewGuid(), Name = "Wife" },
                new Relation { Id = Guid.NewGuid(), Name = "Uncle" },
                new Relation { Id = Guid.NewGuid(), Name = "Husband" },
                new Relation { Id = Guid.NewGuid(), Name = "Mother" },
                new Relation { Id = Guid.NewGuid(), Name = "Child" },
                new Relation { Id = Guid.NewGuid(), Name = "UNKNOWN" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.ReportType.Any())
        {
            await context.ReportType.AddRangeAsync(
                new ReportType { Id = Guid.NewGuid(), Name = "Weekly" },
                new ReportType { Id = Guid.NewGuid(), Name = "Monthly" },
                new ReportType { Id = Guid.NewGuid(), Name = "3 Months" },
                new ReportType { Id = Guid.NewGuid(), Name = "6 Months" },
                new ReportType { Id = Guid.NewGuid(), Name = "9 Months" },
                new ReportType { Id = Guid.NewGuid(), Name = "Yearly" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.SalaryChangeReason.Any())
        {
            await context.SalaryChangeReason.AddRangeAsync(
                new SalaryChangeReason { Id = Guid.NewGuid(), Name = "Performance" },
                new SalaryChangeReason { Id = Guid.NewGuid(), Name = "Annual Increment" },
                new SalaryChangeReason { Id = Guid.NewGuid(), Name = "Promotion" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.SkillLevel.Any())
        {
            await context.SkillLevel.AddRangeAsync(
                new SkillLevel { Id = Guid.NewGuid(), Name = "Satisfactory" },
                new SkillLevel { Id = Guid.NewGuid(), Name = "Excellent" },
                new SkillLevel { Id = Guid.NewGuid(), Name = "Very Good" },
                new SkillLevel { Id = Guid.NewGuid(), Name = "Good" },
                new SkillLevel { Id = Guid.NewGuid(), Name = "Beginner" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.SponsorType.Any())
        {
            await context.SponsorType.AddRangeAsync(
                new SponsorType { Id = Guid.NewGuid(), Name = "Government" },
                new SponsorType { Id = Guid.NewGuid(), Name = "Self Sponsored" },
                new SponsorType { Id = Guid.NewGuid(), Name = "Unknown" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TerminationReason.Any())
        {
            await context.TerminationReason.AddRangeAsync(
                new TerminationReason { Id = Guid.NewGuid(), Name = "በሞት" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "Pension" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "Transfer" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "Termination of Contract" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "ሌላ/ባልታወቀ" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "በራሱ" },
                new TerminationReason { Id = Guid.NewGuid(), Name = "ስነ ምግባር" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TrainingSource.Any())
        {
            await context.TrainingSource.AddRangeAsync(
                new TrainingSource { Id = Guid.NewGuid(), Name = "Based on Training Need Analysis" },
                new TrainingSource { Id = Guid.NewGuid(), Name = "New" },
                new TrainingSource { Id = Guid.NewGuid(), Name = "Transferred From Last year" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TrainingType.Any())
        {
            await context.TrainingType.AddRangeAsync(
                new TrainingType { Id = Guid.NewGuid(), Name = "Promotional Training" },
                new TrainingType { Id = Guid.NewGuid(), Name = "Off The Job Training" },
                new TrainingType { Id = Guid.NewGuid(), Name = "On The Job Training" },
                new TrainingType { Id = Guid.NewGuid(), Name = "Induction Training" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.TransferReason.Any())
        {
            await context.TransferReason.AddRangeAsync(
                new TransferReason { Id = Guid.NewGuid(), Name = "በራሶ ጥያቄ" },
                new TransferReason { Id = Guid.NewGuid(), Name = "ዝውውር" },
                new TransferReason { Id = Guid.NewGuid(), Name = "ቅጣት" },
                new TransferReason { Id = Guid.NewGuid(), Name = "የተሻለ ስራ" },
                new TransferReason { Id = Guid.NewGuid(), Name = "Unknown" },
                new TransferReason { Id = Guid.NewGuid(), Name = "በስራ ምክንያት" }
            );
            await context.SaveChangesAsync();
        }

        if (!context.VoucherType.Any())
        {
            await context.VoucherType.AddRangeAsync(
                new VoucherType { Id = Guid.NewGuid(), Name = "PCPV" },
                new VoucherType { Id = Guid.NewGuid(), Name = "JV" },
                new VoucherType { Id = Guid.NewGuid(), Name = "BPV" },
                new VoucherType { Id = Guid.NewGuid(), Name = "CRV" }
            );
            await context.SaveChangesAsync();
        }

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}

        //if (!context.LeaveType.Any())
        //{
        //    await context.LeaveType.AddRangeAsync(
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" },
        //        new LeaveType { Id = Guid.NewGuid(), Name = "Add" }
        //    );
        //    await context.SaveChangesAsync();
        //}







    }
}

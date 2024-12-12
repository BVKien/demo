using Microsoft.EntityFrameworkCore;
using OJTEDU.Domain.Entities;
using OJTEDU.Domain.Interfaces;
using OJTEDU.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OJTEDU.Infrastructure.Repositories
{
    public class EvaluationRepository : IEvaluationRepository
    {
        private readonly OJTEDU_DB_V1Context _context;

        public EvaluationRepository(OJTEDU_DB_V1Context context)
        {
            _context = context;
        }

        // University, Company
        public async Task<Evaluation> CreateEvaluationAsync(int? internshipId, Evaluation? info)
        {
            try
            {
                // Internship 
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                // Check evaluation exist
                var evaluationExist = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                // Create 
                if (evaluationExist != null)
                {
                     
                }

                // Working report
                var workingReports = await _context.WorkingReports
                    .Where(wk => wk.StudentId == internship.StudentId)
                    .ToListAsync();

                if (workingReports == null)
                {
                    throw new Exception("Not found working report list for this internship.");
                }

                // If internship status = pass
                if (internship.Status == "2")
                {
                    // Calculate process scores -> 70%
                    var mentorProcessScore = (workingReports.Sum(wk => wk.MentorScore ?? 0)) / workingReports.Count();
                    var uniProcessScore = (workingReports.Sum(wk => wk.LecturerScore ?? 0)) / workingReports.Count();
                    var processScore = (((mentorProcessScore + uniProcessScore) / 2) * 70) / 100;

                    // Final score -> 30%
                    var finalScore = (((info?.CompanyScore + info?.DeanScore) / 2) * 30) / 100;

                    // Evaluation score 
                    var evaluationScore = processScore + finalScore;

                    var evaluation = new Evaluation
                    {
                        MentorId = internship.CompanyId,
                        LecturerId = internship.LecturerId,
                        StudentId = internship.StudentId,
                        CompanyComment = info?.CompanyComment,
                        DeanComment = info?.DeanComment,
                        CompanyScore = info?.CompanyScore,
                        DeanScore = info?.DeanScore,
                        EvaluationScore = evaluationScore,
                        Status = "1",
                        CreatedAt = GetVietnamTime(),
                        UpdatedAt = GetVietnamTime()
                    };

                    await _context.Evaluations.AddAsync(evaluation);
                    await _context.SaveChangesAsync();

                    return evaluation;
                }

                // If internship status = pass
                if (internship.Status == "0")
                {
                    var evaluation = new Evaluation
                    {
                        MentorId = internship.CompanyId,
                        LecturerId = internship.LecturerId,
                        StudentId = internship.StudentId,
                        CompanyComment = info?.CompanyComment,
                        DeanComment = info?.DeanComment,
                        CompanyScore = 0,
                        DeanScore = 0,
                        EvaluationScore = 0,
                        Status = "0",
                        CreatedAt = GetVietnamTime(),
                        UpdatedAt = GetVietnamTime()
                    };

                    await _context.Evaluations.AddAsync(evaluation);
                    await _context.SaveChangesAsync();

                    return evaluation;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //public async Task<Evaluation> CreateEvaluationAsync(int? internshipId, Evaluation? info)
        //{
        //    try
        //    {
        //        // Validate internship
        //        var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

        //        if (internship == null)
        //        {
        //            throw new Exception("Internship not found.");
        //        }

        //        // Check if an evaluation already exists
        //        var evaluationExist = await _context.Evaluations
        //            .FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

        //        if (evaluationExist != null)
        //        {
        //            // Check if DeanScore and DeanComment already exist
        //            if (evaluationExist.DeanScore != null && evaluationExist.DeanComment != null)
        //            {
        //                // Only allow updating CompanyScore and CompanyComment
        //                if (info?.CompanyScore == null || info.CompanyComment == null)
        //                {
        //                    throw new Exception("CompanyScore and CompanyComment are required for update.");
        //                }

        //                evaluationExist.CompanyScore = info.CompanyScore;
        //                evaluationExist.CompanyComment = info.CompanyComment;
        //            }
        //            // Check if CompanyScore and CompanyComment already exist
        //            else if (evaluationExist.CompanyScore != null && evaluationExist.CompanyComment != null)
        //            {
        //                // Only allow updating DeanScore and DeanComment
        //                if (info?.DeanScore == null || info.DeanComment == null)
        //                {
        //                    throw new Exception("DeanScore and DeanComment are required for update.");
        //                }

        //                evaluationExist.DeanScore = info.DeanScore;
        //                evaluationExist.DeanComment = info.DeanComment;
        //            }
        //            else
        //            {
        //                throw new Exception("Invalid update operation. One set of scores/comments must already exist.");
        //            }

        //            // Update evaluation score only if all scores are provided
        //            if (evaluationExist.CompanyScore != null && evaluationExist.DeanScore != null && internship.Status == "2")
        //            {
        //                var workingReports = await _context.WorkingReports
        //                    .Where(wk => wk.StudentId == internship.StudentId)
        //                    .ToListAsync();

        //                if (!workingReports.Any())
        //                {
        //                    throw new Exception("Working reports not found for this internship.");
        //                }

        //                var mentorProcessScore = workingReports.Sum(wk => wk.MentorScore ?? 0) / workingReports.Count();
        //                var uniProcessScore = workingReports.Sum(wk => wk.LecturerScore ?? 0) / workingReports.Count();
        //                var processScore = (((mentorProcessScore + uniProcessScore) / 2) * 70) / 100;

        //                var finalScore = (((evaluationExist.CompanyScore + evaluationExist.DeanScore) / 2) * 30) / 100;
        //                evaluationExist.EvaluationScore = Math.Round(processScore + finalScore, 2);
        //                evaluationExist.Status = "1";
        //            }

        //            evaluationExist.UpdatedAt = GetVietnamTime();
        //            await _context.SaveChangesAsync();
        //            return evaluationExist;
        //        }

        //        // If no evaluation exists, create a new one
        //        if (internship.Status == "2" && info?.CompanyScore != null && info.DeanScore != null)
        //        {
        //            var workingReports = await _context.WorkingReports
        //                .Where(wk => wk.StudentId == internship.StudentId)
        //                .ToListAsync();

        //            if (!workingReports.Any())
        //            {
        //                throw new Exception("Working reports not found for this internship.");
        //            }

        //            var mentorProcessScore = workingReports.Sum(wk => wk.MentorScore ?? 0) / workingReports.Count();
        //            var uniProcessScore = workingReports.Sum(wk => wk.LecturerScore ?? 0) / workingReports.Count();
        //            var processScore = (((mentorProcessScore + uniProcessScore) / 2) * 70) / 100;

        //            var finalScore = (((info.CompanyScore + info.DeanScore) / 2) * 30) / 100;

        //            var evaluation = new Evaluation
        //            {
        //                MentorId = internship.CompanyId,
        //                LecturerId = internship.LecturerId,
        //                StudentId = internship.StudentId,
        //                CompanyComment = info.CompanyComment,
        //                DeanComment = info.DeanComment,
        //                CompanyScore = info.CompanyScore,
        //                DeanScore = info.DeanScore,
        //                EvaluationScore = Math.Round(processScore + finalScore, 2),
        //                Status = "1",
        //                CreatedAt = GetVietnamTime(),
        //                UpdatedAt = GetVietnamTime()
        //            };

        //            await _context.Evaluations.AddAsync(evaluation);
        //            await _context.SaveChangesAsync();

        //            return evaluation;
        //        }

        //        // For internship status "0" (fail)
        //        if (internship.Status == "0")
        //        {
        //            var evaluation = new Evaluation
        //            {
        //                MentorId = internship.CompanyId,
        //                LecturerId = internship.LecturerId,
        //                StudentId = internship.StudentId,
        //                CompanyComment = info?.CompanyComment,
        //                DeanComment = info?.DeanComment,
        //                CompanyScore = 0,
        //                DeanScore = 0,
        //                EvaluationScore = 0,
        //                Status = "0",
        //                CreatedAt = GetVietnamTime(),
        //                UpdatedAt = GetVietnamTime()
        //            };

        //            await _context.Evaluations.AddAsync(evaluation);
        //            await _context.SaveChangesAsync();

        //            return evaluation;
        //        }

        //        throw new Exception("Invalid internship status or insufficient data.");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        // University, Company, Student
        public async Task<Evaluation> GetEvaluationDetailByUserId(int? userId)
        {
            {
                try
                {
                    var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);

                    if (student == null)
                    {
                        throw new Exception("Not found student.");
                    }

                    var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == student.StudentId);

                    if (evaluation == null)
                    {
                        throw new Exception("Not found evaluation information.");
                    }

                    return evaluation;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<Evaluation> GetEvaluationDetailByInternshipId(int? internshipId)
        {
            {
                try
                {
                    var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                    if (internship == null)
                    {
                        throw new Exception("Not found internship.");
                    }

                    var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == internship.StudentId);

                    if (student == null)
                    {
                        throw new Exception("Not found student.");
                    }

                    var evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == student.StudentId);

                    if (evaluation == null)
                    {
                        throw new Exception("Not found evaluation information.");
                    }

                    return evaluation;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        // VN tinme
        private DateTime GetVietnamTime()
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
        }
    }
}

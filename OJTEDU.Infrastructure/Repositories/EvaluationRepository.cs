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
        public async Task<Evaluation> CreateEvaluationAsync(int? userId, int? internshipId, Evaluation? info)
        {
            try
            {
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

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
                    if (user.Role.Name == "Mentor")
                    {
                        var newinfo = new Evaluation
                        {
                            CompanyComment = info.CompanyComment,
                            CompanyScore = info.CompanyScore,
                        };

                        UpdateEvaluationAsync(userId, internshipId, newinfo);
                        return newinfo;
                    }

                    if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                    {
                        var newinfo = new Evaluation
                        {
                            DeanComment = info.DeanComment,
                            DeanScore = info.DeanScore,
                        };

                        UpdateEvaluationAsync(userId, internshipId, newinfo);
                        return newinfo;
                    }
                }

                if (user.Role.Name == "Mentor")
                {
                    var newinfo = new Evaluation
                    {
                        CompanyComment = info.CompanyComment,
                        CompanyScore = info.CompanyScore,
                    };

                    await _context.AddAsync(newinfo);
                    _context.SaveChangesAsync();

                    return newinfo;
                }

                if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                {
                    var newinfo = new Evaluation
                    {
                        DeanComment = info.DeanComment,
                        DeanScore = info.DeanScore,
                    };

                    await _context.AddAsync(newinfo);
                    _context.SaveChangesAsync();
                    return newinfo;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evaluation> UpdateEvaluationAsync(int? userId, int? internshipId, Evaluation? info)
        {
            try
            {
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("Not found user.");
                }

                // Internship 
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.IntershipId == internshipId);

                if (internship == null)
                {
                    throw new Exception("Not found internship.");
                }

                // Check evaluation exist
                var evaluationExist = await _context.Evaluations.FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                if (evaluationExist == null)
                {
                    throw new Exception("Not found working report list for this evaluation.");
                }

                if (user.Role.Name == "Mentor")
                {
                    // Update 
                    evaluationExist.CompanyComment = info.CompanyComment;
                    evaluationExist.CompanyScore = info.CompanyScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                if (user.Role.Name == "Dean" || user.Role.Name == "Lecturer")
                {
                    // Update 
                    evaluationExist.DeanComment = info.DeanComment;
                    evaluationExist.DeanScore = info.DeanScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Evaluation> GetEvaluationScoreAsync(int? userId)
        {
            try
            {
                // Validate User
                var user = await _context.Users.Include(s => s.Role)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                // Validate Internship
                var internship = await _context.Internships.FirstOrDefaultAsync(i => i.Student.UserId == userId);
                if (internship == null)
                {
                    throw new Exception("Internship not found.");
                }

                // Check for Existing Evaluation
                var evaluationExist = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.StudentId == internship.StudentId);

                // Retrieve Working Reports
                var workingReports = await _context.WorkingReports
                    .Where(wk => wk.StudentId == internship.StudentId)
                    .ToListAsync();

                if (workingReports == null || !workingReports.Any())
                {
                    throw new Exception("No working reports found for this internship.");
                }

                // Define Evaluation Object
                Evaluation evaluation;

                // Case: Internship status = "pass" (status == "2")
                if (internship.Status == "2")
                {
                    // Calculate Process Scores (70%)
                    var mentorProcessScore = (workingReports.Sum(wk => wk.MentorScore ?? 0)) / workingReports.Count();
                    var uniProcessScore = (workingReports.Sum(wk => wk.LecturerScore ?? 0)) / workingReports.Count();
                    var processScore = (((mentorProcessScore + uniProcessScore) / 2) * 70) / 100;

                    // Final Score (30%)
                    var finalScore = (((evaluationExist.DeanScore + evaluationExist.CompanyScore) / 2) * 30) / 100;

                    // Total Evaluation Score
                    var evaluationScore = processScore + finalScore;

                    evaluationExist.EvaluationScore = evaluationScore;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }
                // Case: Internship status = "fail" (status == "0")
                if (internship.Status == "0")
                {
                    evaluationExist.EvaluationScore = 0;
                    evaluationExist.UpdatedAt = DateTime.Now;
                }

                // Save Evaluation to Database
                await _context.SaveChangesAsync();

                return evaluationExist;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating evaluation: {ex.Message}");
            }
        }

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
    }
}

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

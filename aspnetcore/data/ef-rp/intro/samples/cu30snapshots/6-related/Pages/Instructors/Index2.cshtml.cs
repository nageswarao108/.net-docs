#region snippet_all
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;  // Add VM
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors
{
#region snippet_1
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData Instructor { get; set; }
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

#region snippet_OnGetAsync
        public async Task OnGetAsync(int? id, int? courseID)
        {
            InstructorData = new InstructorIndexData();
#region snippet_ThenInclude
            InstructorData.Instructors = await _context.Instructors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)
                        .ThenInclude(i => i.Department)
                  .AsNoTracking()
                  .OrderBy(i => i.LastName)
                  .ToListAsync();
#endregion

#region snippet_ID
            if (id != null)
            {
                InstructorID = id.Value;
                Instructor instructor = InstructorData.Instructors.Where(
                    i => i.ID == id.Value).Single();
                InstructorData.Courses = instructor.CourseAssignments.Select(s => s.Course);
            }
#endregion

#region snippet_courseID
            if (courseID != null)
            {
                CourseID = courseID.Value;
                InstructorData.Enrollments = InstructorData.Courses.Where(
                    x => x.CourseID == courseID).Single().Enrollments;
            }
#endregion
        }
#endregion
#endregion
    }
}
#endregion
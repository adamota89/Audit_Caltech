using Caltec.StudentInfoProject.Business;
using Caltec.StudentInfoProject.Business.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Caltec.StudentInfoProject.WebUi.Pages.Students
{
    public class StudentListModel : StudentModelBase
    {
        public StudentListModel(StudentService service) : base(service)
        {
            
        }

        public IList<StudentDto> Students { get;set; } = default!;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public int TotalStudents { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1)
        {
            int pageSize = 10;
            TotalStudents = await _service.GetStudentsnumber();
            TotalPages = (int)Math.Ceiling(TotalStudents / (double) pageSize);
            

            Students = await _service.GetStudentsAsync(CancellationToken.None, pageNumber, pageSize);
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasNextPage = PageNumber < TotalPages;

        }
    }
}

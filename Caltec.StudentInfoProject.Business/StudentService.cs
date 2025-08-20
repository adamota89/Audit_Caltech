using Caltec.StudentInfoProject.Business.Dto;
using Caltec.StudentInfoProject.Domain;
using Caltec.StudentInfoProject.Persistence;
using Caltec.Dependency.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Caltec.StudentInfoProject.Business
{
    public class StudentService : BaseService
    {
        public StudentService(StudentInfoDbContext studentInfoDbContext) : base(studentInfoDbContext)
        {

        }

        /**
         * Optimisation de cette fonction pour éviter de faire deux fois un foreach qui prend trop de temps
         */ 
        public async Task<List<StudentDto>> GetStudentsAsync(CancellationToken cancellationToken, int pageNumber = 1, int pageSize = 10)
        {
            var students = await StudentInfoDbContext.Students
                .Include(s => s.Class)
                .Include(s => s.Fees)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Address = s.Address,
                    City = s.City,
                    ClassId = s.Class != null ? s.Class.Id : 0,
                    ClassName = s.Class != null ? s.Class.Name : string.Empty,
                    Country = s.Country,
                    Email = s.Email,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Phone = s.Phone,
                    State = s.State,
                    Zip = s.Zip,
                    SumOfFees = s.Fees.Sum(f => f.Amount) // calculé directement
                })
                .ToListAsync(cancellationToken);

            return students;
        }

        public async Task<StudentDto> UpdateAsync(StudentDto StudentToUpdate, CancellationToken cancellationToken)
        {
            var student = await StudentInfoDbContext.Students.FindAsync(StudentToUpdate.Id);
            if (student == null)
            {
                throw new NotFoundException("Student not found");
            }
            student.FirstName = StudentToUpdate.FirstName;
            student.LastName = StudentToUpdate.LastName;
            student.Email = StudentToUpdate.Email;
            student.Phone = StudentToUpdate.Phone;
            student.Address = StudentToUpdate.Address;
            student.City = StudentToUpdate.City;
            student.State = StudentToUpdate.State;
            student.Zip = StudentToUpdate.Zip;
            student.Country = StudentToUpdate.Country;
            student.Class = await StudentInfoDbContext.StudentClasses.FindAsync(StudentToUpdate.ClassId);
            await StudentInfoDbContext.SaveChangesAsync(cancellationToken);
            return StudentToUpdate;
        }

        public async  Task<StudentDto> InsertSpecialStudent(StudentDto StudentToInsert, CancellationToken cancellationToken)
        {
            var query = $"INSERT INTO Students (FirstName, LastName) VALUES ('{StudentToInsert.FirstName}', '{StudentToInsert.LastName}')";

            StudentInfoDbContext.Database.ExecuteSqlRaw(query);

            return new StudentDto
            {
                FirstName = StudentToInsert.FirstName,
                LastName = StudentToInsert.LastName
            };
        }

        public async Task<StudentDto> GetOne(long Id, CancellationToken cancellationToken)
        {
            var student = await StudentInfoDbContext.Students
                .Include(s => s.Fees)
                .Include(x => x.Class)
                .FirstOrDefaultAsync(x => x.Id == Id, cancellationToken);

            if (student == null)
            {
                throw new NotFoundException("Student not found");
            }
            return new StudentDto
            {
                Id = student.Id,
                Address = student.Address,
                City = student.City,
                ClassName = student?.Class?.Name,
                ClassId = student?.Class?.Id,
                Country = student.FirstName,
                Email = student.Email,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Phone = student.Phone,
                State = student.State,
                Zip = student.Zip
            };
        }

        public async Task<StudentDto> InsertStudent(StudentDto studentDto, CancellationToken cancellationToken)
        {
            var student = new Student
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                Address = studentDto.Address,
                City = studentDto.City,
                State = studentDto.State,
                Zip = studentDto.Zip,
                Country = studentDto.Country,
                Class = await StudentInfoDbContext.StudentClasses.FindAsync(studentDto.ClassId)
            };
            await StudentInfoDbContext.Students.AddAsync(student, cancellationToken);
            await StudentInfoDbContext.SaveChangesAsync(cancellationToken);
            return new StudentDto
            {
                Address = student.Address,
                City = student.City,
                ClassName = student?.Class?.Name,
                ClassId = student?.Class?.Id,
                Country = student.Country,
                Email = student.Email,
                FirstName = student.FirstName,
                Id = student.Id,
                LastName = student.LastName,
                Phone = student.Phone,
                State = student.State,
                
            };
        }

        public async Task DeleteStudentAsync(long Id, CancellationToken cancellationToken)
        {
            var student = await StudentInfoDbContext.Students
                .Include(s => s.Fees)
                .FirstOrDefaultAsync(s => s.Id == Id, cancellationToken);
            if (student == null)
            {
                throw new NotFoundException("Student not found");
            }
            // Supprime uniquement l'étudiant
            StudentInfoDbContext.Students.Remove(student);
            await StudentInfoDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> GetStudentsnumber()
        {
            return await StudentInfoDbContext.Students.CountAsync();
        }
    }
}

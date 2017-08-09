using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Data;
using SchoolProject.Models;
using System.IO;
using SchoolProject.Util;
using System.Collections.Generic;

namespace SchoolProject.Controllers
{
    public class TeachersController : Controller
    {
        private readonly SchoolContext _context;

        public TeachersController(SchoolContext context)
        {
            _context = context;    
        }

        // GET: Teachers
        public IActionResult Index()
        {
            var model = new StudentTeacherModel();
            model.Teachers = _context.Teachers.ToList();
            model.Students = _context.Students.ToList();
            return View(model);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.SingleOrDefaultAsync(m => m.ID == id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName")] Teacher teacher)
        {
            if (id != teacher.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers
                .SingleOrDefaultAsync(m => m.ID == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.SingleOrDefaultAsync(m => m.ID == id);
            await _context.Students
                .Where(s => s.teacher == teacher)
                .ForEachAsync(
            student =>
            {
                student.teacher = null;
                _context.Update(student);
            });

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.ID == id);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCSV()
        {
            var csvFile = Request.Form.Files[0];
            var csvContent = string.Empty;

            using (var reader = new StreamReader(csvFile.OpenReadStream()))
            {
                csvContent = reader.ReadToEnd();
            }

            CSVUtil.parseCSVForTeachers(csvContent).ForEach(e =>
            {
                _context.Add(e);
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent()
        {
            var formData = Request.Form;
            int teacherID = -1;

            if (!string.IsNullOrEmpty(formData["teacherID"]))
            {
                teacherID = int.Parse(formData["teacherID"]);
            }
            else
            {
                return RedirectToAction("Index");
            }

            List<string> studentIds = formData["student[]"].ToList();

            System.Diagnostics.Debug.WriteLine(teacherID + "  " + formData.Count);
            System.Diagnostics.Debug.WriteLine(formData["student[]"]);

            Teacher teacher = _context.Teachers.Where(t => t.ID == teacherID).First();

            foreach(string studentID in studentIds)
            {
                Student student = _context.Students.Where(s => s.ID == int.Parse(studentID)).First();

                if(student != null && student.teacher != teacher)
                {
                    System.Diagnostics.Debug.WriteLine("Adding Student " + student);
                    student.teacher = teacher;
                    _context.Update(student);
                }
            } 

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}

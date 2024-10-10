using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QUANLY_KHACHSAN.InterfacesRepositories;
using QUANLY_KHACHSAN.Models;
using QUANLY_KHACHSAN.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QUANLY_KHACHSAN.Controllers
{
    public class MonanController : Controller
    {
        private readonly IMonanRepository _monanRepo;
        private readonly INhanvienRepository _employeeRepo;
        public MonanController(IMonanRepository monanRepo, INhanvienRepository employeeRepo)
        {
            _monanRepo = monanRepo;
            _employeeRepo = employeeRepo;
        }

        // Hiển thị danh sách món ăn
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var monans = await _monanRepo.GetAllAsync();
            return View(monans);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var employees = await _employeeRepo.GetAllAsync();
            ViewData["Manv"] = new SelectList(employees, "Manv", "Hoten");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Monan monan)
        {
            try
            {
                var nhanVienExists = await _employeeRepo.NhanvienExists(monan.Manv);
                if (!nhanVienExists)
                {
                    ModelState.AddModelError("Manv", "Nhân viên không tồn tại.");
                }
                await _monanRepo.AddAsync(monan);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra: " + ex.Message);
                return View(monan);
            }
            //if (ModelState.IsValid)
            //{
            //    // Kiểm tra xem MANV có tồn tại không
            //    var nhanVienExists = await _employeeRepo.NhanvienExists(monan.Manv);

            //    if (!nhanVienExists)
            //    {
            //        ModelState.AddModelError("Manv", "Nhân viên không tồn tại.");
            //        return View(monan);
            //    }

            //    await _monanRepo.AddAsync(monan);
            //    return RedirectToAction(nameof(Index));
            //}

            //var nhanVienList = await _employeeRepo.GetAllAsync();
            //ViewBag.NhanVienList = new SelectList(nhanVienList, "Manv", "Hoten");
            //return View(monan);
        }

        // Hiển thị form sửa món ăn
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var monan = await _monanRepo.GetByIdAsync(id);
            if (monan == null)
            {
                return NotFound();
            }
            return View(monan);
        }

        // Xử lý form sửa món ăn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Monan monan)
        {
            if (ModelState.IsValid)
            {
                await _monanRepo.UpdateAsync(monan);
                return RedirectToAction("Index");
            }
            return View(monan);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !(await _monanRepo.MonanExists(id.Value)))
            {
                return NotFound();
            }

            var rent = await _monanRepo.GetByIdAsync(id.Value);
            return View(rent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin về phiếu thuê cần xóa
            var vehicle = await _monanRepo.GetByIdAsync(id);

            // Kiểm tra nếu xe không tồn tại
            if (vehicle == null)
            {
                return NotFound();
            }


            await _monanRepo.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
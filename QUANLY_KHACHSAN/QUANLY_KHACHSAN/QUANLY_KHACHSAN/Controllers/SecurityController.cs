using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using QUANLY_KHACHSAN.InterfacesRepositories;
using QUANLY_KHACHSAN.Models;
using QUANLY_KHACHSAN.Repositories;
using QUANLY_KHACHSAN.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QUANLY_KHACHSAN.Controllers
{
    public class SecurityController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly INhanvienRepository _employeeRepo;

        public SecurityController(IVehicleRepository vehicleRepository, INhanvienRepository employeeRepo)
        {
            _vehicleRepository = vehicleRepository;
            _employeeRepo = employeeRepo;
        }

        public async Task<IActionResult> VehicleList(string searchString, string SortOrder, string sortColumn, int pageNumber, string currentFilter, int manager)
        {
            ViewData["sortColumn"] = sortColumn;
            ViewData["sortOrder"] = SortOrder;
            ViewData["MaSortParam"] = sortColumn == "Mabv" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
            ViewData["LicensePlateSortParam"] = sortColumn == "LicensePlate" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
            ViewData["CheckInDateSortParam"] = sortColumn == "CheckInDate" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
            ViewData["CheckOutDateSortParam"] = sortColumn == "CheckOutDate" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            TempData["Manager"] = manager;

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPageNumber"] = pageNumber;
            var baoves = _vehicleRepository.GetAllAsync();
            if (!string.IsNullOrEmpty(searchString))
            {
                baoves = baoves.Where(r => r.LicensePlate != null && r.LicensePlate.ToLower().Contains(searchString.ToLower()));
            }
            
            switch (sortColumn)
            {
                case "Mabv":
                    baoves = SortOrder == "desc" ? baoves.OrderByDescending(r => r.Mabv) : baoves.OrderBy(r => r.Mabv);
                    break;
                case "LicensePlate":
                    baoves = SortOrder == "desc" ? baoves.OrderByDescending(r => r.LicensePlate) : baoves.OrderBy(r => r.LicensePlate);
                    break;
                case "CheckInDate":
                    baoves = SortOrder == "desc" ? baoves.OrderByDescending(r => r.CheckInDate) : baoves.OrderBy(r => r.CheckInDate);
                    break;
                case "Dongia":
                    baoves = SortOrder == "desc" ? baoves.OrderByDescending(r => r.CheckOutDate) : baoves.OrderBy(r => r.CheckOutDate);
                    break;
                default:
                    baoves = baoves.OrderBy(r => r.Mabv);
                    break;
            }

            if (pageNumber < 1)
            {
                pageNumber = 1;
            }
            int pageSize = 7;

            ViewBag.Manager = manager;

            return View(await PaginatedList<Baove>.CreateAsync(baoves, pageNumber, pageSize));


        }
        [HttpGet]
        public async Task<IActionResult> Create(int manager)
        {
            TempData["Manager"] = manager;

            // Lấy danh sách nhân viên để hiển thị trong dropdown
            var employees = await _employeeRepo.GetAllEmAsync();
            ViewBag.Manv = new SelectList(employees, "Manv", "Hoten");

            return View(new Baove());
        }
        // GET: Vehicle/Create
        public async Task<IActionResult> Create()
        {
            var employees = await _vehicleRepository.GetAllEmployeesAsync();
            ViewBag.Employees = employees; // Lưu danh sách nhân viên vào ViewBag
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Baove baove)
        {
            if (ModelState.IsValid)
            {
                await _vehicleRepository.AddAsync(baove);
                return RedirectToAction(nameof(Index));
            }
            return View(baove);
        }

        // GET: Vehicle/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            var employees = await _vehicleRepository.GetAllEmployeesAsync();
            ViewBag.Employees = employees; // Lưu danh sách nhân viên vào ViewBag
            return View(vehicle);
        }

        // POST: Vehicle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Baove baove)
        {
            if (id != baove.Mabv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _vehicleRepository.UpdateAsync(baove);
                return RedirectToAction(nameof(Index));
            }
            return View(baove);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !(await _vehicleRepository.VehicleExists(id.Value)))
            {
                return NotFound();
            }

            var rent = await _vehicleRepository.GetByIdAsync(id.Value);
            return View(rent);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Lấy thông tin về phiếu thuê cần xóa
            var vehicle = await _vehicleRepository.GetByIdAsync(id);

            // Kiểm tra nếu xe không tồn tại
            if (vehicle == null)
            {
                return NotFound();
            }

           
            await _vehicleRepository.DeleteAsync(id);
            return RedirectToAction(nameof(VehicleList));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QUANLY_KHACHSAN.Models;
using QUANLY_KHACHSAN.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLY_KHACHSAN.ViewModels;

public class TapvuController : Controller
{
    private readonly ITapvuRepository _tapvuRepo;
    private readonly IPhongRepository _phongRepo;

    public TapvuController(ITapvuRepository tapvuRepo, IPhongRepository phongRepo)
    {
        _tapvuRepo = tapvuRepo;
        _phongRepo = phongRepo;
    }

    public async Task<IActionResult> Index(string searchString, string SortOrder, string sortColumn, int pageNumber = 1, string currentFilter = null)
    {
        ViewData["sortColumn"] = sortColumn;
        ViewData["sortOrder"] = SortOrder;
        ViewData["MaSortParam"] = sortColumn == "Matapvu" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
        ViewData["DaDonDepSortParam"] = sortColumn == "DaDonDep" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
        ViewData["DaThemDoDungSortParam"] = sortColumn == "DaThemDoDung" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
        ViewData["SoLuongKhanSortParam"] = sortColumn == "SoLuongKhan" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
        ViewData["SoLuongGaGiuongSortParam"] = sortColumn == "SoLuongGaGiuong" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";
        ViewData["SoLuongDungCuVeSinhSortParam"] = sortColumn == "SoLuongDungCuVeSinh" ? (SortOrder == "asc" ? "desc" : "asc") : "asc";

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;
        ViewData["CurrentPageNumber"] = pageNumber;

        var tapvus = _tapvuRepo.GetAllAsync();

        if (!string.IsNullOrEmpty(searchString))
        {
            tapvus = tapvus.Where(t => t.MapNavigation.Tenphong != null && t.MapNavigation.Tenphong.ToLower().Contains(searchString.ToLower()));
        }

        switch (sortColumn)
        {
            case "Matapvu":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.Matapvu) : tapvus.OrderBy(t => t.Matapvu);
                break;
            case "DaDonDep":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.DaDonDep) : tapvus.OrderBy(t => t.DaDonDep);
                break;
            case "DaThemDoDung":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.DaThemDoDung) : tapvus.OrderBy(t => t.DaThemDoDung);
                break;
            case "SoLuongKhan":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.SoLuongKhan) : tapvus.OrderBy(t => t.SoLuongKhan);
                break;
            case "SoLuongGaGiuong":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.SoLuongGaGiuong) : tapvus.OrderBy(t => t.SoLuongGaGiuong);
                break;
            case "SoLuongDungCuVeSinh":
                tapvus = SortOrder == "desc" ? tapvus.OrderByDescending(t => t.SoLuongDungCuVeSinh) : tapvus.OrderBy(t => t.SoLuongDungCuVeSinh);
                break;
            default:
                tapvus = tapvus.OrderBy(t => t.Matapvu);
                break;
        }

        int pageSize = 7;
        return View(await PaginatedList<Tapvu>.CreateAsync(tapvus, pageNumber, pageSize));
    }

    public async Task<IActionResult> Update(int id, int manager)
    {
        TempData["Manager"] = manager;
        var tapvu = await _tapvuRepo.GetByIdAsync(id);
        if (tapvu == null)
        {
            return NotFound();
        }

        var rooms = await _phongRepo.GetAllAsync().ToListAsync();
        ViewBag.Rooms = new SelectList(rooms, "Map", "Tenphong", tapvu.Map);
        return View(tapvu);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Tapvu tapvu, int manager)
    {
        if (!ModelState.IsValid)
        {
            var rooms = await _phongRepo.GetAllAsync().ToListAsync();
            ViewBag.Rooms = new SelectList(rooms, "Map", "Tenphong", tapvu.Map);
            return View(tapvu);
        }

        await _tapvuRepo.UpdateAsync(tapvu);
        return RedirectToAction("Index", new { manager });
    }
}


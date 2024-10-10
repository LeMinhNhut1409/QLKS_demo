using Microsoft.EntityFrameworkCore;
using QUANLY_KHACHSAN.InterfacesRepositories;
using QUANLY_KHACHSAN.Models;
using QUANLY_KHACHSAN.Repositories;

public class TapvuRepository : ITapvuRepository
{
    private readonly QUANLY_KHACHSANContext _context;
    private readonly IPhongRepository _phongRepo;

    private readonly INhanvienRepository _nhanvienRepo;
    public TapvuRepository(QUANLY_KHACHSANContext context, IPhongRepository phongRepo, INhanvienRepository nhanvienRepo)
    {
        _context = context;
        _phongRepo = phongRepo;
        _nhanvienRepo = nhanvienRepo;
    }

    public IQueryable<Tapvu> GetAllAsync()
    {
        return _context.Tapvus
            .Include(t => t.MapNavigation)
            .Select(tapvu => new Tapvu
            {
                Matapvu = tapvu.Matapvu,
                MapNavigation = tapvu.MapNavigation,
                DaDonDep = tapvu.DaDonDep,
                DaThemDoDung = tapvu.DaThemDoDung,
                SoLuongKhan = tapvu.SoLuongKhan,
                SoLuongGaGiuong = tapvu.SoLuongGaGiuong,
                SoLuongDungCuVeSinh = tapvu.SoLuongDungCuVeSinh,
            });
    }

    public async Task<Tapvu> GetByIdAsync(int id)
    {
        return await _context.Tapvus
            .Include(t => t.MapNavigation)
            .Include(t => t.ManvNavigation)
            .FirstOrDefaultAsync(t => t.Matapvu == id);
    }

    public async Task UpdateAsync(Tapvu tapvuUpdate)
    {
       
        _context.Tapvus.Update(tapvuUpdate);
        
        await _context.SaveChangesAsync();
    }

    public async Task<List<Tapvu>> GetTapvuByRoomIdAsync(int roomId)
    {
        return await _context.Tapvus
            .Where(t => t.Map == roomId)
            .ToListAsync();
    }

    public async Task<List<Tapvu>> GetTapvuByEmployeeIdAsync(int employeeId)
    {
        return await _context.Tapvus
            .Where(t => t.Manv == employeeId)
            .ToListAsync();
    }
}
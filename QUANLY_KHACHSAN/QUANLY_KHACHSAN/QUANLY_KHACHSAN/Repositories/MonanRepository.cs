using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QUANLY_KHACHSAN.InterfacesRepositories;
using QUANLY_KHACHSAN.Models;

namespace QUANLY_KHACHSAN.Repositories
{
    public class MonanRepository : IMonanRepository
    {
        private readonly QUANLY_KHACHSANContext _context;

        public MonanRepository(QUANLY_KHACHSANContext context)
        {
            _context = context;
        }

        public async Task<List<Monan>> GetAllAsync()
        {
            return await _context.Monans
                .Include(m => m.ManvNavigation) // Bao gồm thông tin nhân viên nếu cần
                .ToListAsync();
        }

        public async Task<Monan> GetByIdAsync(int id)
        {
            return await _context.Monans
                .Include(m => m.ManvNavigation) // Bao gồm thông tin nhân viên nếu cần
                .FirstOrDefaultAsync(m => m.Mamonan == id);
        }

        public async Task AddAsync(Monan monan)
        {
            await _context.Monans.AddAsync(monan); 
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(Monan monan)
        {
            _context.Update(monan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var monan = await GetByIdAsync(id);
            if (monan != null)
            {
                _context.Monans.Remove(monan);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> MonanExists(int id)
        {
            return await _context.Monans.AnyAsync(b => b.Mamonan == id);
        }
        public async Task<List<int>> GetDistinctManvAsync()
        {
            return await _context.Phieuthues
                .Select(r => r.Manv)
                .Distinct()
                .ToListAsync();
        }
        public async Task<List<Nhanvien>> GetAllEmployeesAsync()
        {
            return await _context.Nhanviens.ToListAsync();
        }

        public async Task<List<Phieuthue>> GetMonanByEmployeeIdAsync(int manv)
        {
            return await _context.Phieuthues
                .Where(pt => pt.Manv == manv)
                .ToListAsync();
        }
    }
}
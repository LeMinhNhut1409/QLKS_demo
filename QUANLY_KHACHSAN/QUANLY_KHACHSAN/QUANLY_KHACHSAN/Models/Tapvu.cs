using QUANLY_KHACHSAN.Models;

public class Tapvu
{
    public int Matapvu { get; set; } // Khóa chính
    public bool DaDonDep { get; set; } // Trạng thái dọn dẹp
    public bool DaThemDoDung { get; set; } // Trạng thái thêm đồ dùng
    public int? SoLuongKhan { get; set; } // Số lượng khăn trong phòng
    public int? SoLuongGaGiuong { get; set; } // Số lượng ga giường trong phòng
    public int? SoLuongDungCuVeSinh { get; set; } // Số lượng dụng cụ vệ sinh trong phòng
    public int Map { get; set; } // Khóa ngoại
    public int Manv { get; set; } // Khóa ngoại

    public virtual Phong MapNavigation { get; set; } = null!;
    public virtual Nhanvien ManvNavigation { get; set; } = null!;
}
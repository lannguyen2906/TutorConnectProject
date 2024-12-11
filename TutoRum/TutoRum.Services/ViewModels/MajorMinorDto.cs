using System;
using System.Collections.Generic;
using TutoRum.Data.Models;

namespace TutoRum.Services.ViewModels
{
    public class MajorMinorDto
    {
        public string Major { get; set; }
        public List<string> Minors { get; set; }
    }
    public class TutorMajorDto
    {
        // public Guid TutorId { get; set; }
       // public string? Specialization { get; set; }
        public string? Major { get; set; }
        public string? ShortDescription { get; set; }
        public List<string> Minors { get; set; } = new List<string>();
    }
    public static class MajorMinorData
    {
        public static List<MajorMinorDto> FieldsList = new List<MajorMinorDto>
    {
        new MajorMinorDto
        {
            Major = "Công nghệ Thông tin",
            Minors = new List<string> { "Khoa học Máy tính", "An ninh mạng", "Phát triển Phần mềm", "Trí tuệ Nhân tạo", "Khoa học Dữ liệu" }
        },
        new MajorMinorDto
        {
            Major = "Kinh tế",
            Minors = new List<string> { "Quản trị Kinh doanh", "Kinh tế Quốc tế", "Kế toán", "Kinh tế Phát triển", "Tài chính" }
        },
        new MajorMinorDto
        {
            Major = "Kỹ thuật Điện",
            Minors = new List<string> { "Điện Tử Công nghiệp", "Hệ thống Năng lượng", "Kỹ thuật Điều khiển", "Điện Tử Viễn Thông", "Kỹ thuật Tự động hóa" }
        },
        new MajorMinorDto
        {
            Major = "Luật",
            Minors = new List<string> { "Luật Kinh tế", "Luật Quốc tế", "Luật Dân sự", "Luật Hình sự", "Luật Thương mại" }
        },
        new MajorMinorDto
        {
            Major = "Quản trị Du lịch và Lữ hành",
            Minors = new List<string> { "Quản trị Nhà hàng - Khách sạn", "Hướng dẫn Du lịch", "Quản trị Lữ hành", "Tổ chức Sự kiện", "Quản lý Khu Nghỉ dưỡng" }
        },
        new MajorMinorDto
        {
            Major = "Kỹ thuật Cơ khí",
            Minors = new List<string> { "Cơ khí Chế tạo Máy", "Cơ Điện Tử", "Kỹ thuật Ô tô", "Robot Công nghiệp", "Kỹ thuật Nhiệt Lạnh" }
        },
        new MajorMinorDto
        {
            Major = "Khoa học Môi trường",
            Minors = new List<string> { "Quản lý Tài nguyên Môi trường", "Kỹ thuật Môi trường", "Sinh thái học", "Công nghệ Sinh học Môi trường", "Khoa học Biển" }
        },
        new MajorMinorDto
        {
            Major = "Tài chính - Ngân hàng",
            Minors = new List<string> { "Tài chính Doanh nghiệp", "Ngân hàng", "Đầu tư Tài chính", "Chứng khoán", "Quản trị Rủi ro" }
        },
        new MajorMinorDto
        {
            Major = "Marketing",
            Minors = new List<string> { "Marketing Kỹ thuật số", "Truyền thông", "Quan hệ Công chúng", "Quản trị Thương hiệu", "Quảng cáo" }
        },
        new MajorMinorDto
        {
            Major = "Khoa học Xã hội",
            Minors = new List<string> { "Tâm lý học", "Xã hội học", "Khoa học Chính trị", "Nhân học", "Giới và Phát triển" }
        },
        new MajorMinorDto
        {
            Major = "Nông nghiệp",
            Minors = new List<string> { "Công nghệ Thực phẩm", "Khoa học Cây trồng", "Chăn nuôi", "Khoa học Đất", "Thủy sản" }
        },
        new MajorMinorDto
        {
            Major = "Giáo dục",
            Minors = new List<string> { "Sư phạm Mầm non", "Sư phạm Tiểu học", "Sư phạm Toán", "Sư phạm Văn", "Giáo dục Thể chất" }
        },
        new MajorMinorDto
        {
            Major = "Y học",
            Minors = new List<string> { "Y khoa", "Dược", "Điều dưỡng", "Y tế Công cộng", "Kỹ thuật Hình ảnh Y học" }
        },
        new MajorMinorDto
        {
            Major = "Thiết kế Đồ họa",
            Minors = new List<string> { "Thiết kế Web", "Thiết kế Game", "Thiết kế Sản phẩm", "Thiết kế Thời trang", "Nghệ thuật Kỹ thuật số" }
        },
        new MajorMinorDto
        {
            Major = "Kiến trúc",
            Minors = new List<string> { "Kiến trúc Cảnh quan", "Quy hoạch Đô thị", "Thiết kế Nội thất", "Xây dựng Dân dụng", "Kiến trúc Công nghiệp" }
        },
        new MajorMinorDto
        {
            Major = "Truyền thông Đa phương tiện",
            Minors = new List<string> { "Báo chí", "Phát thanh - Truyền hình", "Sản xuất Phim", "Quan hệ Công chúng", "Truyền thông Xã hội" }
        },
        new MajorMinorDto
        {
            Major = "Hóa học",
            Minors = new List<string> { "Hóa Dược", "Hóa Vô cơ", "Hóa Hữu cơ", "Hóa Lý", "Công nghệ Nano" }
        }
    };
    }

}

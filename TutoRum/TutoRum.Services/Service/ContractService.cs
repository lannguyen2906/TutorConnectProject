using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoRum.Services.IService;
using Xceed.Words.NET;

namespace TutoRum.Services.Service
{

    public class ContractData
    {
        public string? ContractNumber { get; set; }
        public DateTime? ContractDate { get; set; }
        public string? Location { get; set; }
        public string? LocationDetail { get; set; }
        public string? CustomerName { get; set; } // Tên học sinh (Learner)
        public DateTime? CustomerBirthYear { get; set; } // Năm sinh của học sinh (Learner)
        public string? CustomerAddress { get; set; }
        public string? CustomerPhone { get; set; }

        public string? TutorName { get; set; }
        public DateTime? TutorBirthYear { get; set; }
        public string? TutorAddress { get; set; }
        public string? TutorPhone { get; set; }
        public string? TutorSpecialization { get; set; }

        public string? SubjectName { get; set; }
        public string? DaysOfWeek { get; set; }
        public DateTime? StartDate { get; set; }
        public decimal? HourlyRate { get; set; }
        public DateTime? ContractStartDate { get; set; }
        public int? SessionsPerWeek { get; set; }
        public int? HoursPerSession { get; set; }
    }


    public class ContractService : IContractService
    {
        private readonly string _connectionString;

        public ContractService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public async Task<string> GenerateContractAsync(int tutorLearnerSubjectID)
        {
            // 1. Lấy dữ liệu hợp đồng từ cơ sở dữ liệu
            var contractData = await GetContractDataAsync(tutorLearnerSubjectID);




            // 2. Đường dẫn file mẫu và file đầu ra
            string templatePath = @"C:\home\site\wwwroot\Template\contract_template.docx";

            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output", $"contract_{tutorLearnerSubjectID}.docx");

            // Kiểm tra nếu thư mục "Output" chưa tồn tại thì tạo mới
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output"));
            }

            // 3. Tạo hợp đồng từ mẫu
            using (var document = DocX.Load(templatePath))
            {
                // Thay thế các placeholder với dữ liệu thực (kiểm tra null trước khi thay thế)
                document.ReplaceText("{ContractNumber}", contractData.ContractNumber ?? string.Empty);
                document.ReplaceText("{ContractDate}", contractData.ContractDate?.ToString("dd/MM/yyyy") ?? string.Empty);
                document.ReplaceText("{Location}", contractData.Location ?? string.Empty);
                document.ReplaceText("{LocationDetail}", contractData.LocationDetail ?? string.Empty);

                // Thông tin học sinh (Learner)
                document.ReplaceText("{CustomerName}", contractData.CustomerName ?? string.Empty);
                document.ReplaceText("{CustomerBirthYear}", contractData.CustomerBirthYear?.ToString("dd/MM/yyyy") ?? string.Empty);
                document.ReplaceText("{CustomerAddress}", contractData.CustomerAddress ?? string.Empty);
                document.ReplaceText("{CustomerPhone}", contractData.CustomerPhone ?? string.Empty);

                // Thông tin gia sư (Tutor)
                document.ReplaceText("{TutorName}", contractData.TutorName ?? string.Empty);
                document.ReplaceText("{TutorBirthYear}", contractData.TutorBirthYear?.ToString("dd/MM/yyyy") ?? string.Empty);
                document.ReplaceText("{TutorAddress}", contractData.TutorAddress ?? string.Empty);
                document.ReplaceText("{TutorPhone}", contractData.TutorPhone ?? string.Empty);

                // Thông tin hợp đồng và môn học
                document.ReplaceText("{SubjectName}", contractData.SubjectName ?? string.Empty);
                document.ReplaceText("{DaysOfWeek}", contractData.DaysOfWeek ?? string.Empty);
                document.ReplaceText("{StartDate}", contractData.StartDate?.ToString("dd/MM/yyyy") ?? string.Empty);
                document.ReplaceText("{HourlyRate}", contractData.HourlyRate?.ToString("N2") ?? string.Empty);
                document.ReplaceText("{ContractStartDate}", contractData.ContractStartDate?.ToString("dd/MM/yyyy") ?? string.Empty);
                document.ReplaceText("{SessionsPerWeek}", contractData.SessionsPerWeek?.ToString() ?? string.Empty);
                document.ReplaceText("{HoursPerSession}", contractData.HoursPerSession?.ToString() ?? string.Empty);


                // Lưu hợp đồng đã điền dữ liệu
                document.SaveAs(outputPath);
            }

            return outputPath; // Trả về đường dẫn của file đã tạo
        }

        public async Task<ContractData> GetContractDataAsync(int tutorLearnerSubjectID)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("GetContractData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TutorLearnerSubjectID", tutorLearnerSubjectID);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new ContractData
                            {
                                // Thông tin hợp đồng
                                ContractNumber = reader["ContractNumber"]?.ToString(),
                                ContractDate = reader["ContractDate"] != DBNull.Value ? Convert.ToDateTime(reader["ContractDate"]) : DateTime.MinValue,
                                Location = reader["Location"]?.ToString(),
                                LocationDetail = reader["LocationDetail"]?.ToString(),

                                // Thông tin gia sư (Tutor)
                                TutorName = reader["TutorName"]?.ToString(),
                                TutorBirthYear = reader["TutorDOB"] != DBNull.Value ? ((DateTime)reader["TutorDOB"]) : DateTime.MinValue,
                                TutorAddress = reader["TutorAddress"]?.ToString(),
                                TutorPhone = reader["TutorPhone"]?.ToString(),

                                // Thông tin học sinh (Learner)
                                CustomerName = reader["LearnerName"]?.ToString(),
                                CustomerBirthYear = reader["LearnerDOB"] != DBNull.Value ? Convert.ToDateTime(reader["LearnerDOB"]) : DateTime.MinValue,
                                CustomerAddress = reader["LearnerAddress"]?.ToString(),
                                CustomerPhone = reader["LearnerPhone"]?.ToString(),

                                // Thông tin môn học và thời gian
                                SubjectName = reader["SubjectName"]?.ToString(),
                                DaysOfWeek = reader["DaysOfWeek"]?.ToString(),
                                StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToDateTime(reader["StartDate"]) : DateTime.MinValue,
                                HourlyRate = reader["HourlyRate"] != DBNull.Value ? Convert.ToDecimal(reader["HourlyRate"]) : 0,
                                ContractStartDate = reader["ContractStartDate"] != DBNull.Value ? Convert.ToDateTime(reader["ContractStartDate"]) : DateTime.MinValue,

                                // Thông tin bổ sung
                                SessionsPerWeek = reader["SessionsPerWeek"] != DBNull.Value ? Convert.ToInt32(reader["SessionsPerWeek"]) : 0,
                                HoursPerSession = reader["HoursPerSession"] != DBNull.Value ? Convert.ToInt32(reader["HoursPerSession"]) : 0
                            };
                        }
                    }
                }
            }

            throw new Exception("Contract data not found.");
        }
    }

       
    }

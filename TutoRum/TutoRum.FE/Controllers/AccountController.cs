using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TutoRum.Services.IService;
using TutoRum.Services.ViewModels;
using TutoRum.Services.Service;
using TutoRum.FE.Common;
using System.Web;
using TutoRum.Data.Enum;
using TutoRum.Data.Models;

namespace TutoRum.FE.Controllers
{
    public class AccountsController : ApiControllerBase
    {
        private readonly IAccountService accountRepo;
        private readonly IEmailService _emailService;
        private readonly UserManager<AspNetUser> _userManager;
        public AccountsController(IAccountService repo, IEmailService emailService, UserManager<AspNetUser> userManager)
        {
            accountRepo = repo;
            _emailService = emailService;
            _userManager = userManager;
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(ApiResponse<SignInResponseDto>), 200)]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await accountRepo.GetCurrentUser(User);
            
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(ApiResponseFactory.Success(user));
        }

        [HttpPost]
        [Route(Common.Url.User.Identity.SignUp)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            try
            {
                // Thực hiện đăng ký
                var result = await accountRepo.SignUpAsync(signUpModel);
                if (result.Succeeded)
                {
                    var user = await accountRepo.GetUserByEmailAsync(signUpModel.Email);

                    if (user != null)
                    {
                        var token = await accountRepo.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Accounts",
                                        new { userId = user.Id, token = token }, Request.Scheme);
                    var emailContent = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    </head>
  
                    <body style='font-family: Arial, sans-serif; background-color: #dedcdc; margin: 0; padding: 20px;'>
                        <div style='background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); max-width: 600px; margin: auto;'>
        
                            <p style='line-height: 1.6; color: #555;'>Chúng tôi rất vui mừng chào đón bạn đến với <b>Tutor Connect</b>. Để hoàn tất quá trình đăng ký, vui lòng xác nhận tài khoản của bạn bằng cách nhấn vào nút dưới đây:</p>
                            <div style=""text-align: center;"">
                        <a href='{confirmationLink}' 
                            style='display: inline-block; 
                                    padding: 12px 25px; 
                                    margin: 20px 0; 
                                    background-color: #007bff; 
                                    color: #fff; 
                                    text-decoration: none; 
                                    border-radius: 5px; 
                                    font-weight: bold; 
                                    transition: background-color 0.3s ease, box-shadow 0.3s ease;
                                    text-align: center;'
                            onmouseover=""this.style.backgroundColor='#0056b3'; this.style.boxShadow='0px 4px 10px rgba(0, 0, 0, 0.1)';""
                            onmouseout=""this.style.backgroundColor='#007bff'; this.style.boxShadow='none';"">
                            Xác nhận tài khoản
                        </a>
                    </div>
                            <p style='line-height: 1.6; color: #555;'>Cảm ơn bạn đã lựa chọn chúng tôi!</p>
                            <div style='margin-top: 20px; font-size: 0.9em; color: #777;'>
                                <p>Trân trọng,</p>
                                <p>Đội ngũ hỗ trợ</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";

                    // Gửi email với confirmationLink
                    await _emailService.SendEmailAsync(user.Email, "[TutorConnect] Xác nhận tài khoản của bạn", emailContent);

                        var response = ApiResponseFactory.Success<object>(null, "Đăng ký thành công. Đã gửi email xác nhận." );
                        return Ok(response);
                    }

                    var userNotFoundResponse = ApiResponseFactory.NotFound<object>("Không tìm thấy người dùng sau khi đăng ký.");
                    return NotFound(userNotFoundResponse);
                }

                var unauthorizedResponse = ApiResponseFactory.Unauthorized<object>("Đăng ký không thành công.");
                return Unauthorized(unauthorizedResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpPost]
        [Route(Common.Url.User.Identity.SendEmailConfirmation)]
        public async Task<IActionResult> SendEmailConfirmation(string userId)
        {
            try
            {
                var user = await accountRepo.GetUserByIdAsync(userId);
                if (user == null)
                {
                    var response = ApiResponseFactory.NotFound<object>("User not found.");
                    return NotFound(response);
                }

                var token = await accountRepo.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Accounts",
                                    new { userId = user.Id, token = token }, Request.Scheme);

                // Gửi email với confirmationLink
                await _emailService.SendEmailAsync(user.Email, "Xác nhận tài khoản của bạn",
                    $"Vui lòng xác nhận tài khoản của bạn bằng cách nhấn vào đường link sau: {confirmationLink}");

                var successResponse = ApiResponseFactory.Success(new { message = "Confirmation email sent." });
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var response = ApiResponseFactory.ServerError<object>(detail: ex.Message);
                return StatusCode(500, response);
            }
        }


        [HttpGet]
        [Route(Common.Url.User.Identity.ConfirmEmail)]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await accountRepo.GetUserByIdAsync(userId);
            if (user == null) return BadRequest("User not found.");

            var result = await accountRepo.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return Redirect("https://tutor-rum-project.vercel.app/login");

            return BadRequest("Email confirmation failed.");
        }

        [HttpPost]
        [Route(Common.Url.User.Identity.ForgotPassword)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            // Kiểm tra xem email của người dùng có tồn tại trong hệ thống hay không
            var user = await accountRepo.GetUserByEmailAsync(model.Email);
            if (user == null) return BadRequest("Không tìm thấy người dùng với email này.");

            // Tạo token để reset mật khẩu
            var token = await accountRepo.GeneratePasswordResetTokenAsync(user);

            token = HttpUtility.UrlEncode(token);
            // Tạo link reset mật khẩu với token và email của người dùng
            var resetLink = "https://tutor-rum-project.vercel.app/reset-password?token=" + token + "&email=" + user.Email;

            var emailContent = $@"
                    <!DOCTYPE html>
                    <html lang='vi'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    </head>
  
                    <body style='font-family: Arial, sans-serif; background-color: #dedcdc; margin: 0; padding: 20px;'>
                        <div style='background-color: #ffffff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1); max-width: 600px; margin: auto;'>
        
                            <p style='line-height: 1.6; color: #555;'>Để đặt lại mật khẩu, vui lòng nhấn vào nút dưới đây:</p>
                            <div style=""text-align: center;"">
                        <a href='{resetLink}' 
                            style='display: inline-block; 
                                    padding: 12px 25px; 
                                    margin: 20px 0; 
                                    background-color: #007bff; 
                                    color: #fff; 
                                    text-decoration: none; 
                                    border-radius: 5px; 
                                    font-weight: bold; 
                                    transition: background-color 0.3s ease, box-shadow 0.3s ease;
                                    text-align: center;'
                            onmouseover=""this.style.backgroundColor='#0056b3'; this.style.boxShadow='0px 4px 10px rgba(0, 0, 0, 0.1)';""
                            onmouseout=""this.style.backgroundColor='#007bff'; this.style.boxShadow='none';"">
                            Đổi mật khẩu
                        </a>
                    </div>
                            <p style='line-height: 1.6; color: #555;'>Cảm ơn bạn đã lựa chọn chúng tôi!</p>
                            <div style='margin-top: 20px; font-size: 0.9em; color: #777;'>
                                <p>Trân trọng,</p>
                                <p>Đội ngũ hỗ trợ</p>
                            </div>
                        </div>
                    </body>
                    </html>
                    ";

            // Gửi email chứa đường link reset mật khẩu
            await _emailService.SendEmailAsync(user.Email, "[TutorConnect] Đặt lại mật khẩu của bạn", emailContent);
            var response = ApiResponseFactory.Success<object>(null,"Đường link đặt lại mật khẩu đã được gửi qua email.");

            return Ok(response);
        }


        [HttpPost]
        [Route(Common.Url.User.Identity.ResetPassword)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await accountRepo.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await accountRepo.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {

                return Ok(ApiResponseFactory.Success<object>(null,"Đổi mật khẩu thành công"));
            }

            return BadRequest("Password reset failed.");
        }
        [HttpPost]
        [Route(Common.Url.User.Identity.SignIn)]
        [ProducesResponseType(typeof(ApiResponse<SignInResponseDto>), 200)]
        public async Task<IActionResult> SignIn(SignInModel signInModel)
        {

            var user = await _userManager.FindByEmailAsync(signInModel.Email);
            if (user == null)
            {
                return Unauthorized(); // Nếu người dùng không tồn tại
            }

            // Kiểm tra LockoutEnabled để xác định xem tài khoản có bị block không
            if (!user.LockoutEnabled) 
            {
                return BadRequest("Tài khoản của bạn đã bị block.");
            }

            // Thực hiện đăng nhập
            var result = await accountRepo.SignInAsync(signInModel);
            if (result == null)
            {
                return Unauthorized();
            }

            return Ok(ApiResponseFactory.Success(result));
        }

        [HttpPost]
        [Route(Common.Url.User.Identity.SignOut)]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return Ok("You have been signed out.");
        }


        [HttpGet]
        [Route(Common.Url.User.Identity.ViewAllAccounts)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ViewAccount>>), 200)]
        public async Task<IActionResult> ViewAllAccounts()
        {
            // Check if the current user is an admin
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                return Unauthorized(ApiResponseFactory.Unauthorized<IEnumerable<ViewAccount>>("Bạn không có quyền Admin."));
            }

            try
            {
                var accounts = await accountRepo.GetAllAccountsAsync();

                if (accounts == null || !accounts.Any())
                {
                    return NotFound(ApiResponseFactory.NotFound<IEnumerable<ViewAccount>>("Không có tài khoản nào trong hệ thống."));
                }

                return Ok(ApiResponseFactory.Success(accounts));
            }
            catch (Exception ex)
            {

                return StatusCode(500, ApiResponseFactory.ServerError<object>(detail: ex.Message));
            }
        }


        [HttpPost]
        [Route(Common.Url.User.Identity.BlockUser)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)] // Định dạng kết quả trả về
        public async Task<IActionResult> BlockUserAsync([FromBody] Guid userId)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponseFactory.BadRequest<object>("Dữ liệu không hợp lệ."));
            }
            // Lấy thông tin user hiện tại
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                // Trả về lỗi nếu không có quyền admin
                return Unauthorized(ApiResponseFactory.Unauthorized<object>("Bạn không có quyền Admin."));
            }
            // Kiểm tra xem admin có đang cố gắng block chính mình không
            if (currentUser.Id == userId)
            {
                return BadRequest("Bạn không thể block chính mình.");
            }

            try
            {
                // Gọi service để block user
                await accountRepo.BlockUserAsync(userId);

                // Trả về kết quả thành công
                return Ok(ApiResponseFactory.Success<object>(null, "Người dùng đã bị block thành công."));
            }
            catch (Exception ex)
            {
                // Trả về lỗi server trong trường hợp gặp lỗi không mong muốn
                return StatusCode(500, ApiResponseFactory.ServerError<object>(detail: ex.Message));
            }
        }
        [HttpPost]
        [Route(Common.Url.User.Identity.UnblockUser)]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)] 
        public async Task<IActionResult> UnblockUserAsync([FromBody] Guid userId)
        {
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !await _userManager.IsInRoleAsync(currentUser, AccountRoles.Admin))
            {
                
                return Unauthorized(ApiResponseFactory.Unauthorized<object>("Bạn không có quyền Admin."));
            }

           
            if (currentUser.Id == userId)
            {
                return BadRequest("Bạn không thể mở khóa chính mình.");
            }

            try
            {
               
                await accountRepo.UnblockUserAsync(userId);

                return Ok(ApiResponseFactory.Success<object>(null, "Người dùng đã được mở khóa thành công."));
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, ApiResponseFactory.ServerError<object>(detail: ex.Message));
            }
        }

        [HttpPost]
        [Route(Common.Url.User.Identity.ChangePassword)]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Attempt to change the password
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                return Ok(ApiResponseFactory.Success<object>(null, "Đổi mật khẩu thành công."));
            }

            // If there are errors, return them
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest($"Đổi mật khẩu không thành công: {errorMessage}");
        }


    }
}
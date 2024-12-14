using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using TutoRum.Services.IService;

public class FirebaseService: IFirebaseService
{
    public FirebaseService()
    {
        // Khởi tạo Firebase App (chỉ cần gọi một lần)
        if (FirebaseApp.DefaultInstance == null)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "tutorconnect-firebase-adminsdk.json");

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(filePath)
            });
        }
    }

    public async Task<bool> SendNotificationAsync(string fcmToken, string title, string body, Dictionary<string, string>? data = null)
    {
        var message = new Message()
        {
            Token = fcmToken.Trim(),
            Notification = new Notification()
            {
                Title = title,
                Body = body
            },
            Data = data // Dữ liệu bổ sung nếu cần
        };

        try
        {
            // Gửi thông báo qua Firebase Cloud Messaging
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return true; // Thành công
        }
        catch (Exception ex)
        {
            return false;
            throw new Exception("Failed to send notification", ex);
        }
    }
}

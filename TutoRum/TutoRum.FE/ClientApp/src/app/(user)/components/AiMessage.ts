import OpenAI from "openai";

export const messages: OpenAI.Chat.Completions.ChatCompletionMessageParam[] = [
  {
    role: "system",
    content:
      "Đây là hệ thống TutorConnect, một nền tảng để kết nối học viên với gia sư",
  },
  {
    role: "assistant",
    content:
      "Xin chào! Tôi là TutoBot, trợ lý ảo của bạn. Bạn có thể hỏi tôi bất kỳ điều gì về TutorConnect!",
  },
  {
    role: "user",
    content: "Tôi có cần trả phí để sử dụng nền tảng TutorConnect không?",
  },
  {
    role: "assistant",
    content:
      "Bạn không mất phụ phí nào khi sử dụng nền tảng. TutorConnect chỉ thu 5% trên mỗi hóa đơn thanh toán cho gia sư để làm chi phí vận hành.",
  },
  {
    role: "user",
    content: "Làm thế nào để xem thông tin chi tiết của một gia sư?",
  },
  {
    role: "assistant",
    content:
      "Bạn vào mục 'Tìm gia sư', chọn gia sư muốn xem và nhấn 'Xem chi tiết' để xem đầy đủ thông tin.",
  },
  {
    role: "user",
    content: "Tôi có thể hủy yêu cầu học sau khi gửi không?",
  },
  {
    role: "assistant",
    content:
      "Có, bạn có thể hủy yêu cầu học sau khi gửi nếu bạn thay đổi ý định.",
  },
  {
    role: "user",
    content: "Hệ thống có hỗ trợ học thử trước khi bắt đầu khóa học không?",
  },
  {
    role: "assistant",
    content: "Hiện tại hệ thống chưa hỗ trợ tính năng học thử.",
  },
  {
    role: "user",
    content: "Làm thế nào để kiểm tra lịch học đã lên lịch?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể kiểm tra lịch học bằng cách vào mục 'Lớp đang học' (đối với học viên) hoặc 'Lớp đang dạy' (đối với gia sư), sau đó chọn xem chi tiết lớp học.",
  },
  {
    role: "user",
    content: "Tôi cần điều kiện gì để đăng ký làm gia sư trên hệ thống?",
  },
  {
    role: "assistant",
    content:
      "Bạn cần điền đầy đủ thông tin cá nhân, có thể upload chứng chỉ hoặc video giới thiệu. Sau khi hệ thống kiểm duyệt, hồ sơ của bạn sẽ được công khai và bạn sẵn sàng nhận lớp.",
  },
  {
    role: "user",
    content: "Làm thế nào để cập nhật lịch dạy của tôi trên nền tảng?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể cập nhật lịch dạy hoặc thời gian rảnh trong phần 'Thông tin gia sư' ở mục 'Cá nhân'.",
  },
  {
    role: "user",
    content: "Tôi có thể xem đánh giá từ học viên ở đâu?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể xem đánh giá tổng thể trong mục 'Nhận xét của học sinh' ở phần cá nhân hoặc xem đánh giá chi tiết trong hồ sơ public của bạn.",
  },
  {
    role: "user",
    content: "Tôi nên làm gì nếu học viên không tham gia buổi học đã lên lịch?",
  },
  {
    role: "assistant",
    content:
      "Nếu học viên không tham gia, bạn có thể chỉnh sửa buổi học hoặc tự thêm mới buổi học nếu cần học bù.",
  },
  {
    role: "user",
    content: "Có giới hạn số lượng học viên mà tôi có thể nhận dạy không?",
  },
  {
    role: "assistant",
    content:
      "TutorConnect không giới hạn số lượng học viên mà bạn nhận, miễn là bạn sắp xếp thời gian hợp lý.",
  },
  {
    role: "user",
    content: "TutorConnect có hỗ trợ học trực tuyến không?",
  },
  {
    role: "assistant",
    content: "Hiện tại TutorConnect không hỗ trợ tính năng học trực tuyến.",
  },
  {
    role: "user",
    content: "Làm thế nào để tìm gia sư cho một nhóm học viên thay vì 1 kèm 1?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể dùng tài khoản học viên để gửi yêu cầu học, nhưng hãy thông báo trước với gia sư về số lượng học viên để họ cân nhắc.",
  },
  {
    role: "user",
    content: "Hệ thống có cung cấp tài liệu học tập không?",
  },
  {
    role: "assistant",
    content: "TutorConnect tích hợp chatbot để hỗ trợ học tập nếu cần thiết.",
  },
  {
    role: "user",
    content: "Làm sao để xem thông báo từ hệ thống?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể xem thông báo bằng cách nhấn vào biểu tượng chuông thông báo bên cạnh hồ sơ cá nhân.",
  },
  {
    role: "user",
    content: "TutorConnect bảo mật thông tin cá nhân của tôi như thế nào?",
  },
  {
    role: "assistant",
    content:
      "Thông tin cá nhân của bạn, bao gồm email và số điện thoại, sẽ được bảo mật tuyệt đối. Thông tin này chỉ được công khai nếu bạn cho phép.",
  },
  {
    role: "user",
    content: "TutorConnect hỗ trợ các phương thức thanh toán nào?",
  },
  {
    role: "assistant",
    content:
      "TutorConnect hỗ trợ thanh toán qua VNPay hoặc tài khoản ngân hàng.",
  },
  {
    role: "user",
    content: "Làm thế nào để kiểm tra số dư trong ví của tôi?",
  },
  {
    role: "assistant",
    content:
      "Nếu bạn là gia sư, bạn có thể kiểm tra số dư trong mục 'Ví của tôi' ở phần cá nhân.",
  },
  {
    role: "user",
    content: "Làm sao để rút tiền từ hệ thống?",
  },
  {
    role: "assistant",
    content:
      "Bạn có thể tạo yêu cầu rút tiền, nhập thông tin tài khoản ngân hàng của mình. Hệ thống sẽ xử lý và chuyển tiền về tài khoản của bạn.",
  },
];

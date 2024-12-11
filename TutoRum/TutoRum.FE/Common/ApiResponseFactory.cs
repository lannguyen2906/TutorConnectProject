namespace TutoRum.FE.Common
{
    public static class ApiResponseFactory
    {
        // Phản hồi thành công
        public static ApiResponse<T> Success<T>(T data, string message = "Request completed successfully.")
        {
            return new ApiResponse<T>(200, true, message, data);
        }

        // Phản hồi khi không tìm thấy
        public static ApiResponse<T> NotFound<T>(string message = "Item not found.", string code = "ITEM_NOT_FOUND")
        {
            var errors = new List<ApiError>
        {
            new ApiError(code, message)
        };
            return new ApiResponse<T>(404, false, message, default, errors);
        }

        // Phản hồi lỗi máy chủ
        public static ApiResponse<T> ServerError<T>(string message = "An internal server error occurred.", string code = "INTERNAL_SERVER_ERROR", string detail = "")
        {
            var errors = new List<ApiError>
        {
            new ApiError(code, detail)
        };
            return new ApiResponse<T>(500, false, message, default, errors);
        }

        // Phản hồi không ủy quyền (Unauthorized)
        public static ApiResponse<T> Unauthorized<T>(string message = "Unauthorized request.", string code = "UNAUTHORIZED")
        {
            var errors = new List<ApiError>
        {
            new ApiError(code, message)
        };
            return new ApiResponse<T>(401, false, message, default, errors);
        }

        // Phản hồi không hợp lệ (Bad Request)
        public static ApiResponse<T> BadRequest<T>(string message = "Bad request.", string code = "BAD_REQUEST")
        {
            var errors = new List<ApiError>
            {
                new ApiError(code, message)
            };
            return new ApiResponse<T>(400, false, message, default, errors);
        }

    }
}

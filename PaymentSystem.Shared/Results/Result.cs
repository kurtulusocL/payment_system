
namespace PaymentSystem.Shared.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int StatusCode { get; private set; }

        private Result() { }

        public static Result<T> Success(T data, int statusCode = 200)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static Result<T> Failure(string errorMessage, int statusCode = 400)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }

    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int StatusCode { get; private set; }

        private Result() { }

        public static Result Success(int statusCode = 200)
        {
            return new Result
            {
                IsSuccess = true,
                StatusCode = statusCode
            };
        }

        public static Result Failure(string errorMessage, int statusCode = 400)
        {
            return new Result
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }
}

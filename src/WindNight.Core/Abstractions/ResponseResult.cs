namespace WindNight.Core
{
    /// <summary> 响应结构体 </summary>
    public class ResponseResult
    {
        /// <summary> 响应码 </summary>
        public int Code { get; set; }

        /// <summary> 响应信息 </summary>
        public string? Message { get; set; }
    }

    /// <summary>
    ///     响应结构体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseResult<T> : ResponseResult
    {
        /// <summary> </summary>
        public ResponseResult() : this(default!)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        public ResponseResult(T data)
        {
            Data = data;
        }

        /// <summary> 响应数据 </summary>
        public T Data { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> Ok(T data)
        {
            return new ResponseResult<T>
            {
                Code = 0,
                Message = string.Empty,
                Data = data
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> SystemError(string? message = null)
        {
            return new ResponseResult<T>
            {
                Code = 100500,
                Message = message ?? "NOT FOUND",
                Data = default
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> NotFound(string? message = null)
        {
            return new ResponseResult<T>
            {
                Code = 100404,
                Message = message ?? "NOT FOUND",
                Data = default
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> BadRequest(string message)
        {
            return new ResponseResult<T>
            {
                Code = 100400,
                Message = message ?? "",
                Data = default
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> BadRequest(int code, string message)
        {
            return new ResponseResult<T>
            {
                Code = code,
                Message = message ?? "",
                Data = default
            };
        }
    }
}
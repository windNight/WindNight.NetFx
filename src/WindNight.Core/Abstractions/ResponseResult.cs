using System;

namespace WindNight.Core
{
    /// <summary> 响应结构体 </summary>
    public partial class ResponseResult
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
        ///// <summary> 响应码 </summary>
        //public int Code { get; set; }

        ///// <summary> 响应信息 </summary>
        //public string? Message { get; set; }

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
            message = message.IsNullOrEmpty() ? "SystemError" : message;
            return new ResponseResult<T>
            {
                Code = 100500,
                Message = message,
                Data = default
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> NotFound(string? message = null)
        {
            message = message.IsNullOrEmpty() ? "NOT FOUND" : message;
            return new ResponseResult<T>
            {
                Code = 100404,
                Message = message,
                Data = default
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult<T> BadRequest(string message)
        {
            message = message.IsNullOrEmpty() ? "BadRequest" : message;
            return new ResponseResult<T>
            {
                Code = 100400,
                Message = message,
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
            message = message.IsNullOrEmpty() ? "BadRequest" : message;
            return new ResponseResult<T>
            {
                Code = code,
                Message = message,
                Data = default
            };
        }


    }


    public partial class ResponseResult
    {

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual ResponseResult Ok()
        {
            return new ResponseResult
            {
                Code = 0,
                Message = string.Empty,
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult SystemError(string? message = null)
        {
            message = message.IsNullOrEmpty() ? "SystemError" : message;
            return new ResponseResult
            {
                Code = 100500,
                Message = message,
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult NotFound(string? message = null)
        {
            message = message.IsNullOrEmpty() ? "NOT FOUND" : message;
            return new ResponseResult
            {
                Code = 100404,
                Message = message,
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult BadRequest(string message)
        {
            message = message.IsNullOrEmpty() ? "BadRequest" : message;
            return new ResponseResult
            {
                Code = 100400,
                Message = message,
            };
        }

        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual ResponseResult BadRequest(int code, string message)
        {
            message = message.IsNullOrEmpty() ? "BadRequest" : message;
            return new ResponseResult
            {
                Code = code,
                Message = message,
            };
        }



    }



    public partial class ResponseResult
    {

        public static ResponseResult GenOkRes() => new ResponseResult().Ok();

        public static ResponseResult<T> GenOkRes<T>(T data) => new ResponseResult<T>().Ok(data);

        public static ResponseResult GenSystemErrorRes(string message) => new ResponseResult().SystemError(message);

        public static ResponseResult<T> GenSystemErrorRes<T>(string message) => new ResponseResult<T>().SystemError(message);


        public static ResponseResult GenBadRes(string message) => new ResponseResult().BadRequest(message);

        public static ResponseResult<T> GenBadRes<T>(string message) => new ResponseResult<T>().BadRequest(message);


        public static ResponseResult GenBadRes(int code, string message) => new ResponseResult().BadRequest(code, message);

        public static ResponseResult<T> GenBadRes<T>(int code, string message) => new ResponseResult<T>().BadRequest(code, message);


        public static ResponseResult GenNotFoundRes(string message) => new ResponseResult().NotFound(message);

        public static ResponseResult<T> GenNotFoundRes<T>(string message) => new ResponseResult<T>().NotFound(message);







    }
}
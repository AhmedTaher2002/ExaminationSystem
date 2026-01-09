using ExaminationSystem.Models.Enums;
using System.Runtime.CompilerServices;

namespace ExaminationSystem.ViewModels.Response
{
    public class ResponseViewModel<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public ErrorCode IsError { get; set; }
        public string Massage { get; set; }

        /*
        public static ResponseViewModel<T> Success (T data) {
            return new ResponseViewModel<T>()
            {
                Data = data,
                IsSuccess = true,
                IsError = ErrorCode.NoError,
                Massage =""
            };
        }
        */

    }
}

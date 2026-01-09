using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.ViewModels.Response
{
    public class FailResponseViewModel<T> : ResponseViewModel<T>
    {
        public FailResponseViewModel(string message, ErrorCode errorCode)
        {
            IsSuccess = false;
            IsError = errorCode;
            Massage = message;
            Data = default;
        }
    }
}

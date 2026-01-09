using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.ViewModels.Response
{
    public class SuccessResponseViewModel<T>:ResponseViewModel<T>
    {
        public SuccessResponseViewModel(T data) { 
            Data= data;
            IsError = ErrorCode.NoError;
            IsSuccess = true;
            Massage = "";
        }
    }
}

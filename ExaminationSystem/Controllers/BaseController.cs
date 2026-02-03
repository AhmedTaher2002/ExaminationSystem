using AutoMapper;
using ExaminationSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;

        protected BaseController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected ResponseViewModel<TDistination> HandleResult<TSourse, TDistination>(ResponseViewModel<TSourse> result)
        {
            if (!result.IsSuccess)
            {
                return new FailResponseViewModel<TDistination>(
                    result.Massage,
                    result.IsError
                );
            }

            var data = _mapper.Map<TDistination>(result.Data);
            return new SuccessResponseViewModel<TDistination>(data);
        }
    }
}

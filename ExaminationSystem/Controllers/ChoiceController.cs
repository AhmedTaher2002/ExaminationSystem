using AutoMapper;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Choice;
using ExaminationSystem.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChoiceController : BaseController
    {
        private readonly ChoiceService _choiceService;

        public ChoiceController(IMapper mapper):base(mapper)
        {
            _choiceService = new ChoiceService(mapper);
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>> GetAll()
        {
            var result = await _choiceService.GetAll();
            return HandleResult<IEnumerable<GetAllChoicesDTO>, IEnumerable<GetAllChoicesViewModel>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetChoiceByIdViewModel>> GetByID(int id)
        {
            var result = await _choiceService.GetByID(id);
            return HandleResult<GetChoiceByIdDTO,GetChoiceByIdViewModel> (result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateChoiceDTO dto)
        {
            var result = await _choiceService.Create(dto);
            return HandleResult<bool,bool>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateChoiceDTO dto)
        {
            var result = await _choiceService.Update(id, dto);
            return HandleResult<bool, bool>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _choiceService.SoftDelete(id);
            return HandleResult<bool, bool>(result);
        }

        [HttpGet("question/{questionId}")]
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>> GetChoicesForQuestion(int questionId)
        {
            var result = await _choiceService.GetChoiceForQuestionID(questionId);
            return HandleResult<IEnumerable<GetAllChoicesDTO>, IEnumerable<GetAllChoicesViewModel>>(result);
        }
    }
}

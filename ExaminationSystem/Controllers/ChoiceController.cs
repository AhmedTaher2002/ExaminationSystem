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
    public class ChoiceController : ControllerBase
    {
        private readonly ChoiceService _choiceService;
        private readonly IMapper _mapper;

        public ChoiceController(ChoiceService choiceService, IMapper mapper)
        {
            _choiceService = choiceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>> GetAll()
        {
            var result = await _choiceService.GetAll();
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>>(result);
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<GetChoiceByIdViewModel>> GetByID(int id)
        {
            var result = await _choiceService.GetByID(id);
            return _mapper.Map<ResponseViewModel<GetChoiceByIdViewModel>>(result);
        }

        [HttpPost]
        public async Task<ResponseViewModel<bool>> Create([FromBody] CreateChoiceDTO dto)
        {
            var result = await _choiceService.Create(dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(int id, [FromBody] UpdateChoiceDTO dto)
        {
            var result = await _choiceService.Update(id, dto);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> SoftDelete(int id)
        {
            var result = await _choiceService.SoftDelete(id);
            return _mapper.Map<ResponseViewModel<bool>>(result);
        }

        [HttpGet("question/{questionId}")]
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>> GetChoicesForQuestion(int questionId)
        {
            var result = await _choiceService.GetChoiceForQuestionID(questionId);
            return _mapper.Map<ResponseViewModel<IEnumerable<GetAllChoicesViewModel>>>(result);
        }
    }
}

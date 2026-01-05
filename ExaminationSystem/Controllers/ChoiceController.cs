using AutoMapper;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.Models;
using ExaminationSystem.Services;
using ExaminationSystem.ViewModels.Choice;
using Microsoft.AspNetCore.Mvc;
namespace ExaminationSystem.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ChoiceController : ControllerBase
    {
        private readonly ChoiceService _service;
        private readonly IMapper _mapper;
        public ChoiceController(IMapper mapper)
        {
            _service = new ChoiceService(mapper);
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GetAllChoicesViewModel> GetByQuestion(int questionId)
        {
            var choices = _mapper.Map<IEnumerable<GetAllChoicesViewModel>>(_service.GetChoiceForQuestionID(questionId));
            return choices;
            /*
            return _service.GetByQuestionID(questionId)
                .Select(c => new GetAllChoicesViewModel
                {
                    ID = c.ID,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                });
            */
        }

        [HttpPost]
        public async Task<bool> Create(CreateChoiceViewModel vm)
        {
            await _service.Create(new CreateChoiceDTO
            {
                Text = vm.Text,
                IsCorrect = vm.IsCorrect,
                QuestionId = vm.QuestionId
            });
            return true;
        }

        [HttpPut]
        public async Task Update(int id, UpdateChoiceViewModel updateChoiceViewModel)
        {
            await _service.Update(id, _mapper.Map<UpdateChoiceDTO>(updateChoiceViewModel));
        }

    }
}

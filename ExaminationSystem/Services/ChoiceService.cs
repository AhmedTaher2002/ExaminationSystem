using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class ChoiceService
    {
        private readonly ChoiceRepository _choiceRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        public ChoiceService(IMapper mapper)
        {
            _choiceRepository = new ChoiceRepository();
            _questionRepository = new QuestionRepository();
            _mapper = mapper;

        }
        public IEnumerable<GetAllChoicesDTO> GetAll()
        {
            /*return _choiceRepository.GetAll()
                .Select(c => new GetAllChoicesDTO
                {
                    ID = c.ID,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    QuestionId = c.QuestionId
                })
                .AsNoTracking().ToList();*/
            var choice =_choiceRepository.GetAll().ToList();
            return _mapper.Map<List<GetAllChoicesDTO>>(choice);
        }
        public async Task<GetChoiceByIdDTO> GetByID(int id)
        {
            if (!_choiceRepository.IsExist(id))
                throw new Exception("Choice Not Found");

            var choice = await _choiceRepository.GetByIDWithTracking(id);
            return _mapper.Map<GetChoiceByIdDTO>(choice);
            /*var res= new GetChoiceByIdDTO
            {
                ID = choice.ID,
                Text = choice.Text,
                IsCorrect = choice.IsCorrect,
                QuestionId = choice.QuestionId
            return res;
            };*/
        }
        public async Task<GetAllChoicesDTO> Get(int? id,string? text) { 
            var query = _choiceRepository.GetAll().AsQueryable();
            if (id.HasValue)
            {
                query = query.Where(c => c.ID == id.Value);
            }
            if (!string.IsNullOrEmpty(text))
            {
                query = query.Where(c => c.Text.Contains(text));
            }
            var choice = await query.AsNoTracking().FirstOrDefaultAsync() ?? throw new Exception("StudentCourse Not Found");
            
            /*return new GetAllChoicesDTO
            {
                ID = choice.ID,
                Text = choice.Text,
                IsCorrect = choice.IsCorrect,
                QuestionId = choice.QuestionId
            };*/
            return _mapper.Map<GetAllChoicesDTO>(choice);
        }
        public async Task Create(CreateChoiceDTO dto)
        {
            if (!_questionRepository.IsExist(dto.QuestionId))
                throw new Exception("Question Not Found");
            /*
            Choice choice = new Choice
            {
                Text = dto.Text,
                IsCorrect = dto.IsCorrect,
                QuestionId = dto.QuestionId
            };
            */
            Choice choice= _mapper.Map<Choice>(dto);
            await _choiceRepository.Add(choice);
        }
        public async Task Update(int id, UpdateChoiceDTO dto)
        {
            if (!_choiceRepository.IsExist(id))
                throw new Exception("Choice Not Found");

            if (!_questionRepository.IsExist(dto.QuestionId))
                throw new Exception("Question Not Found");
            var choice =await GetByID(id);
            dto = new ()
            {
                Text = dto.Text == "string" ? choice.Text: dto.Text,
                IsCorrect = dto.IsCorrect==default?choice.IsCorrect:dto.IsCorrect,
                QuestionId=dto.QuestionId==0?choice.QuestionId:dto.QuestionId
            };
            /*
            Choice choice =new Choice
            {
                ID = id,
                Text = dto.Text,
                IsCorrect = dto.IsCorrect,
                QuestionId = dto.QuestionId
            };
            */
            var choiceUpdate = _mapper.Map<Choice>(dto);
            await _choiceRepository.Update(choiceUpdate);
        }
        public async Task SoftDelete(int choiceId)
        {
            if (!_choiceRepository.IsExist(choiceId))
                throw new Exception("Choice Not Found");
            
            await _choiceRepository.SoftDelete(choiceId);
        }
        public async Task HardDelete(int choiceId)
        {
            if (!_choiceRepository.IsExist(choiceId))
                throw new Exception("Choice Not Found");

            await _choiceRepository.HardDelete(choiceId);
        }
        
        //--------------------------------SRS CONTROLLER HELPERS
        public List<GetAllChoicesDTO> GetChoiceForQuestionID(int questionId)
        {
            if (!_questionRepository.IsExist(questionId))
                throw new Exception("Question Not Found");
            /*
            var res = _choiceRepository.GetAll()
                .Where(c => c.QuestionId == questionId)
                .Select(c => new GetAllChoicesDTO
                {
                    ID = c.ID,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect,
                    QuestionId = c.QuestionId
                })
                .AsNoTracking().ToList();
            */
            var res=_mapper.Map<List<GetAllChoicesDTO>>(_choiceRepository.GetAll().Where(   c=>c.QuestionId==questionId).ToList()  );
            return res;
        }
    }
}

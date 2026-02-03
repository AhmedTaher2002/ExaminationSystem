using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExaminationSystem.DTOs.Choice;
using ExaminationSystem.Models;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services
{
    public class ChoiceService
    {
        #region Repositories & Mapper
        private readonly ChoiceRepository _choiceRepository;
        private readonly QuestionRepository _questionRepository;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public ChoiceService(IMapper mapper)
        {
            _choiceRepository = new ChoiceRepository();
            _questionRepository = new QuestionRepository();
            _mapper = mapper;
        }
        #endregion

        #region Choice CRUD

        // Get all choices
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesDTO>>> GetAll()
        {
            var choices = _choiceRepository.GetAll();
            var result = choices.ProjectTo<GetAllChoicesDTO>(_mapper.ConfigurationProvider);
            return new SuccessResponseViewModel<IEnumerable<GetAllChoicesDTO>>(result);
        }

        // Get choice by ID
        public async Task<ResponseViewModel<GetChoiceByIdDTO>> GetByID(int id)
        {
            if (!_choiceRepository.IsExists(id))
                return new FailResponseViewModel<GetChoiceByIdDTO>("Choice ID does not exist", ErrorCode.InvalidChoiceId);

            var choice = _choiceRepository.GetByID(id);
            var result = await choice.ProjectTo<GetChoiceByIdDTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (result is null)
                return new FailResponseViewModel<GetChoiceByIdDTO>("Choice not found", ErrorCode.ChoiceNotFound);

            return new SuccessResponseViewModel<GetChoiceByIdDTO>(result);
        }

        // Create new choice
        public async Task<ResponseViewModel<bool>> Create(CreateChoiceDTO dto)
        {
            if (!CreateDataValidate(dto))
                return new FailResponseViewModel<bool>("Invalid data for creating choice", ErrorCode.ChoiceNotCreated);

            var choice = _mapper.Map<Choice>(dto);
            await _choiceRepository.AddAsync(choice);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Validate create data
        private bool CreateDataValidate(CreateChoiceDTO createChoiceDTO)
        {
            if (createChoiceDTO == null)
                return false;

            if (string.IsNullOrEmpty(createChoiceDTO.Text) || createChoiceDTO.QuestionId <= 0)
                return false;

            if (!_questionRepository.IsExists(createChoiceDTO.QuestionId))
                return false;

            if (_choiceRepository.IsExists(createChoiceDTO.Text, createChoiceDTO.QuestionId))
                return false;

            return true;
        }

        // Update existing choice
        public async Task<ResponseViewModel<bool>> Update(int id, UpdateChoiceDTO dto)
        {
            if (!UpdateDataValidate(dto))
                return new FailResponseViewModel<bool>("Invalid data for updating choice", ErrorCode.ChoiceNotUpdated);

            var choice = await _choiceRepository.GetByIDWithTracking(id);

            dto = new()
            {
                Text = dto.Text == "string" ? choice.Text : dto.Text,
                IsCorrect = dto.IsCorrect == default ? choice.IsCorrect : dto.IsCorrect,
                QuestionId = dto.QuestionId == 0 ? choice.QuestionId : dto.QuestionId
            };

            var choiceUpdate = _mapper.Map<Choice>(dto);
            await _choiceRepository.UpdateAsync(choiceUpdate);

            return new SuccessResponseViewModel<bool>(true);
        }

        // Validate update data
        private bool UpdateDataValidate(UpdateChoiceDTO updateChoiceDTO)
        {
            if (updateChoiceDTO == null)
                return false;

            if (string.IsNullOrEmpty(updateChoiceDTO.Text) || updateChoiceDTO.QuestionId <= 0)
                return false;

            if (!_questionRepository.IsExists(updateChoiceDTO.QuestionId))
                return false;

            if (_choiceRepository.IsExists(updateChoiceDTO.Text, updateChoiceDTO.QuestionId))
                return false;

            return true;
        }

        // Soft delete a choice
        public async Task<ResponseViewModel<bool>> SoftDelete(int choiceId)
        {
            if (!_choiceRepository.IsExists(choiceId))
                return new FailResponseViewModel<bool>("Choice does not exist", ErrorCode.ChoiceNotFound);

            await _choiceRepository.SoftDeleteAsync(choiceId);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Get all choices for a specific question
        public async Task<ResponseViewModel<IEnumerable<GetAllChoicesDTO>>> GetChoiceForQuestionID(int questionId)
        {
            if (!_questionRepository.IsExists(questionId))
                return new FailResponseViewModel<IEnumerable<GetAllChoicesDTO>>("Question does not exist", ErrorCode.InvalidQuestionId);

            var questionChoices = _choiceRepository.GetAll().Where(c => c.QuestionId == questionId && !c.IsDeleted);
            var result = await questionChoices.ProjectTo<GetAllChoicesDTO>(_mapper.ConfigurationProvider).ToListAsync();

            return new SuccessResponseViewModel<IEnumerable<GetAllChoicesDTO>>(result);
        }

        #endregion
    }
}

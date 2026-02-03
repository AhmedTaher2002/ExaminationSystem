using ExaminationSystem.Data;
using ExaminationSystem.Models.Enums;
using ExaminationSystem.Repositories;
using ExaminationSystem.ViewModels.Response;

namespace ExaminationSystem.Services
{
    public class RoleFeatureService
    {
        #region Repositories & Context

        public RoleFeatureRepository _roleFeatureRepository;
        public Context _context;

        public RoleFeatureService()
        {
            _roleFeatureRepository = new RoleFeatureRepository();
            _context = new Context();
        }

        #endregion

        #region Role-Feature Management

        // Assign a feature to a role
        public async Task<ResponseViewModel<bool>> AssignFeatureToRole(Role role, Feature feature)
        {
            if (_roleFeatureRepository.IsExists(role, feature))
            {
                return new FailResponseViewModel<bool>("This role already has the specified feature assigned.",ErrorCode.RoleAlreadyHasFeature
                );
            }

            await _roleFeatureRepository.AddAsync(role, feature);
            return new SuccessResponseViewModel<bool>(true);
        }

        // Remove a feature from a role
        public async Task<ResponseViewModel<bool>> RemoveFeatureFromRole(Role role, Feature feature)
        {
            if (!_roleFeatureRepository.IsExists(role, feature))
            {
                return new FailResponseViewModel<bool>(
                    "This role does not have the specified feature assigned.",ErrorCode.RoleDoesNotHaveFeature);
            }

            await _roleFeatureRepository.SoftDeleteAsync(role, feature);
            return new SuccessResponseViewModel<bool>(true);
        }

        #endregion
    }
}

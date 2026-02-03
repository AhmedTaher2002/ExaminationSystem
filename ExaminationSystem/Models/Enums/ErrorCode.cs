namespace ExaminationSystem.Models.Enums
{
    public enum ErrorCode
    {
        #region AuthenticationService (1000-1099)
        InvalidStudentEmail = 1000,
        InvalidPassword = 1001,
        UserNotFound = 1002,
        StudentInvalidCredentials = 1003,
        InstructorInvalidCredentials = 1004,
        #endregion

        #region RoleFeatureService (1100-1199)
        RoleDoesNotHaveFeature = 1100,
        RoleAlreadyHasFeature = 1101,
        RoleFeatureNotFound = 1102,
        #endregion

        #region InstructorService (1200-1299)
        InstructorNotFound = 1200,
        InvalidInstructorId = 1201,
        StudentAlreadyEnrolled = 1202,
        #endregion

        #region StudentService (1300-1399)
        StudentNotFound = 1300,
        InvalidStudentId = 1301,
        StudentNotCreated = 1302,
        StudentNotUpdated = 1303,
        StudentAlreadyAssignedToCourse = 1304,
        StudentNotAssignedToCourse = 1305,
        StudentStartedExam = 1306,
        StudentAlreadyTookFinalExam = 1307,
       
        #endregion

        #region CourseService (1400-1499)
        InvalidCourseId = 1400,
        CourseNotFound = 1401,
        CourseAlreadyExists = 1402,
        CourseNotCreated = 1403,
        InvalidCourseFilter = 1404,
        CourseHasNoStudents = 1405,
        #endregion

        #region ExamService (1500-1599)
        InvalidExamId = 1500,
        ExamNotFound = 1501,
        InvalidExamData = 1502,
        NotEnoughQuestions = 1503,
        QuestionAlreadyAssigned = 1504,
        StudentNotAssignedToExam = 1505,
        StudentNotSubmittedExam = 1506,
        InvalidTotalQuestions = 1507,
        TotalPercentageNot100 = 1508,
        ExamTimeExpired = 1509,
        ExamNotStarted = 1510,
        ExamAlreadySubmitted = 1511,
        #endregion

        #region QuestionService (1600-1699)
        InvalidQuestionId = 1600,
        QuestionNotFound = 1601,
        QuestionNotUpdated = 1602,
        QuestionNotCreated = 1603,
        QuestionNotAssignedToExam = 1604,
        InvalidQuestion = 1605,
        #endregion

        #region ChoiceService (1700-1799)
        ChoiceNotFound = 1700,
        ChoiceNotCreatedError = 1701,
        ChoiceNotUpdated = 1702,
        InvalidChoiceId = 1703,
        ChoiceNotCreated = 1704,
        InvalidChoice = 1705,
        #endregion
    }
}

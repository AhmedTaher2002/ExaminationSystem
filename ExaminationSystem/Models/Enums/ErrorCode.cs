namespace ExaminationSystem.Models.Enums
{
    public enum ErrorCode
    {
        NoError = 0,

        // Course related
        InvalidCourseId = 101,
        CourseNotFound = 103,
        CourseAreadyExists = 104,
        CourseNotCreated = 106,
        InvalidCourseFilter= 107,

        // Instructor related
        InvalidInstrutorId = 201,
        InstrutorNotFound = 203,
        InstrutorAreadyExists = 204,
        InstructorNotCreated = 206,
        InvalidInstrutorEmail = 207,
        InstrutorEmailAlreadyExists = 208,

        // Student related
        InvalidStudentId = 301,
        StudentNotFound = 303,
        StudentAreadyExists = 304,
        StudentNotCreated = 306,
        InvalidStudentEmail = 307,
        StudentEmailAlreadyExists = 308,
        StudentNotUpdated = 309,
        StudentNotAssignedToCourse = 708,
        StudentStartedExam = 709,
        StudentAreadyTakeFinalExam = 710,

        // Exam related
        InvalidExamId = 401,
        ExamNotFound = 403,
        ExamNotCreated = 406,
        ExamDateInvalid = 407,
        ExamDurationInvalid = 408,

        // Question related
        InvalidQuestionId = 501,
        QuestionNotFound = 503,
        QuestionNotCreated = 506,
        QuestionNotUpdated = 507,
        QuestionAreadyExists= 508,
        InvalidQuestion = 509,

        // Choice related
        invalidChoiceId = 601,
        ChoiceNotFound = 603,
        ChoiceNotCreated = 606,
        ChoiceDoesNotBelongToQuestion = 607,
        ChoiceNotUpdated = 608,

        // Exam & student assignment related
        StudentNotEnrolledInCourse = 701,
        StudentNotAssignedToExam = 702,
        StudentNotSubmittedExam = 703,
        TotalPrcentageNot100 = 705,
        TolalPrcentageInvalid = 706,
        QuestionNotAssignedToExam = 707,
        InvalidChoice = 711,
        ExamNotStarted = 712,
        ExamAlreadySubmitted = 713,
        ExamTimeExpired = 714
    }
}

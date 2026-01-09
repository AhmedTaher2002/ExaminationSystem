namespace ExaminationSystem.Models.Enums
{
    public enum ErrorCode
    {
        NoError = 0,

        InvalidCourseId =101,
        InvalidCourseName =102,
        CourseNotFound=103,
        CourseAreadyExists=104,
        InvalidCourseFilter=105,
        CourseNotCreated=106,

        InvalidInstrutorId =201,
        InvalidInstrutorName=202,
        InstrutorNotFound=203,
        InstrutorAreadyExists=204,
        InvalidInstructorFilter=205,
        InstructorNotCreated =206,
        InvalidInstrutorEmail =207,
        InstrutorEmailAlreadyExists =208,
        
        InvalidStudentId =301,
        InvalidStudentName=302,
        StudentNotFound=303,
        StudentAreadyExists=304,
        InvalidStudentFilter=305,
        StudentNotCreated=306,
        InvalidStudentEmail =307,
        StudentEmailAlreadyExists =308,
        StudentNotUpdated=309,


        InvalidExamId =401,
        InvalidExamType =402,
        ExamNotFound=403,
        ExamAreadyExists=404,
        InvalidExamFilter=405,
        ExamNotCreated=406,
        ExamDateInvalid=407,
        ExamDurationInvalid=408,

        InvalidQuestionId =501,
        InvalidQuestionText=502,
        QuestionNotFound=503,
        QuestionAreadyExists=504,
        InvalidQuestionFilter=505,
        QuestionNotCreated=506,
        QuestionNotUpdated=507,

        invalidChoiceId =601,
        invalidChoiceText=602,
        ChoiceNotFound=603,
        ChoiceAreadyExists=604,
        InvalidChoiceFilter=605,
        ChoiceNotCreated=606,
        ChoiceDoesNotBelongToQuestion=607,
        ChoiceNotUpdated=608,

        StudentNotEnrolledInCourse=701,
        StudentNotAssignedToExam=702,
        StudentNotSubmittedExam=703,
        InvalidPercentageEnter= 704,
        TotalPrcentageNot100= 705,
        TolalPrcentageInvalid = 706,
        QuestionNotAssignedToExam= 707,
        StudentNotAssignedToCourse= 708,
        StudentStartedExam= 709,
        StudentAreadyTakeFinalExam= 710
    }
}

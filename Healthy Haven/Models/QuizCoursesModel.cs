using Microsoft.AspNetCore.Mvc.Rendering;

namespace Healthy_Haven.Models
{
    public class QuizCoursesModel
    {
        public QuizzesModel Quizzes { get; set; }

        public List<CoursesModel> CoursesList { get; set; }

        
    }
}

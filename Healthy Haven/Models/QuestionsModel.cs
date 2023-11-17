using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Healthy_Haven.Models
{
    public class QuestionsModel
    {
        [Key]
        public int Id { get; set; }

        public int QuizId { get; set; }

        [StringLength(500)]
        public string QuestionText { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        //public QuizzesModel Quiz { get; set; }

        // Collection navigation property for options
        //public ICollection<OptionsModel> Options { get; set; }

    }
}

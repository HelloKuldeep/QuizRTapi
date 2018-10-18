using System.Collections.Generic;
namespace QuizRT.Models{
    public interface IQuizRTRepo {
        List<QuizRTTemplate> GetTemplate();
        List<Questions> GetQuestion();
        List<Options> GetOption();
        bool PostTemplate(QuizRTTemplate q);
        void DeleteTemplate();
        // ------------
        void GenerateQuestion(string category);

    }
}
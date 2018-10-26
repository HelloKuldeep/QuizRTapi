using System.Collections.Generic;
using System;
namespace QuizRT.Models{
    public interface IQuizRTRepo {
        List<QuizRTTemplate> GetTemplate();
        List<Questions> GetQuestion();
        List<Options> GetOption();
        bool PostQuery(Object q);
        bool PostTemplate(Questions q);
        void DeleteTemplate();

    }
}
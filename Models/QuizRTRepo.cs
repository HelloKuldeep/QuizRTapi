using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace QuizRT.Models{
    public class QuizRTRepo : IQuizRTRepo {
        string sparQL = "";
        int NumberOfQuestions = 100;
        int optionNumber = 3;
        List<string> quesReviewList = new List<string>();
        List<string> optionReviewList = new List<string>();
        QuizRTContext context = null;
        public QuizRTRepo(QuizRTContext _context){
            this.context = _context;
        }
        public List<QuizRTTemplate> GetTemplate(){
            return context.QuizRTTemplateT.ToList();
        }
        public List<Questions> GetQuestion(){
            return context.QuestionsT
            .Include( n => n.QuestionOptions )
            .ToList();
        }
        public List<Options> GetOption(){
            return context.OptionsT.ToList();
        }

        public List<Questions> GetQuestion_directly(QuizRTTemplate template)
       {
           List<Questions> g_question_notable = new List<Questions>();
           Task<string> id= Gettopic_id("https://www.wikidata.org/w/api.php?action=wbsearchentities&search="+template.TopicName+"&language=en&format=json");
           string f_id = id.Result;
           string sparQL = "SELECT ?personLabel WHERE { ?person wdt:P106 wd:"+f_id+" . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }";
           Task<List<string>> questions = GetQuestionData(sparQL);
           List<string> all_questions_without_tables = questions.Result;
           Task<List<string>> options= GetOptionData("SELECT ?cid ?options WHERE {?cid wdt:P31 wd:Q28640. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit 100");
           List<string> all_options = options.Result;
           for(int i=0;i<all_questions_without_tables.Count;i++)
           {
               Questions s_object = new Questions();
               s_object.QuestionGiven="What is "+all_questions_without_tables[i]+" Occupation ?";
               List<Options> mut_options_single_q = randomizeOptions(all_options,template.TopicName);
               s_object.QuestionOptions=mut_options_single_q;
               g_question_notable.Add(s_object);
           }
           return g_question_notable;
           // Console.WriteLine(all_options+"opopop");
           // Console.WriteLine(all_questions_without_tables+"qqqqq");

       }

       public List<Options> randomizeOptions(List<string> optionReviewList,string topic_name){
           List<string> other_options = new List<string>();
           List<Options> g_List_options_notable = new List<Options>();
           List<int> randomNumber = getRandonNumber(0, optionReviewList.Count-1, optionNumber-1);
           for(int i=0; i < randomNumber.Count ; ) {
               Options g_options_notable_f = new Options();
               g_options_notable_f.OptionGiven=optionReviewList[randomNumber[i]];
               g_options_notable_f.IsCorrect=false;
               g_List_options_notable.Add(g_options_notable_f); 
               i++;
           }
           Options g_options_notable = new Options();
               g_options_notable.OptionGiven=topic_name;
               g_options_notable.IsCorrect=true;
               g_List_options_notable.Add(g_options_notable);  
           return g_List_options_notable;
       }

       async Task<string> Gettopic_id(string sparQL){
           //string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
           //The 'using' will help to prevent memory leaks.
           //Create a new instance of HttpClient
           using (HttpClient client = new HttpClient())
           //Setting up the response...
           using (HttpResponseMessage res = await client.GetAsync(sparQL))
           using (HttpContent content = res.Content){
               string data = await content.ReadAsStringAsync();
               // JObject data = await content.ReadAsAsync<JObject>();
               JObject json = JObject.Parse(data);
               JArray J = (JArray)json["search"];
               string str = (string)J[0]["id"];
               return str;
           }
       }
        public bool PostQuery(Object q){
            JObject jo = (JObject)(q);
            string categId = jo["categ"].ToString();
            string topicId = jo["topic"].ToString();
            string categName = jo["categName"].ToString();
            string topicName = jo["topicName"].ToString();
            // Console.WriteLine("--------"+categName);
            if( context.QuizRTTemplateT.FirstOrDefault( n => n.Categ == categId) == null ){
                QuizRTTemplate qT = new QuizRTTemplate();
                qT.Categ = categId;
                qT.CategName = categName;
                qT.Topic = topicId;
                qT.TopicName = topicName;
                context.QuizRTTemplateT.Add(qT);
                context.SaveChanges();

                if( context.QuestionsT.FirstOrDefault( n => n.Categ == categId) == null ){
                    if ( GenerateQuestion(qT) && GenerateOptions(qT) ) 
                        return true;
                }
                return true;
            }
            return false;
        }
        public bool PostTemplate(Questions q){// QuizRTTemplate
            // if( context.QuizRTTemplateT.FirstOrDefault( n => n.Categ == q.Categ) == null ){
            //     context.QuizRTTemplateT.Add(q);
            //     context.SaveChanges();
            //     return true;
            // }
            // return false;
            context.QuestionsT.Add(q);
            context.SaveChanges();
            return true;
        }
        public void DeleteTemplate(){
            List<QuizRTTemplate> Lqt = context.QuizRTTemplateT.ToList();
            if( Lqt.Count > 0 ){
                context.Database.ExecuteSqlCommand("TRUNCATE TABLE QuizRTTemplateT");
                // context.RemoveRange(Lqt);
                // context.SaveChanges();
            }
            List<Questions> LqtQ = context.QuestionsT.ToList();
            if( LqtQ.Count > 0 ){
                // context.Database.ExecuteSqlCommand("TRUNCATE TABLE [QuestionsT]");
                context.RemoveRange(LqtQ);
                context.SaveChanges();
            }
            List<Options> Lops = context.OptionsT.ToList();
            if( Lops.Count > 0 ){
                // context.Database.ExecuteSqlCommand("TRUNCATE TABLE [OptionsT]");
                context.RemoveRange(Lops);
                context.SaveChanges();
            }
        }

        // ------------------------

        public bool GenerateQuestion(QuizRTTemplate q) {
            sparQL = "SELECT ?personLabel WHERE { ?person wdt:"+q.Topic+" wd:"+q.Categ+" . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT "+NumberOfQuestions+"";
            Task<List<string>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetQuestionData(sparQL).Result);
            List<string> quesReviewList = dataReturns.Result;

            List<Questions> qL = new List<Questions>();
            for(int i=0; i<quesReviewList.Count; i++){
                Questions ques = new Questions();
                ques.QuestionGiven = "What is "+quesReviewList[i]+" "+q.TopicName+"?";
                ques.Topic = q.TopicName;
                ques.Categ = q.CategName;
                qL.Add(ques);
            }
            context.QuestionsT.AddRange(qL);
            context.SaveChanges();
            return true;
        }
        async Task<List<string>> GetQuestionData(string sparQL){
            string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())
            //Setting up the response...         
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content){
                string data = await content.ReadAsStringAsync();
                // JObject data = await content.ReadAsAsync<JObject>();
                JObject json = JObject.Parse(data);
                if (data != null){
                    for(int i=0; i < ((JArray)json["results"]["bindings"]).Count ; i++){  
                        quesReviewList.Add(json["results"]["bindings"][i]["personLabel"]["value"].ToString());
                    }
                    return quesReviewList;
                }
                return new List<string>();
            }
        }
        public bool GenerateOptions(QuizRTTemplate q) {
            sparQL = "SELECT ?cid ?options WHERE {?cid wdt:P31 wd:Q28640. OPTIONAL {?cid rdfs:label ?options filter (lang(?options) = 'en') . }}Limit "+NumberOfQuestions*10+"";
            Task<List<string>> dataReturns = System.Threading.Tasks.Task<string>.Run(() => GetOptionData(sparQL).Result);
            List<string> optionReviewList = dataReturns.Result;
            
            List<Questions> qL = context.QuestionsT
                                        .Where( n => n.Categ == q.CategName)
                                        .ToList();
            
            for(int i=0; i<qL.Count; i++){
                List<Options> oL = new List<Options>();
                oL = randomizeOptions(optionReviewList, q.CategName, qL[i].QuestionsId);
                context.OptionsT.AddRange(oL);
            }
            context.SaveChanges();
            return true;
        }
        async Task<List<string>> GetOptionData(string sparQL){
            string baseUrl = "https://query.wikidata.org/sparql?query="+sparQL+"&format=json";
            //The 'using' will help to prevent memory leaks.
            //Create a new instance of HttpClient
            using (HttpClient client = new HttpClient())
            //Setting up the response...         
            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
            using (HttpContent content = res.Content){
                string data = await content.ReadAsStringAsync();
                // JObject data = await content.ReadAsAsync<JObject>();
                JObject json = JObject.Parse(data);
                // JArray J = (JArray)json["results"]["bindings"];
                if (data != null){
                    for(int i=0; i < ((JArray)json["results"]["bindings"]).Count ; i++){

                        if ( ((JArray)json["results"]["bindings"])[i].Count() >= 2)
                            optionReviewList.Add(json["results"]["bindings"][i]["options"]["value"].ToString());
                    }
                    return optionReviewList;
                }
                return new List<string>();
            }
        }
        public List<Options> randomizeOptions(List<string> optionReviewList, string categName, int qId){
            List<int> randomNumber = getRandonNumber(0, optionReviewList.Count-1, optionNumber-1);

            List<Options> optionPerQues = new List<Options>();
            for(int i=0; i < randomNumber.Count ; i++) {
                // if(optionReviewList[i] == categName){
                    Options ops = new Options();
                    ops.OptionGiven = optionReviewList[randomNumber[i]];
                    ops.IsCorrect = false;
                    ops.QuestionsId = qId;
                    optionPerQues.Add(ops);
                // } else {
                //     randomizeOptions(optionReviewList, categName, qId);
                // }
            }
            Options opsCorrect = new Options();
            opsCorrect.OptionGiven = categName;
            opsCorrect.IsCorrect = true;
            opsCorrect.QuestionsId = qId;
            optionPerQues.Add(opsCorrect);
            // shuffling the option to create randomness
            // optionPerQues = shuffle(optionPerQues);
            return optionPerQues;
        }
        public List<int> getRandonNumber(int iFromNum, int iToNum, int iNumOfItem){
            List<int> lstNumbers = new List<int>();
            Random rndNumber = new Random();

            int number = rndNumber.Next(iFromNum, iToNum + 1);
            lstNumbers.Add(number);
            int count = 0;
            do{
                number = rndNumber.Next(iFromNum, iToNum + 1);
                if (!lstNumbers.Contains(number)){
                    lstNumbers.Add(number);
                    count++;
                }
            } while (count < iNumOfItem);
            return lstNumbers;
        }
        public List<Options> shuffle(List<Options> optionPerQues){

            return optionPerQues;
        }
    }

}
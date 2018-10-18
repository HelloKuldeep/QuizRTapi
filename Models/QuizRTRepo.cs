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
        List<string> quesReviewList = new List<string>();
        string category = "";
        string topic = "";
        QuizRTContext context = null;
        public QuizRTRepo(QuizRTContext _context){
            this.context = _context;
        }
        public List<QuizRTTemplate> GetTemplate(){
            return context.QuizRTTemplateT.ToList();
        }
        public List<Questions> GetQuestion(){
            return context.QuestionsT.ToList();
        }
        public List<Options> GetOption(){
            return context.OptionsT.ToList();
        }
        public bool PostTemplate(QuizRTTemplate q){
            if( context.QuizRTTemplateT.FirstOrDefault( n => n.Categ == q.Categ) == null ){
                context.QuizRTTemplateT.Add(q);
                context.SaveChanges();
                this.GenerateQuestion(q.Categ);
                return true;
            }
            return false;
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
        }

        // ------------------------

        public void GenerateQuestion(string categParam){
            List<QuizRTTemplate> Lqt = context.QuizRTTemplateT
                                                .Where( n => n.Categ == categParam)
                                                .ToList();
            category = Lqt[0].Categ;
            topic = Lqt[0].Topic;
            sparQL = "SELECT ?personLabel WHERE { ?person wdt:"+topic+" wd:"+category+" . SERVICE wikibase:label { bd:serviceParam wikibase:language 'en' . } }LIMIT 10";
            GetData(sparQL);
            // Task<JObject> dataReturns = System.Threading.Tasks.Task<JObject>.Run(() => GetData(sparQL).Result);
            // Console.WriteLine(GetData(sparQL));
            // Questions qu = new Questions();
            // qu.QuestionGiven = "ABC";
            // if( true ){
            //     context.QuestionsT.Add(qu);
            //     context.SaveChanges();
            // }
        }

        static async void GetData(string sparQL){
            //We will make a GET request to a really cool website...
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
                    // Console.WriteLine(json.Count);
                    // Console.WriteLine(json.results.bindings[0].personLabel.value);
                    for(int i=0; i < ((JArray)json["results"]["bindings"]).Count ; i++){
                        Console.WriteLine(json["results"]["bindings"][i]["personLabel"]["value"].ToString());  
                    }  
                    // make call to template T and generate ques string using above loop then insert into question table 
                    // var id = json["results"]["bindings"][0]["personLabel"]["value"].ToString();
                    // return json;
                }
                // return new JObject();
            }
        }

    }

}
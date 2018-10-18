using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuizRT.Models;

namespace QuizRTapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizRTController : ControllerBase
    {
        IQuizRTRepo quizRTRepo;
        public QuizRTController(IQuizRTRepo _quizRTRepo){
            this.quizRTRepo = _quizRTRepo;
        }
        // GET api/values
        [HttpGet]
        // public List<QuizRTTemplate> Get(){
        //     List<QuizRTTemplate> Lqt = quizRTRepo.GetTemplate();
        //     if( Lqt.Count > 0 )
        //         return Lqt;
        //     return new List<QuizRTTemplate>();
        // }
        public IActionResult Get(){
            List<QuizRTTemplate> Lqt = quizRTRepo.GetTemplate();
            if( Lqt.Count > 0 )
                return Ok(Lqt);
            return NotFound("Empty Database");
        }

        // GET api/values/5
        [HttpGet("{id:int}")]
        public IActionResult Get(int id){
            List<Questions> Lqt = quizRTRepo.GetQuestion();
            if( Lqt.Count > 0 )
                return Ok(Lqt);
            return NotFound("Empty Database");
        }
        [HttpGet("{searchstring}")]
        public IActionResult Get(string searchstring, [FromQuery] string type ){
            List<Options> Lo = quizRTRepo.GetOption();
            if( Lo.Count > 0 )
                return Ok(Lo);
            return NotFound("Empty Database");
        }

        // POST api/values
        [HttpPost]
        // public QuizRTTemplate Post([FromBody] QuizRTTemplate q){
        //     if(quizRTRepo.PostTemplate(q)){
        //         return q;
        //     }
        //     return new QuizRTTemplate();
        // }
        public IActionResult Post([FromBody] QuizRTTemplate q){
            if(quizRTRepo.PostTemplate(q)){
                return Created("/api/quizrt",q);
            }
            return BadRequest("Database Error!!");
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id){
            quizRTRepo.DeleteTemplate();
        }
    }
}

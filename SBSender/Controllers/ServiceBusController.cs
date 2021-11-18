using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SBSender.Services;

using SBShared.Models;

using static SBShared.Utils.Enums;

namespace SBSender.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceBusController : ControllerBase
    {
        private readonly ITopicService _topicService;
        private readonly IConfiguration _config;
        public ServiceBusController(ITopicService topicService, IConfiguration config)
        {
            _config = config;
            _topicService = topicService;
        }

        /// <summary>
        /// Posts a message into service bus
        /// </summary>
        /// <param name="input">Message to be sent</param>
        /// <returns>Result</returns>
        [HttpPost("PostExampleModel")]
        public async Task<IActionResult> PostExampleModel([FromBody] ExampleModel input)
        {
            try
            {
                //Creating service bus input
                SBModel<ExampleModel> serviceBusInput = new SBModel<ExampleModel>
                {
                    Message = input,
                    Publisher = _config.GetValue<string>("AzureServiceBus:Publisher")
                };
                
                //Creating async tasks
                var tasks = new List<Task>(); 
                tasks.Add(this._topicService.SendMessageAsync(serviceBusInput, TopicEnum.TestTopic1));

                try
                {
                    // Async call for tasks
                    await Task.WhenAll(tasks);
                }
                catch (Exception)
                {
                    throw;
                }
                
                return Ok();
            }
            catch (Exception ex)
            {
                //TODO Log exception 
                return BadRequest("Handled error message");
            }
        }

    }
}

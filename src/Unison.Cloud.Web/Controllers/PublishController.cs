using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unison.Cloud.Core.Interfaces.Amqp;
using Unison.Cloud.Infrastructure.Amqp;
using Unison.Cloud.Infrastructure.Data.Entities;
using Unison.Cloud.Infrastructure.Data.Repositories;
using Unison.Cloud.Infrastructure.Models;

namespace Unison.Cloud.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly IAmqpPublisher _amqpPublisher;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<PublishController> _logger;

        public PublishController(ILogger<PublishController> logger, IAmqpPublisher amqpClient, IProductRepository productRepository)
        {
            _amqpPublisher = amqpClient;
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            IEnumerable<Product> products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpPost]
        public IActionResult Publish(PublishDTO request)
        {
            _logger.LogInformation("Got request");
            string message = request.Message;

            //_amqpClient.Publish(message);
            //_logger.LogInformation($"Published: {message}");

            return Ok();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationQueue.Entities
{
    public class Position
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "authorId")]
        public string AuthorId { get; set; }

        [JsonProperty(PropertyName = "botId")]
        public string BotId { get; set; }

        [JsonProperty(PropertyName = "number")]
        public int NumberInTheQueue { get; set; }

        [DisplayName("Author")]
        [JsonProperty(PropertyName = "requester")]
        [Required]
        public string Requester { get; set; }

        [JsonProperty(PropertyName = "registrationTime")]
        public DateTime RegistrationTime { get; set; } = DateTime.Now;

        [JsonProperty(PropertyName = "description")]
        [Required]
        public string Description { get; set; }
        
    }
}

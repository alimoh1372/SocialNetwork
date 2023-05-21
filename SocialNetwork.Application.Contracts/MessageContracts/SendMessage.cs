using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Application.Contracts.MessageContracts
{
    public class SendMessage        
    {
        [DisplayName("Message from")]
        [Range(1,long.MaxValue)]
        public long FkFromUserId { get;  set; }

        [DisplayName("Message to")]
        [Range(1, long.MaxValue)]
        public long FkToUserId { get;  set; }

        [DisplayName("Message text")]
        [Required]
        public string MessageContent { get;  set; }
    }
}
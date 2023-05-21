using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Application.Contracts.MessageContracts
{
    public class EditMessage
    {
        public long Id { get; set; }

        [DisplayName("Message text")]
        [Required]
        public string MessageContent { get;  set; }
    }
}
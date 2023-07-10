using System.ComponentModel.DataAnnotations;

namespace ProjectRoomChat.ViewModels
{
    public class UploadViewModel
    {
        [Required]
        public int RoomId { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}

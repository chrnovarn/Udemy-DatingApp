using System;

namespace API.DTOs
{
    public class PhotoDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public Boolean IsMain { get; set; }
    }
}
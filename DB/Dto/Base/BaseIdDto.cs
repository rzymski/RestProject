using DB.Dto.HATEOAS;
using System.ComponentModel.DataAnnotations;

namespace DB.Dto.Base
{
    public abstract class BaseIdDto : BaseDto
    {
        [Required]
        public int Id { get; set; }
        public List<Link> Links { get; set; } = new List<Link>();

        public BaseIdDto() { }

        public BaseIdDto(int id)
        {
            Id = id;
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace DB.Dto.Base
{
    public abstract class BaseIdDto : BaseDto
    {
        [Required]
        public int Id { get; set; }

        public BaseIdDto() { }

        public BaseIdDto(int id)
        {
            Id = id;
        }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Dto
{
    public class TodoItemDto
    {
        [Key]
        public long Id { get; set; }
        [Required(ErrorMessage = "Campo nome é obrigatório")]
        public string Name { get; set; }
        [DefaultValue(false)]
        public bool IsComplete { get; set; }
    }
}
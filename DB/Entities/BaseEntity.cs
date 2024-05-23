using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace DB.Entities
{
    public abstract class BaseEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Type type = GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                sb.Append($"{property.Name}: {property.GetValue(this)}, ");
            }
            return sb.ToString().TrimEnd(',', ' ');
        }
    }
}

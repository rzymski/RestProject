using DB.Dto.HATEOAS;
using System.Reflection;
using System.Text;

namespace DB.Dto.Base
{
    public abstract class BaseDto
    {
        public List<Link> Links { get; set; } = new List<Link>();
        public BaseDto() { }

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

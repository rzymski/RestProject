namespace DB.Dto.HATEOAS
{
    public class LinkResourceBase
    {
        public LinkResourceBase()
        {

        }

        public List<Link> Links { get; set; } = new List<Link>();
    }
}

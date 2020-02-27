using System.Text.Json;

namespace API.Dto.GraphQL
{
    public class GraphQLQueryDto
    {
        public string Query { get; set; }
        public JsonElement? Variables { get; set; }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using API.Dto.GraphQL;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.GraphQL
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/graphql")]
    public class GraphQLController : ControllerBase
    {
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;
        public GraphQLController(ISchema schema, IDocumentExecuter documentExecuter)
        {
            _schema = schema;
            _documentExecuter = documentExecuter;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQueryDto query)
        {
            var result = await _documentExecuter.ExecuteAsync(_ =>
            {
                _.Schema = _schema;
                _.Query = query.Query;
                _.ExposeExceptions = true;
                _.Inputs = query.Variables?.ToString().ToInputs();
            }).ConfigureAwait(false);
            var t = result.Data;
            if (result.Errors?.Count > 0)
            {
                return Problem(statusCode: Int32.Parse(result.Errors.Select(_ => _.Message).FirstOrDefault()));
            }
            return Ok(result.Data);
        }
    }
}
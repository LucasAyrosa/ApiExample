using GraphQL;
using GraphQL.Types;

namespace API.GraphQL
{
    public class MySchema : Schema, ISchema
    {
        public MySchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<Query>();
            Mutation = resolver.Resolve<Mutation>();
        }
    }
}
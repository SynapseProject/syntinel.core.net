using System;

namespace Syntinel.Core
{
    public interface IResolver
    {
        public Status ProcessRequest(ResolverRequest request);
    }
}

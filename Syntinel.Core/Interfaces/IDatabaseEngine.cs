using System;
namespace Syntinel.Core
{
    public interface IDatabaseEngine
    {
        T Get<T>(string id);
    }
}

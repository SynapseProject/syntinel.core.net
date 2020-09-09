using System;
using System.Collections.Generic;

namespace Syntinel.Core.Resolvers
{
    public class Utilities
    {
        public static Status Echo(ResolverRequest request)
        {
            Status status = new Status();
            Dictionary<object, object> data = new Dictionary<object, object>();
            foreach (CueVariable variable in request.Variables)
                data[variable.Name] = variable.Values;

            status.Id = request.Id;
            status.ActionId = request.ActionId;
            status.Data = data;
            status.NewStatus = Core.StatusType.Completed;
            status.CloseSignal = true;

            return status;
        }
    }
}

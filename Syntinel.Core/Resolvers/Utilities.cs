using System;
using System.Collections.Generic;
using System.Text;

namespace Syntinel.Core.Resolvers
{
    public class Utilities
    {
        public static Status Echo(ResolverRequest request)
        {
            Status status = new Status();
            Dictionary<object, object> data = new Dictionary<object, object>();
            StringBuilder sb = new StringBuilder();
            foreach (CueVariable variable in request.Variables)
            {
                data[variable.Name] = variable.Values;
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append($"{variable.Name} : {String.Join(',', variable.Values)}");
            }

            status.Id = request.Id;
            status.ActionId = request.ActionId;
            status.Message = sb.ToString();
            status.Data = data;
            status.NewStatus = Core.StatusType.Completed;
            status.CloseSignal = false;

            return status;
        }
    }
}

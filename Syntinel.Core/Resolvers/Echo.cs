using System;
using System.Collections.Generic;
using System.Text;

using Syntinel.Core;

public class Echo : IResolver
{
    public Status ProcessRequest(ResolverRequest request)
    {
        Status status = new Status();
        Dictionary<object, object> data = new Dictionary<object, object>();
        StringBuilder sb = new StringBuilder();

        ActionType action = request.Actions[request.ActionId];
        foreach (MultiValueVariable variable in action.Variables)
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
        status.NewStatus = StatusType.Completed;
        status.CloseSignal = false;

        return status;
    }
}

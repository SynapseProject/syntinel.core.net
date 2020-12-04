using System;
using System.Collections.Generic;
using System.Text;

using Syntinel.Core;

public class Echo : IResolver
{
    public Status ProcessRequest(ResolverRequest request)
    {
        Status status = new Status();
        status.Id = request.Id;
        status.ActionId = request.ActionId;

        Dictionary<object, object> data = new Dictionary<object, object>();
        StringBuilder sb = new StringBuilder();

        try
        {
            ActionType action = request.Actions[request.ActionId];
            foreach (MultiValueVariable variable in action.Variables)
            {
                data[variable.Name] = variable.Values;
                if (sb.Length > 0)
                    sb.AppendLine();
                sb.Append($"{variable.Name} = {String.Join(',', variable.Values)}");
            }

            status.Message = sb.ToString();
            status.Data = data;
            status.NewStatus = StatusType.Completed;
            status.CloseSignal = false;
        }
        catch (Exception e)
        {
            status.NewStatus = StatusType.Error;
            status.Message = e.Message;
        }

        return status;
    }
}

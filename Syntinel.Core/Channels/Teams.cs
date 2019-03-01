using System;
using System.Net;
using System.Text;
using System.IO;

namespace Syntinel.Core
{
    public class Teams
    {
        public static TeamsMessage Publish(string id, ChannelDbType channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return Publish(request);
        }

        public static TeamsMessage Publish(ChannelRequest request)
        {
            TeamsMessage message = CreateTeamsMessage(request);
            Signal signal = request.Signal;


            return message;
        }

        public static TeamsMessage CreateTeamsMessage(ChannelRequest request)
        {
            TeamsMessage message = new TeamsMessage();
            message.Body.Add(new TeamsBody());
            Signal signal = request.Signal;

            TeamsBodyItems header = new TeamsBodyItems();
            header.Weight = "Bolder";
            header.Type = "TextBlock";
            header.Size = "Medium";
            header.Text = signal.Name;
            message.Body[0].Items.Add(header);

            TeamsBodyItems description = new TeamsBodyItems();
            description.Text = signal.Description;
            description.Type = "TextBlock";
            description.Size = "Medium";
            message.Body[0].Items.Add(description);

            int totalCues = signal.Cues.Keys.Count;
            foreach (string cue in signal.Cues.Keys)
            {
                TeamsCard card = CreateTeamsCard(request.Id, cue, signal.Cues[cue]);
                if (totalCues == 1)
                {
                    // Create Single Cue in Main Body of Message
                    foreach (TeamsBody body in card.Body)
                        message.Body.Add(body);

                    foreach (TeamsAction action in card.Actions)
                        message.Actions.Add(action);
                }
                else
                {
                    // Create Each Cue as an Action.ShowCard Button
                    TeamsAction action = new TeamsAction();
                    action.Type = "Action.ShowCard";
                    action.Title = cue;
                    action.Card = card;
                    message.Actions.Add(action);
                }
            }

            return message;
        }

        public static TeamsCard CreateTeamsCard(string signalId, string cueId, CueOption cue)
        {
            TeamsCard card = new TeamsCard();
            card.Body.Add(new TeamsBody());
            card.Body[0].Separator = true;

            TeamsBodyItems header = new TeamsBodyItems();
            header.Weight = "Bolder";
            header.Type = "TextBlock";
            header.Size = "Medium";
            header.Text = cue.Name;
            header.Wrap = true;
            card.Body[0].Items.Add(header);

            TeamsBodyItems description = new TeamsBodyItems();
            description.Text = cue.Description;
            description.Type = "TextBlock";
            description.Size = "Medium";
            description.Wrap = true;
            card.Body[0].Items.Add(description);

            foreach (SignalVariable action in cue.Actions)
            {
                TeamsAction myAction = new TeamsAction();
                if (action.Type == VariableType.button)
                {
                    myAction.Type = "Action.Submit";
                    myAction.Id = "action";
                    myAction.Title = action.Name;
                    myAction.Data = new TeamsActionData();
                    myAction.Data.CallbackId = $"{signalId}|{cueId}";
                    myAction.Data.Action = action.DefaultValue;

                    card.Actions.Add(myAction);
                }
                else if (action.Type == VariableType.choice)
                {
                    myAction.Type = "Action.ShowCard";
                    myAction.Title = action.Name;

                    myAction.Card = new TeamsCard();
                    myAction.Card.Type = "AdaptiveCard";

                    TeamsAction choiceAction = new TeamsAction();
                    choiceAction.Type = "Action.Submit";
                    choiceAction.Title = "Submit";
                    choiceAction.Data = new TeamsActionData();
                    choiceAction.Data.CallbackId = $"{signalId}|{cueId}";
                    myAction.Card.Actions.Add(choiceAction);

                    TeamsBody myBody = new TeamsBody();
                    myBody.Type = "Container";
                    myBody.Separator = true;
                    myBody.Items = new System.Collections.Generic.List<TeamsBodyItems>();

                    TeamsBodyItems choiceItems = new TeamsBodyItems();
                    choiceItems.Type = "Input.ChoiceSet";
                    choiceItems.Id = "action";

                    foreach (string key in action.Values.Keys)
                    {
                        TeamsBodyChoice teamsChoice = new TeamsBodyChoice();
                        teamsChoice.Title = action.Values[key];
                        teamsChoice.Value = key;
                        choiceItems.Choices.Add(teamsChoice);
                    }

                    myBody.Items.Add(choiceItems);
                    myAction.Card.Body.Add(myBody);
                    card.Actions.Add(myAction);
                }
            }

            return card;
        }

    }
}

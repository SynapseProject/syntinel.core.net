using System;
using System.Net;
using System.Text;
using System.IO;
using System.Web;
using System.Collections.Generic;


namespace Syntinel.Core
{
    public class Teams
    {
        public static AdaptiveCard Publish(string id, ChannelDbRecord channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return Publish(request);
        }

        public static AdaptiveCard Publish(ChannelRequest request)
        {
            AdaptiveCard message = CreateAdaptiveCardMessage(request);
            Signal signal = request.Signal;


            return message;
        }

        public static Cue CreateCue(Dictionary<string, object> reply)
        {
            Cue cue = new Cue();

            string callbackId = (string)reply["callback_id"];
            string signalId = callbackId.Split('|')[0];
            string cueId = HttpUtility.HtmlDecode(callbackId.Split('|')[1]);

            cue.Id = signalId;
            cue.CueId = cueId;

            foreach (string key in reply.Keys)
            {
                if (key != "callback_id")
                {
                    CueVariable variable = new CueVariable();
                    variable.Name = key;
                    // TOOD: Might have to split value by commans for multi-value field types.
                    variable.Values.Add((string)reply[key]);

                    cue.Variables.Add(variable);
                }
            }

            return cue;
        }

        public static AdaptiveCard CreateAdaptiveCardMessage(ChannelRequest request)
        {
            AdaptiveCard message = new AdaptiveCard();
            message.Body.Add(new AdaptiveCardBody());
            Signal signal = request.Signal;

            AdaptiveCardBodyItems header = new AdaptiveCardBodyItems();
            header.Weight = "Bolder";
            header.Type = "TextBlock";
            header.Size = "Medium";
            header.Text = signal.Name;
            message.Body[0].Items.Add(header);

            AdaptiveCardBodyItems description = new AdaptiveCardBodyItems();
            description.Text = signal.Description;
            description.Type = "TextBlock";
            description.Size = "Medium";
            message.Body[0].Items.Add(description);

            int totalCues = signal.Cues.Keys.Count;
            foreach (string cue in signal.Cues.Keys)
            {
                AdaptiveCard card = CreateAdaptiveCard(request.Id, cue, signal.Cues[cue]);
                if (totalCues == 1)
                {
                    // Create Single Cue in Main Body of Message
                    foreach (AdaptiveCardBody body in card.Body)
                        message.Body.Add(body);

                    foreach (AdaptiveCardAction action in card.Actions)
                        message.Actions.Add(action);
                }
                else
                {
                    // Create Each Cue as an Action.ShowCard Button
                    AdaptiveCardAction action = new AdaptiveCardAction();
                    action.Type = "Action.ShowCard";
                    action.Title = cue;
                    action.Card = card;
                    message.Actions.Add(action);
                }
            }

            return message;
        }

        public static AdaptiveCard CreateAdaptiveCard(string signalId, string cueId, CueOption cue)
        {
            AdaptiveCard card = new AdaptiveCard();
            card.Body.Add(new AdaptiveCardBody());
            card.Body[0].Separator = true;

            AdaptiveCardBodyItems header = new AdaptiveCardBodyItems();
            header.Weight = "Bolder";
            header.Type = "TextBlock";
            header.Size = "Medium";
            header.Text = cue.Name;
            header.Wrap = true;
            card.Body[0].Items.Add(header);

            AdaptiveCardBodyItems description = new AdaptiveCardBodyItems();
            description.Text = cue.Description;
            description.Type = "TextBlock";
            description.Size = "Medium";
            description.Wrap = true;
            card.Body[0].Items.Add(description);

            foreach (SignalVariable action in cue.Actions)
            {
                AdaptiveCardAction myAction = new AdaptiveCardAction();
                if (action.Type == VariableType.button)
                {
                    myAction.Type = "Action.Submit";
                    myAction.Id = "action";
                    myAction.Title = action.Name;
                    myAction.Data = new AdaptiveCardActionData();
                    myAction.Data.CallbackId = $"{signalId}|{cueId}";
                    myAction.Data.Action = action.DefaultValue;

                    card.Actions.Add(myAction);
                }
                else if (action.Type == VariableType.choice)
                {
                    myAction.Type = "Action.ShowCard";
                    myAction.Title = action.Name;

                    myAction.Card = new AdaptiveCard();
                    myAction.Card.Type = "AdaptiveCard";

                    AdaptiveCardAction choiceAction = new AdaptiveCardAction();
                    choiceAction.Type = "Action.Submit";
                    choiceAction.Title = "Submit";
                    choiceAction.Data = new AdaptiveCardActionData();
                    choiceAction.Data.CallbackId = $"{signalId}|{cueId}";
                    myAction.Card.Actions.Add(choiceAction);

                    AdaptiveCardBody myBody = new AdaptiveCardBody();
                    myBody.Type = "Container";
                    myBody.Separator = true;
                    myBody.Items = new System.Collections.Generic.List<AdaptiveCardBodyItems>();

                    AdaptiveCardBodyItems choiceItems = new AdaptiveCardBodyItems();
                    choiceItems.Type = "Input.ChoiceSet";
                    choiceItems.Id = "action";

                    foreach (string key in action.Values.Keys)
                    {
                        AdaptiveCardBodyChoice teamsChoice = new AdaptiveCardBodyChoice();
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

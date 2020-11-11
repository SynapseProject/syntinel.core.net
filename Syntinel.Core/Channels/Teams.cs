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
        public static MessageCard Publish(string id, ChannelDbRecord channel, Signal signal)
        {
            ChannelRequest request = new ChannelRequest
            {
                Id = id,
                Channel = channel,
                Signal = signal
            };

            return Publish(request);
        }

        public static MessageCard Publish(ChannelRequest request)
        {
            MessageCard message = CreateMessageCardMessage(request);
            string json = JsonTools.Serialize(message, true);
            Signal signal = request.Signal;

            String webHook = request?.Channel?.Target;
            if (webHook != null)
            {
                SendMessage(webHook, message);
                return message;
            }
            else
                throw new Exception("No Target Information Was Provided.");
        }

        public static void SendMessage(string webHook, MessageCard message)
        {
            string body = JsonTools.Serialize(message);
            Utils.PostMessage(webHook, body);
        }

        public static Cue CreateCue(Dictionary<string, object> reply)
        {
            Cue cue = new Cue();

            string signalId = (string)reply["signalId"];
            string cueId = (string)reply["cueId"];

            cue.Id = signalId;
            cue.CueId = cueId;

            foreach (string key in reply.Keys)
            {
                if (key != "signalId" && key != "cueId")
                {
                    MultiValueVariable variable = new MultiValueVariable();
                    variable.Name = key;
                    // TODO: Might have to split value by semicolons for multi-value field types.
                    variable.Values.Add((string)reply[key]);
                    cue.Variables.Add(variable);
                }
            }

            return cue;
        }

        public static Cue CreateCueOld(Dictionary<string, object> reply)
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
                    MultiValueVariable variable = new MultiValueVariable();
                    variable.Name = key;
                    // TOOD: Might have to split value by commans for multi-value field types.
                    variable.Values.Add((string)reply[key]);

                    cue.Variables.Add(variable);
                }
            }

            return cue;
        }

        // ******************************************************************
        // *** MessageCard Methods
        // ******************************************************************

        public static MessageCard CreateMessageCardMessage(ChannelRequest request)
        {
            MessageCard message = new MessageCard();
            Signal signal = request.Signal;

            message.Title = String.IsNullOrEmpty(signal.Name) ? " " : signal.Name;
            message.Text = String.IsNullOrEmpty(signal.Description) ? " " : signal.Description;
            if (signal.IncludeId)
            {
                if (String.IsNullOrWhiteSpace(message.Title))
                    message.Title = $"Id: {request.Id}";
                else
                    message.Title += $" (Id: {request.Id})";
            }

            if (signal.Cues != null)
            {
                int totalCues = signal.Cues.Count;
                foreach (string key in signal.Cues.Keys)
                {
                    CueOption cue = signal.Cues[key];
                    MessageCardSection section = new MessageCardSection
                    {
                        Title = cue.Name,
                        Text = cue.Description
                    };
                    message.Sections.Add(section);

                    foreach (SignalVariable cueAction in cue.Actions)
                    {
                        MessageCardAction action = CreateMessageCardAction(request, key, cueAction);
                        if (action != null)
                            message.PotentialActions.Add(action);
                    }
                }
            }

            return message;
        }

        public static MessageCardAction CreateMessageCardAction(ChannelRequest request, string cueId, SignalVariable action)
        {
            string actionUrl = request.Channel?.Config?["actionUrl"]?.ToString();

            MessageCardAction potnetialAction = new MessageCardAction();
            potnetialAction.Name = action.Text;

            Dictionary<string, object> actionBody = new Dictionary<string, object>();
            actionBody.Add("signalId", request.Id);
            actionBody.Add("cueId", cueId);

            if (action.Type == VariableType.choice)
            {
                potnetialAction.Type = MessageCardActionType.ActionCard;
                potnetialAction.Inputs = new List<MessageCardInput>();
                potnetialAction.Actions = new List<MessageCardAction>();

                MessageCardInput input = new MessageCardInput();
                input.Type = MessageCardInputType.MultichoiceInput;
                input.Id = action.Id;
                input.Title = action.Text;
                input.Value = action.DefaultValue;
                input.Style = MessageCardInputStyle.expanded;       // Expanded = Radio Buttons.  Remove for Drop Down"
                input.IsMultiSelect = false;
                input.Choices = new List<MessageCardInputChoice>();
                foreach (string key in action.Values.Keys)
                {
                    MessageCardInputChoice messageChoice = new MessageCardInputChoice()
                    {
                        Name = action.Values[key],
                        Value = key
                    };
                    input.Choices.Add(messageChoice);
                }

                potnetialAction.Inputs.Add(input);

                actionBody.Add(action.Id, "{{action.value}}");
                MessageCardAction submit = new MessageCardAction()
                {
                    Type = MessageCardActionType.HttpPOST,
                    Name = "Submit",
                    Target = actionUrl,
                    Body = JsonTools.Serialize(actionBody)
                };
                potnetialAction.Actions.Add(submit);
            }
            else if (action.Type == VariableType.button)
            {
                actionBody.Add(action.Id, action.DefaultValue);

                potnetialAction.Type = MessageCardActionType.HttpPOST;
                potnetialAction.Target = actionUrl;
                potnetialAction.Body = JsonTools.Serialize(actionBody);
            }
            else
                // Unknown or Unsupported Action Type.  Ignore It.
                potnetialAction = null;

            return potnetialAction;
        }

        // ******************************************************************
        // *** Adaptive Card Methods (Not Supported By Teams As Of 4-JUN-2020)
        // ******************************************************************

        public static AdaptiveCard CreateAdaptiveCardMessage(ChannelRequest request)
        {
            AdaptiveCard message = new AdaptiveCard();
            message.Body.Add(new AdaptiveCardBody());
            Signal signal = request.Signal;

            if (!String.IsNullOrEmpty(signal.Name))
            {
                AdaptiveCardBodyItems header = new AdaptiveCardBodyItems();
                header.Weight = "Bolder";
                header.Type = "TextBlock";
                header.Size = "Medium";
                header.Text = signal.Name;
                message.Body[0].Items.Add(header);
            }

            if (!String.IsNullOrEmpty(signal.Description))
            {
                AdaptiveCardBodyItems description = new AdaptiveCardBodyItems();
                description.Text = signal.Description;
                description.Type = "TextBlock";
                description.Size = "Medium";
                message.Body[0].Items.Add(description);
            }

            if (signal.Cues != null)
            {
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
                    myAction.Id = action.Id;
                    myAction.Title = action.Text;
                    myAction.Data = new AdaptiveCardActionData();
                    myAction.Data.CallbackId = $"{signalId}|{cueId}";
                    myAction.Data.Action = action.DefaultValue;

                    card.Actions.Add(myAction);
                }
                else if (action.Type == VariableType.choice)
                {
                    myAction.Type = "Action.ShowCard";
                    myAction.Title = action.Text;

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
                    choiceItems.Id = action.Id;

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

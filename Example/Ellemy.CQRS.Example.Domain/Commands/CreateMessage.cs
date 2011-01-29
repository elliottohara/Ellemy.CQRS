using System;
using Ellemy.CQRS.Command;

namespace Ellemy.CQRS.Example.Commands
{
    public class CreateMessage : ICommand
    {
        public Guid MessageId { get; set; }
        public string Text { get; set; }

        public CreateMessage(Guid messageId, string text)
        {
            MessageId = messageId;
            Text = text;
        }
    }
}
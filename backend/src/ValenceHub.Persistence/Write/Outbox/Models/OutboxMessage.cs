using System;
using System.Collections.Generic;
using System.Text;

namespace ValenceHub.Persistence.Write.Outbox.Models
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; init; } = Guid.NewGuid();

        public string Type { get; init; } = default!;
        public string Payload { get; init; } = default!;
        public int Version { get; init; }

        public DateTimeOffset OccurredOnUtc { get; init; }

        public DateTimeOffset? ProcessedOnUtc { get; private set; }
        public string? Error { get; private set; }

        private OutboxMessage() { } 

        public OutboxMessage(
            string type,
            string payload,
            int version,
            DateTimeOffset occurredOnUtc)
        {
            Type = type;
            Payload = payload;
            Version = version;
            OccurredOnUtc = occurredOnUtc;
        }

        public void MarkProcessed(DateTimeOffset now)
        {
            ProcessedOnUtc = now;
            Error = "Success"; 
        }


        public void MarkFailed(string error)
            => Error = error;
    }

}

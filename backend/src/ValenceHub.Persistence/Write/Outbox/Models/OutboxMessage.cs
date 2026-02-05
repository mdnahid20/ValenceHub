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

        public DateTime OccurredOnUtc { get; init; }

        public DateTime? ProcessedOnUtc { get; private set; }
        public string? Error { get; private set; }

        private OutboxMessage() { } 

        public OutboxMessage(
            string type,
            string payload,
            int version,
            DateTime occurredOnUtc)
        {
            Type = type;
            Payload = payload;
            Version = version;
            OccurredOnUtc = occurredOnUtc;
        }

        public void MarkProcessed()
            => ProcessedOnUtc = DateTime.UtcNow;

        public void MarkFailed(string error)
            => Error = error;
    }

}

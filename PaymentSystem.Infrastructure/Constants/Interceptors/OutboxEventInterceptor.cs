using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PaymentSystem.Domain.Entities;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Infrastructure.Constants.Interceptors
{
    public class OutboxEventInterceptor : SaveChangesInterceptor
    {
        private readonly Dictionary<object, EntityState> _entityStates = new();

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            CaptureEntityStates(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            CaptureEntityStates(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            GenerateOutboxEvents(eventData.Context);
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
        {
            GenerateOutboxEvents(eventData.Context);
            return base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void CaptureEntityStates(DbContext? context)
        {
            if (context == null) return;
            _entityStates.Clear();

            var entries = context.ChangeTracker.Entries()
                .Where(e => (e.Entity is BaseEntity || e.Entity is IEntity) &&
                            e.Entity is not OutboxEvent &&
                            (e.State == EntityState.Added ||
                             e.State == EntityState.Modified ||
                             e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
                _entityStates[entry.Entity] = entry.State;
        }

        private void GenerateOutboxEvents(DbContext? context)
        {
            if (context == null) return;

            foreach (var kvp in _entityStates)
            {
                var eventType = kvp.Value == EntityState.Added ? "Added" :
                                kvp.Value == EntityState.Modified ? "Modified" : "Deleted";

                var payload = JsonSerializer.Serialize(kvp.Key, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                context.Database.ExecuteSqlRaw(
                    "INSERT INTO OutboxEvents (EntityType, EventType, Payload, IsProcessed, IsActive, IsDeleted, CreatedDate, ProcessedDate) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})",
                    kvp.Key.GetType().Name,
                    eventType,
                    payload,
                    false,
                    true,
                    false,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                );
            }
            _entityStates.Clear();
        }
    }
}

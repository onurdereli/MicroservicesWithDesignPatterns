﻿using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.API.EventStores;
using EventSourcing.API.Models;
using EventSourcing.Shared;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSourcing.API.BackgroundServices
{
    public class ProductReadModelEventStore : BackgroundService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadModelEventStore> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ProductReadModelEventStore(IEventStoreConnection eventStoreConnection, IServiceProvider serviceProvider, ILogger<ProductReadModelEventStore> logger)
        {
            _eventStoreConnection = eventStoreConnection;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppeared, autoAck: false);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)}, EventSourcing.Shared");

            _logger.LogInformation($"The Message processing... : {type.ToString()}");
            var eventData = Encoding.UTF8.GetString(arg2.Event.Data);

            var @event = JsonSerializer.Deserialize(eventData, type);

            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Product product = null;
            
            switch (@event)
            {
                case ProductCreatedEvent productCreatedEvent:
                    product = new()
                    {
                        Id = productCreatedEvent.Id,
                        Name = productCreatedEvent.Name,
                        Price = productCreatedEvent.Price,
                        Stock = productCreatedEvent.Stock,
                        UserId = productCreatedEvent.UserId
                    };

                    context.Products.Add(product);
                    break;
                case ProductDeletedEvent productDeletedEvent:
                    product = await context.Products.FindAsync(productDeletedEvent.Id);
                    if (product != null)
                    {
                        context.Products.Remove(product);
                    }
                    break;
                case ProductNameChangedEvent productNameChangedEvent:
                    product = await context.Products.FindAsync(productNameChangedEvent.Id);
                    if (product != null)
                    {
                        product.Name = productNameChangedEvent.ChangedName;
                    }
                    break;
                case ProductPriceChangedEvent productPriceChangedEvent:
                    product = await context.Products.FindAsync(productPriceChangedEvent.Id);
                    if (product != null)
                    {
                        product.Price = productPriceChangedEvent.ChangedPrice;
                    }
                    break;
            }

            await context.SaveChangesAsync();
             
            arg1.Acknowledge(arg2.Event.EventId);
            //Manuel olarak bütün işlemler bitince bu eventin Persistent Subscriptions'den silinmesini sağlar
        }
    }
}
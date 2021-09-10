using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SagaStateMachineWorkerService.Models;
using Shared;

namespace SagaStateMachineWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {

                    services.AddMassTransit(cfg =>
                    {
                        cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
                        {
                            opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
                            {
                                builder.UseSqlServer(hostContext.Configuration.GetConnectionString("SqlServer"), m =>
                                {
                                    m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                });
                            });
                        });

                        cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configurator =>
                        {
                            configurator.Host(hostContext.Configuration["RabbitMQUrl"], "/", host =>
                            {
                                host.Username("guest");
                                host.Password("guest");
                            });

                            //Belirtilen kuyruğa her bi mesaj geldiğinden OrderStateInstance ile bi nesne örneği oluşacak ve tabloya yazılacak.
                            configurator.ReceiveEndpoint(RabbitMqSettingsConst.OrderSaga, endpointConfigurator =>
                            {
                                endpointConfigurator.ConfigureSaga<OrderStateInstance>(provider);
                            });
                        }));
                    });

                    services.AddMassTransitHostedService();
                    services.AddHostedService<Worker>();
                });
    }
}

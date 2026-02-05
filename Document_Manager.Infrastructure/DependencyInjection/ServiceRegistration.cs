using Document_Manager.Application.Interface;
using Document_Manager.Application.Service;
using Document_Manager.Domain.Interface;
using Document_Manager.Infrastructure.Persistence;
using Document_Manager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Document_Manager.Infrastructure.DependencyInjection
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
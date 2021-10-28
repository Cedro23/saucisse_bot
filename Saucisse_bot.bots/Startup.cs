﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saucisse_bot.Core.Services.Items;
using Saucisse_bot.Core.Services.Profiles;
using Saucisse_bot.DAL;

namespace Saucisse_bot.Bots
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RPGContext>(options => 
            {
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=RPGContext;Trusted_Connection=True;MultipleActiveResultSets=true",
                    x => x.MigrationsAssembly("Saucisse_bot.DAL.Migrations"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IExperienceService, ExperienceService>();

            var serviceProvider = services.BuildServiceProvider();

            var bot = new Bot(serviceProvider);

            services.AddSingleton(bot);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}

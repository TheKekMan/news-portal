﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NewsPortalWebApi.Business_Logic;
using NewsPortalWebApi.Data_Access.EFCore;
using NewsPortalWebApi.Data_Access.EFCore.Repositories;
using NewsPortalWebApi.Data_Access.Interfaces;
using NewsPortalWebApi.Business_Logic.Inerfaces;
using NewsPortalWebApi.Business_Logic.DTO;
using NewsPortalWebApi.Business_Logic.Interfaces;
using NewsPortalWebApi.Business_Logic.Services;
using NewsPortalWebApi.Data_Access.Models;
using NewsPortalWebApi.Extensions;

namespace NewsPortalWebApi
{
    /// <summary>
    /// Класс запуска для веб хоста
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Конструктор класса запуска
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Этот метод вызывается во время выполнения. Используйте этот метод для добавления сервисов в контейнер.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql().AddDbContext<NewsPortalWebApiContext>(optionsAction: opt =>
                opt.UseNpgsql(Configuration.GetConnectionString(name: "NewsPortalWebApiContext")));

            services.AddDbContext<NewsPortalWebApiContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("NewsPortalWebApiContext")));

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IRepository<News>, NewsRepository>();
            services.AddScoped<IRepository<Author>, AuthorsRepository>();

            services.AddScoped<IUnitOfWork, EFUnitOfWork>();

            services.AddScoped<INewsService, NewsServices>();
            services.AddScoped<IAuthorsService, AuthorsServices>();

            services.AddControllers();

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(name: "v1", new OpenApiInfo { Title = "News Web Api", Version = "v1"});
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

        }

        /// <summary>
        /// Этот метод вызывается во время выполнения. Используйте этот метод для настройки конвейера HTTP-запроса.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "News Web Api V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

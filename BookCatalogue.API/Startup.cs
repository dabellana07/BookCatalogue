using AutoMapper;
using BookCatalogue.API.Extensions;
using BookCatalogue.Contracts.Data.Repositories;
using BookCatalogue.Contracts.ElasticSearch.Services;
using BookCatalogue.Data;
using BookCatalogue.Data.Repositories;
using BookCatalogue.ElasticSearch.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookCatalogue.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("AppPolicy", options => options
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            services.AddControllers();

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<BookCatalogueContext>(options =>
                options.UseSqlServer(Configuration["BookCatalogue"]));

            services.AddScoped<IBookRepository, BookRepository>();

            services.AddElasticSearch(Configuration);
            services.AddScoped<IBookElasticService, BookElasticService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AppPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

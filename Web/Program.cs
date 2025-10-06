using Infrastructure;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<WebAppMapsContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<IRepository<Salle>, Repository<Salle>>();
            builder.Services.AddScoped<IRepository<SallePause>, Repository<SallePause>>();
            builder.Services.AddScoped<IRepository<SalleBubble>, Repository<SalleBubble>>();
            builder.Services.AddScoped<IRepository<SalleReunion>, Repository<SalleReunion>>();




            builder.Services.AddScoped<IRepository<Etage>, Repository<Etage>>();

            builder.Services.AddScoped<ISalleManager, SalleManager>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Etages}/{action=SearchSalle}/{id?}")
                .WithStaticAssets();

            app.Run();
        }

    }
}

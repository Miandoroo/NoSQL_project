using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using NoSQL_project.Repositories;
using NoSQL_project.Repositories.Interfaces;
using NoSQL_project.Services;
using NoSQL_project.Services.Interfaces;

namespace NoSQL_project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Carga las variables del archivo .env
            DotNetEnv.Env.TraversePath().Load();

            var builder = WebApplication.CreateBuilder(args);

            // ---------- MONGO CONFIGURATION ----------
            // 1) MongoClient como Singleton (recomendado por MongoDB)
            builder.Services.AddSingleton<IMongoClient>(sp =>
            {
                var conn = builder.Configuration["Mongo:ConnectionString"];
                if (string.IsNullOrWhiteSpace(conn))
                    throw new InvalidOperationException("Mongo:ConnectionString not configured (.env or appsettings.json)");
                return new MongoClient(conn);
            });

            // 2) IMongoDatabase como Scoped (una por request)
            builder.Services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var dbName = builder.Configuration["Mongo:Database"];
                if (string.IsNullOrWhiteSpace(dbName))
                    throw new InvalidOperationException("Mongo:Database not configured.");
                return client.GetDatabase(dbName);
            });

            // ---------- AUTHENTICATION & AUTHORIZATION ----------
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/Login";
                    options.LogoutPath = "/Auth/Logout";
                    options.AccessDeniedPath = "/Auth/Denied";
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                });

            builder.Services.AddAuthorization(o =>
            {
                o.AddPolicy("ServiceDeskOnly", p => p.RequireRole("ServiceDesk", "Admin"));
                o.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
            });

            // ---------- MVC ----------
            builder.Services.AddControllersWithViews();

            // ---------- REPOSITORIES ----------
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();

            // ---------- SERVICES ----------
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ITicketReportService, TicketReportService>();

            var app = builder.Build();

            // ---------- MIDDLEWARE PIPELINE ----------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // First autentication,and after autorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

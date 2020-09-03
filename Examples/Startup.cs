using Examples.Data;
using Examples.Data.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Examples {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            #region Adding Database Connection
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext) {
            initDatabase(dbContext);

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Error");

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private void initDatabase(ApplicationDbContext dbContext) {
            dbContext.Database.EnsureCreated();

            #region Clean up
            foreach (var phone in dbContext.Phones)
                dbContext.Phones.Remove(phone);
            dbContext.SaveChanges();
            #endregion

            #region Create sample Data
            for (int i = 1; i <= 10; i++) {
                dbContext.Phones.Add(new Phone() {
                    Name = $"iPhone {i}",
                    Manufacturer = "Apple"
                });
            }

            for (int i = 1; i <= 10; i++) {
                dbContext.Phones.Add(new Phone() {
                    Name = $"Galaxy S{i}",
                    Manufacturer = "Samsung"
                });
            }

            for (int i = 1; i <= 8; i++) {
                dbContext.Phones.Add(new Phone() {
                    Name = $"OnePlus {i}",
                    Manufacturer = "OnePlus"
                });
            }

            dbContext.SaveChanges();
            #endregion
        }
    }
}

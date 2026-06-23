using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<WeddingPlanner.Infrastructure.Services.BendService>();
builder.Services.AddScoped<WeddingPlanner.Infrastructure.Services.TipVjencanjaService>();
builder.Services.AddScoped<WeddingPlanner.Infrastructure.Services.ExcelService>();
builder.Services.AddScoped<WeddingPlanner.Infrastructure.Services.PdfService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DataSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
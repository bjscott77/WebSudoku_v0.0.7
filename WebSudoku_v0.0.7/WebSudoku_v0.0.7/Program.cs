using Microsoft.EntityFrameworkCore;
using WebSudoku_v0._0._7.Classes;
using WebSudoku_v0._0._7.Data;
using WebSudoku_v0._0._7.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("*")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Add application dbcontext as single instance, project wide to dependency injection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add custom services to the container
var devConfig = new DevConfiguration(builder.Configuration.GetSection("DEV"));
builder.Services.AddSingleton<DevConfiguration>(devConfig);
builder.Services.AddScoped<ISudokuBoard, SudokuBoard>();
builder.Services.AddScoped<ISudokuRepository,SudokuPuzzlesRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors();

    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();
} else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Run();

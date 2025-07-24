using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using GeneticProgramming.Playground.Services;
using GeneticProgramming.Playground.Data;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Radzen services
builder.Services.AddRadzenComponents();

// Add database services
builder.Services.AddSingleton<DatabaseConfigurationService>();

// Configure DbContext with a factory pattern to avoid circular dependency
builder.Services.AddDbContextFactory<ExperimentDbContext>((serviceProvider, options) =>
{
    var dbConfigService = serviceProvider.GetRequiredService<DatabaseConfigurationService>();
    var connectionString = $"Data Source={dbConfigService.CurrentDatabasePath}";
    options.UseSqlite(connectionString);
});

// Register DbContext as scoped using the factory
builder.Services.AddScoped<ExperimentDbContext>(serviceProvider =>
{
    var factory = serviceProvider.GetRequiredService<IDbContextFactory<ExperimentDbContext>>();
    return factory.CreateDbContext();
});

// Add our custom services
builder.Services.AddSingleton<DatabaseConfigurationService>();
builder.Services.AddScoped<GeneticProgrammingService>();
builder.Services.AddScoped<DatasetService>();
builder.Services.AddScoped<ExperimentRunner>();
builder.Services.AddScoped<ExperimentRepository>();
builder.WebHost.UseStaticWebAssets();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

app.Run();

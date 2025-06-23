using EventEase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder; // Added for WebApplicationBuilder, etc.
using Microsoft.Extensions.Configuration; // Added for GetConnectionString
using Microsoft.Extensions.Hosting; // Added for app.Environment.IsDevelopment()

// --- IMPORTANT: 'builder' must be declared BEFORE its first use ---
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Azure Blob Storage Client
var blobStorageConnectionString = builder.Configuration.GetConnectionString("AzureStorage");
if (string.IsNullOrEmpty(blobStorageConnectionString))
{
    // It's good practice to log or handle cases where the connection string might be missing
    Console.WriteLine("AzureStorage connection string is not configured. Blob storage functionality may be impacted.");
    // Optionally, you might throw an exception or use a placeholder service
    // throw new InvalidOperationException("AzureStorage connection string is missing.");
}
else
{
    builder.Services.AddSingleton(x => new BlobServiceClient(blobStorageConnectionString));
}

builder.Services.AddControllersWithViews();

// Configure EventEaseDbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables serving static files from wwwroot

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

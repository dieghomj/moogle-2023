using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MoogleEngine;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

Stopwatch stopwatch = new Stopwatch(); 

Console.WriteLine("Loading documents...");
stopwatch.Start();
Documents db = new Documents("../Content/");
stopwatch.Stop();

TimeSpan ts = stopwatch.Elapsed;
Console.WriteLine(Documents.Doc.Length + " documents were succesfully loaded in " + ts.Minutes + "m" + ts.Seconds + "s");

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

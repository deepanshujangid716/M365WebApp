
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args); 

// 2. The "Initialization": This line tells the app to look at 
// the "AzureAd" section in appsettings.json and set up the login logic.
//builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd");

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "User.Read" }) // Request the token
    .AddMicrosoftGraph(builder.Configuration.GetSection("MicrosoftGraph")) // Dependency Injection for Graph
    .AddInMemoryTokenCaches(); // Store tokens in RAM (volatile)


// Add services to the container.
builder.Services.AddControllersWithViews();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection(); // Redirects HTTP to HTTPS
app.UseStaticFiles();      // Serves your CSS/JS images

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();    // The "Bouncer" - Checks if a user is logged in

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();




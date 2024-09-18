using Microsoft.EntityFrameworkCore;
using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<TypicalTechToolsDbContext>(options => 
{ 
    options.UseSqlServer(connectionString);
});







//Input these when answering the question in the Technical report.
builder.Services.AddSession(options =>
{
    //Tells the session how long to go without a request form the session user before
    //it clears the session data.
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    //Session cookie is only accessible through the HTTP protocol and not through
    //client side scripting such as JavaScript.
    options.Cookie.HttpOnly = true;
    //Sets whether the cookie can be used outside the domain where it was created.
    options.Cookie.SameSite = SameSiteMode.Strict;
    //Set whether the cookie m,ust be sent via https(always) or via the 
    //same method it was sent(Http or Https)
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSingleton<CsvParser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Redirect HTTP requests to HTTPS (if configured)
app.UseHttpsRedirection();

// Serve static files like CSS, JavaScript, and images
app.UseStaticFiles();

//This must be place before the UseRouting();
app.UseSession();

// Route requests to the appropriate controller actions
app.UseRouting(); 

app.UseAuthentication(); 


app.UseAuthorization(); 

 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=HomeIndex}/{id?}");

app.Run(); 

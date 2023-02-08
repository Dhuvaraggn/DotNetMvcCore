using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SelfTraining.Data;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCognitoIdentity();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

//Custom Configuration for cognito
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
//options=>
//{
//    options.CookieManager = new ChunkingCookieManager();

//    options.Cookie.HttpOnly = true;
//    options.Cookie.SameSite = SameSiteMode.None;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//})
//.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
//{
//    options.Authority = "https://cognito-idp.us-east-1.amazonaws.com/us-east-1_TJ9PXFId1";
//    options.ClientSecret = "51pdgocr2f6nl012ndbl41d4kjjinktuav1767o9ifsrqqcsd2s";
//    options.ResponseType = Configuration["Authentication:Cognito:ResponseType"];
//    options.MetadataAddress = Configuration["Authentication:Cognito:MetadataAddress"];
//    options.ClientId = Configuration["Authentication:Cognito:ClientId"];
//    options.Scope.Add("openid");
//    options.Scope.Add("email");
//    options.Scope.Add("profile");
//    options.Scope.Add("aws.cognito.signin.user.admin");
//    options.ResponseType = "code";
//    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.TokenValidationParameters = new TokenValidationParameters()
//    {
//        NameClaimType = "cognito:user",
//        //ValidateIssuer = TokenValidationParameters.ValidateIssuer,
//        RoleClaimType = "cognito:groups"
//    };
//    options.Events = new OpenIdConnectEvents()
//    {
//        OnRedirectToIdentityProviderForSignOut = context =>
//        {
//            var logouturl = "https://dotnet-selftraining.auth.us-east-1.amazoncognito.com/logout?client_id=562crsfgnbnp3958ord6asf7q5";
//            logouturl += $"&logout_uri={context.Request.Scheme}://{context.Request.Host}/";
//            context.Response.Redirect(logouturl);
//            context.HandleResponse();
//            return Task.CompletedTask;
//        }
//    };
//});
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminGroup", policy => policy.RequireClaim("cognito:groups", "Admin"));
//});
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminRole", policy =>
//    policy.RequireAssertion(context =>
//    context.User.HasClaim(c => c.Type == "cognito:groups" && c.Value == "Admin")));
//});


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddRazorPages();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseCors();
app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
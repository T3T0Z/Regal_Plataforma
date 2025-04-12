using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Regal_Plataforma.Funciones;
using Regal_Plataforma.Models.BDD;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add context
builder.Services.AddDbContext<REGAL_ASISTENCIAContext>(
options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Coockies
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;

    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(5);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Login/Login/";
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

//Partial view a String
builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();

//Configure Sessions
builder.Services.AddDistributedMemoryCache();

// Registrar el servicio
builder.Services.AddScoped<IUsuarioServices, UsuarioServices>();
builder.Services.AddScoped<IOrderDatosServices, OrderDatosServices>();
builder.Services.AddScoped<IGremiosServices, GremioServices>();
builder.Services.AddScoped<INotasServices, NotasService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IObraService, ObraService>();
builder.Services.AddScoped<ITrabajosServices, TrabajosServices>();
builder.Services.AddScoped<IArchivoService, ArchivoService>();
builder.Services.AddHttpClient<IAPIServices, ApiService>();

var app = builder.Build();

// Manejo de errores global
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Redirige a la página de error
    app.UseHsts();
}
app.UseStatusCodePagesWithRedirects("/Home/Error?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

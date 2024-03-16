var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=WebApp}/{action=Index}");
app.Run();

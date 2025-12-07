var builder = WebApplication.CreateBuilder(args);

// Configuration YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Middleware pipeline
app.UseRouting();
app.MapReverseProxy();

app.Run();

using System.Reflection;
using HireAI;
using HireAI.Common.Endpoints;
using HireAI.Common.Extensions;
using Microsoft.OpenApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddAntiforgery();

//builder.Services.AddOpenApiWithAuth();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddRateLimitingInternal(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());


//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   

    //app.ApplyMigrations();
}

app.UseSecurityHeaders();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

//app.UseAntiforgery();

app.MapEndpoints();

app.MapControllers();


await app.RunAsync();



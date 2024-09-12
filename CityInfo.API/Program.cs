using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using CityInfo.API.Services;
using CityInfo.API;
using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Asp.Versioning.ApiExplorer;

//Logging providers store logs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/cityinfo.txt,rollingInterval:RollingInterval.Day")
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);  //used to build core application
//builder.Logging.ClearProviders(); // clears all logging providers that have been registered by default or previously added.
//builder.Logging.AddConsole();
builder.Host.UseSerilog();

// Add services to the container.
//builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = false;
}).AddXmlDataContractSerializerFormatters()
  .AddNewtonsoftJson();       // Advanced JSON support
  //.AddProtobufFormatters();     // Protobuf support

//compiler directive
#if DEBUG
builder.Services.AddTransient<IMailService,LocalMailService>();
builder.Services.AddTransient<IEmailNotificationService,LocalEmailNotificationService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
builder.Services.AddTransient<IEmailNotificationService, CloudEmailNotificationService>();
#endif



builder.Services.AddEndpointsApiExplorer();  //adds swagger/open api services which help in testing Api  endpoints

// Generates Swagger documentation 
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddSingleton<CitiesDataStore>();
builder.Services.AddDbContext<CityInfoContext>(DbContextOptions =>

    DbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"])
);
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddTransient<IMailService, LocalMailService>();

//builder.Services.AddTransient<IMailService, LocalMailService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]))
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeFromMumabai", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("city", "Mumbai");
    });
});
//format for representing error in http apis //customization add additional information to error responses
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional info example");
        //   ctx.ProblemDetails.Extensions.Add("server",Environment.MachineName);
    };
});

builder.Services.AddApiVersioning(setup =>
{
    setup.ReportApiVersions = true;
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(setup=>
{
    setup.SubstituteApiVersionInUrl = true;
});

var apiVerDesprovider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
builder.Services.AddSwaggerGen(setupAction =>
{

    foreach(var description in apiVerDesprovider.ApiVersionDescriptions)
    {

        setupAction.SwaggerDoc(
            $"{description.GroupName}",
            new()
            {
                Title = "City Info API",
                Version = description.ApiVersion.ToString(),
                Description ="Through this API You can Acess cities and their POIs"
            });
    }
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);


    setupAction.IncludeXmlComments(xmlCommentsFullPath);

    setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "CityInfoApiBearerAuth" }
            }, new List<string>() }
    });

});
//Builds the WebApplication from the WebApplicationBuilder
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  //generates swagger json
    app.UseSwaggerUI(setup =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach(var description in descriptions)
        {
            setup.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });  //provides swagger ui
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();   //redirect http requst to https for security purpose

app.UseRouting();   //routing middleware
app.UseAuthentication();
app.UseAuthorization(); //Adds authorization middleware

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

//app.MapControllers();

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync("Hello world");
//});

app.Run();
    
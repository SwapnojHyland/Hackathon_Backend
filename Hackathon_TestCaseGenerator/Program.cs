using Hackathon_TestCaseGenerator.Models;
using Hackathon_TestCaseGenerator.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Bind config
var jiraConfig = builder.Configuration.GetSection("Jira").Get<JiraConfig>();
var documentationPath = builder.Configuration["Documentation:Path"];
var openAiKey = builder.Configuration["OpenAI:ApiKey"];
// Register services
builder.Services.AddSingleton(jiraConfig);
//builder.Services.AddSingleton(new OllamaService("llama3"));
builder.Services.AddSingleton(new OpenAIService(openAiKey));
builder.Services.AddSingleton(new JiraService(jiraConfig));
builder.Services.AddSingleton(new XrayUploadService(jiraConfig));
builder.Services.AddSingleton(new DocumentationService(documentationPath));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:3000") // React dev server
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

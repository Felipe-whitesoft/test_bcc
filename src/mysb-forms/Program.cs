using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Azure.Cosmos;
using mysb_forms;
using mysb_forms.core;
using mysb_forms.core.Interfaces;
using HealthChecks.UI.Client;
using mysb_forms.core.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Settings
builder.Services.AddAppSettings(builder.Configuration);

//https clients
builder.Services.AddHttpClients(builder.Configuration);

// Add SingleTone Services
builder.Services.AddScoped<IDatabaseService, CosmosDbService>();

// Add services to the container.
builder.Services.AddScoped<ISettings, Settings>();
builder.Services.AddScoped<IFormService, OFPMService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

//appInsights
builder.Services.AddAppInsights();

//Caching
builder.Services.AddCaching(builder.Configuration);

//CORS
builder.Services.CreateCorsPolicies(builder.Configuration);

//HealthChecks
var dbConfig = builder.Configuration.GetSection("CosmosDb").Get<CosmosDBConfiguration>();
if (dbConfig != null) {
    // Create a DefaultAzureCredential
    var credential = new DefaultAzureCredential();
    builder.Services.AddSingleton(sp => new CosmosClient(dbConfig.Endpoint, credential));
}

var ofpmSettings = builder.Configuration.GetSection("Ofpm").Get<OfpmSetting>();
var formResource = $"{ofpmSettings.BaseUrl}/form/resources";

builder.Services
.AddHealthChecks()
.AddUrlGroup(new Uri(formResource), "external_ofpm_form")
// .AddCheck<ExternalHealthCheck>("external_ipass_api")
.AddAzureCosmosDB()
.AddApplicationInsightsPublisher();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "mysb-forms.xml"));
});

var app = builder.Build();

app.UseHealthChecks("/health", new HealthCheckOptions {
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

//API-Docs
app.UseSwagger();
app.UseReDoc(c => {
    c.DocumentTitle = "My Services Forms APIs Documentation";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RoutePrefix = "docs";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseCors("AllowGet");

app.Run();

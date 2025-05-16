using Mapster;
using Serilog;
using SurveyBasket.Extentions;
using SurveyBasket.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    //configuration.ReadFrom.Configuration(context.Configuration)
    configuration
    .MinimumLevel.Information()
    .WriteTo.Console()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();


app.UseHttpsRedirection();
app.UseCors();
app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Mapster;
using SurveyBasket.Extentions;
using SurveyBasket.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors();
app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();

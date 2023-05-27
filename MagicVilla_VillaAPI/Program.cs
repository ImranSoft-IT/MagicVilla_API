

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// "option => option.ReturnHttpNotAcceptable = true" this option for not any data without json.
builder.Services.AddControllers(option => option.ReturnHttpNotAcceptable = true)
    .AddNewtonsoftJson().AddXmlDataContractSerializerFormatters(); // AddNewtonsoftJson() add for json supproted. Nuget Package is Microsoft.AspNetCore.Mvc.NewtonsoftJson.


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

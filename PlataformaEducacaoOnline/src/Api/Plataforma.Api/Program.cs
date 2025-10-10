var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // necessário p/ Swagger
builder.Services.AddSwaggerGen();           // UI do Swagger

var app = builder.Build();

// SEM redirecionar para HTTPS (fica só HTTP)
//// app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

// Força a escuta em IPv4 explícito e porta fixa
app.Run("http://127.0.0.1:5280");

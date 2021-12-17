using SlidingWindowCounters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(_ => new RateLimiter(TimeSpan.FromSeconds(1), 10));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.Use(async (context, next) =>
{
	var rateLimiter = context.RequestServices.GetService<RateLimiter>();
	var userId = context.Request.Headers["userId"];
	
	if (string.IsNullOrEmpty( userId))
		await next();
	
	var isNotRejected = await rateLimiter.VerifyAndAddRequest(userId, DateTime.UtcNow);
	if (!isNotRejected)
	{
		context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
		await context.Response.WriteAsync("Too Many Requests");
	}

	await next();
});

app.MapControllers();

app.Run();
var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.builder.Services.AddEndpointsApiExplorer();
builder.builder.Services.AddSwaggerGen();

// in Infrastructure DI
builder.Services.AddScoped<IHotelRepository, HotelRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// builder.Services
builder.Services.AddScoped<IPaymentService, StripePaymentService>(); // or stub in dev
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IPdfService, QuestPdfService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(HotelMappingProfile).Assembly);

// MediatR
builder.Services.AddMediatR(typeof(GetHotelDetailsQuery).Assembly);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.


app.Run();
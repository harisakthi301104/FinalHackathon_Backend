using FinalHackathon_Backend.Data;
using FinalHackathon_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using FinalHackathon_Backend.Config;

var builder = WebApplication.CreateBuilder(args);

// ========== READ CONFIGURATION ==========

// Bind JWT settings from appsettings.json
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

// ========== REGISTER SERVICES (ALL MODULES) ==========

// Add controllers
builder.Services.AddControllers();

// Add EF Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JWT settings as singleton
builder.Services.AddSingleton(jwtSettings);

// Register Auth module services (Rubesh)
builder.Services.AddScoped<AuthService>();

//Register Menu/Admin module services (Hariharan) — will be created by Hariharan
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ItemService>();

//Register Cart/Order module services (Nithish Khanna) — will be created by Nithish
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();

//Register Inventory/History module services (Sudharsan) — will be created by Sudharsan
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<OrderHistoryService>();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ========== CONFIGURE MIDDLEWARE PIPELINE ==========

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable HTTPS redirection
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAngular");

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the application
app.Run();


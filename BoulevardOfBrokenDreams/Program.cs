using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using BoulevardOfBrokenDreams.Hubs;
using BoulevardOfBrokenDreams.Interface;
using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;


//line 134

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:5173", "https://mumumsit158.com", "http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader().
               AllowCredentials();
    });
    //--------------------------------新增的 CORS 策略部分--------------------------------
    //options.AddPolicy("AllowAllLocalhost", builder =>
    // {
    //     builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
    //            .AllowAnyMethod()
    //            .AllowAnyHeader()
    //            .AllowCredentials();  // 允許攜帶身份驗證信息
    // });
    // --------------------------------------------------------------------------------
});

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddSingleton(builder.Configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new List<string>()
        }
    });
});


// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var configuration = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["Issuer"],
        ValidAudience = configuration["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Key"]!))
    };
});

builder.Services.AddControllers();

builder.Services.AddDbContext<MumuDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("Mumu")));

builder.Services.AddSignalR();
builder.Services.AddScoped<BoulevardOfBrokenDreams.Services.ServiceMessage>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

//S3client
//make sure that AWSSDK was installed or should nuget Install-Package AWSSDK.S3
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var accessKey = "2c7f57192821fa443ceef715554da6b5";
    var secretKey = "00dea3a1b7ee7a023ed94122dbe197520ee76dae73e1ff699976cc678db3cabb";
    var region = "auto";
    var serviceURL = "https://60fb16943028530a147d89e97a5d599f.r2.cloudflarestorage.com";

    var credentials = new BasicAWSCredentials(accessKey, secretKey);
    var config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region),
        ServiceURL = serviceURL,
    };
    return new AmazonS3Client(credentials, config);
});

var app = builder.Build();

app.UseSwagger();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//    Path.Combine(Directory.GetCurrentDirectory(), "images")),
//    RequestPath = "/resources"
//});

app.UseRouting();

// Enable authentication
app.UseAuthentication();
app.UseAuthorization();

// 使用新的 CORS 策略
app.UseCors();

app.MapControllers();

app.MapHub<ChatHub>("/ChatHub");
app.MapHub<CommentsHub>("/Comments");

app.Run();

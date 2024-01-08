using TemplateBlazorWasmHostedNet8.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<JwtBearerHubUtils>();

// Add services to the container.
#region Autorização e Autenticação
builder.Services
.AddAuthentication(item =>
    {
        item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(item =>
    {
        item.RequireHttpsMetadata = false;
        item.SaveToken = true;
        item.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("ConfigurationServiceOptions:ApiKeyJwt").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
        #region Funciona os eventos!
        item.Events = new JwtBearerEvents
        {
            OnMessageReceived = async context =>
            {
                try
                {
                    var jwtBearerHubUtils = context.HttpContext.RequestServices.GetRequiredService<JwtBearerHubUtils>();
                    await jwtBearerHubUtils.ValidateJwtTokenOnHubByPath(context, "/chat");
                }
                catch (Exception ex)
                {
                    context.Fail(ex);
                    // Salvar log do erro aqui ...
                }
            }
        };
        #endregion
    });

builder.Services.AddAuthorization();
////builder.Services.AddAuthorization(options =>
////{
////    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
////    options.AddPolicy("Cliente", policy => policy.RequireRole("Cliente"));
////});
#endregion

#region CORS
var urlClient = builder.Configuration.GetSection("ConfigurationServiceOptions:UrlClient").Value!;
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    builder.AllowAnyOrigin()
           //.WithOrigins(urlClient!)
           .AllowAnyMethod()
           .AllowAnyHeader());
});
#endregion

#region Feature Flags
builder.Services.AddFeatureManagement();
#endregion

#region Carregando dados do arquivo appsettings.json (Options Pattern)
builder.Services.Configure<ConfigurationServiceOptions>(builder.Configuration.GetSection(nameof(ConfigurationServiceOptions)));
#endregion

#region Services Business Logic
builder.Services.AddScoped<MessagePackService>();
builder.Services.AddScoped<TokenService>();
#endregion

#region SignalR - Etapa 1/2
// configurando para o tamanho da mensagem que trafaga entre client/server do SignalR ser infinito!
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = null;
    options.StreamBufferCapacity = null;
})
    .AddMessagePackProtocol(); // Add MessagePack para transferencia Binária de dados

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});
#endregion

builder.Services.AddAntiforgery();

var app = builder.Build();

#region Middleware - Global Exception (Help: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/handle-errors?view=aspnetcore-8.0)
//app.UseExceptionHandler(exceptionHandlerApp
//    => exceptionHandlerApp.Run(async context
//        => await Results.Problem()
//                     .ExecuteAsync(context)));

app.UseStatusCodePages(async statusCodeContext
=> await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
         .ExecuteAsync(statusCodeContext.HttpContext));
#endregion

app.UseHsts();

#region Acesso a pasta wwwroot e arquivos dentro da pasta
app.UseStaticFiles();
#endregion

app.UseAntiforgery();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

#region Mapeando Minimal APIs (Best Practics App)
app.MapAccountEndpoints();
app.MapDogEndpoints();
#endregion

#region Config Postgres Compatibility
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#endregion

#region NEW SignalR - Etapa 2/2
app.UseResponseCompression();
#endregion


app.MapHub<SeuHub>("/chat");

await app.RunAsync();

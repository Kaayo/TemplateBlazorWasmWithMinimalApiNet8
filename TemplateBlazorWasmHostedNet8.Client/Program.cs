using TemplateBlazorWasmHostedNet8.Client.HttpMessageHandlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<JwtTokenHeaderValidateHandler>();

#region Configura o Message Handler default Global para todas as requisições Http
////builder.Services.ConfigureHttpClientDefaults(config => config.AddHttpMessageHandler<JwtTokenHeaderValidateHandler>()); 
#endregion

builder.Services.AddHttpClient(EnumHttpClient.Api.ToString(), options =>
{
    options.BaseAddress = builder.HostEnvironment.IsDevelopment() ?
         new Uri("https://localhost:7136") :
         new Uri("https://api-blazor.appteste02.vps-kinghost.net");
}).AddHttpMessageHandler<JwtTokenHeaderValidateHandler>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage(config =>
        config.JsonSerializerOptions.WriteIndented = true
    );

builder.Services.AddScoped<NotificationsGenericHub>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<AccountService>());
builder.Services.AddScoped<ApiDogService>();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthorizationCore();
builder.Services.AddMudServices();
await builder.Build().RunAsync();

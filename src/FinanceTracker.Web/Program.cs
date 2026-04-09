using System.IO;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SelfRelianceFinanceTracker.Web.Components;
using SelfRelianceFinanceTracker.Web.Components.Account;
using SelfRelianceFinanceTracker.Web.Data;
using SelfRelianceFinanceTracker.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

if (builder.Environment.IsDevelopment())
{
    // Keep local logging on providers that work without elevated Windows permissions.
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();

    // Keep development keys inside the project so local runs do not depend on a broken user-profile key ring.
    var localKeysPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "keys");
    Directory.CreateDirectory(localKeysPath);

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(localKeysPath))
        .SetApplicationName("SelfRelianceFinanceTracker.Local");
}

var connectionString = ResolveSqliteConnectionString(builder.Configuration, builder.Environment);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ISavingsGoalService, SavingsGoalService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAlertService, AlertService>();

var app = builder.Build();

await EnsureDatabaseAndTestUserAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();

static string ResolveSqliteConnectionString(IConfiguration configuration, IWebHostEnvironment environment)
{
    var rawConnectionString = configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var connectionStringBuilder = new SqliteConnectionStringBuilder(rawConnectionString);
    if (string.IsNullOrWhiteSpace(connectionStringBuilder.DataSource) || connectionStringBuilder.DataSource == ":memory:")
    {
        return rawConnectionString;
    }

    var dataSource = connectionStringBuilder.DataSource;
    if (OperatingSystem.IsWindows() && dataSource.StartsWith("/tmp/", StringComparison.Ordinal))
    {
        dataSource = Path.Combine(Path.GetTempPath(), Path.GetFileName(dataSource));
    }
    else if (!Path.IsPathRooted(dataSource))
    {
        dataSource = Path.GetFullPath(Path.Combine(environment.ContentRootPath, dataSource));
    }

    // SQLite won't create missing directories for file-backed databases.
    var databaseDirectory = Path.GetDirectoryName(dataSource);
    if (!string.IsNullOrWhiteSpace(databaseDirectory))
    {
        Directory.CreateDirectory(databaseDirectory);
    }

    connectionStringBuilder.DataSource = dataSource;
    return connectionStringBuilder.ToString();
}

static async Task EnsureDatabaseAndTestUserAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();

    if (!app.Environment.IsDevelopment())
    {
        return;
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string testEmail = "testuser@finance.local";
    const string testPassword = "Test1234!";

    var existingUser = await userManager.FindByEmailAsync(testEmail);
    if (existingUser is not null)
    {
        return;
    }

    var user = new ApplicationUser
    {
        UserName = testEmail,
        Email = testEmail,
        EmailConfirmed = true,
        DisplayName = "Test User"
    };

    var result = await userManager.CreateAsync(user, testPassword);
    if (!result.Succeeded)
    {
        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to create development test user: {errors}");
    }
}

using SelfHostedGoogleFonts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();


// SelfHostedGoogleFonts: Register services and use file system storage
builder.Services.AddSelfHostedGoogleFonts()
    .AddFileSystemStorage();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
using Microsoft.EntityFrameworkCore;
using PocketKanbanAPI.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    for (int i = 0; i < 6; i++) 
    {
        try
        {
            var dbContext = services.GetRequiredService<AppDbContext>();
            Console.WriteLine($"🛠️ Veritabanı kontrol ediliyor... (Deneme {i + 1}/6)");
            dbContext.Database.Migrate(); 
            
            Console.WriteLine("🚀 Veritabanı başarıyla hazırlandı ve güncellendi!");
            break; 
        }
        catch (Exception ex)
        {

            if (i == 5) 
            {
                Console.WriteLine("❌ KRİTİK HATA: Veritabanı oluşturulamadı.");
                Console.WriteLine($"Hata Detayı: {ex.Message}");
                throw; 
            }
            
            Console.WriteLine($"⏳ SQL Server henüz hazır değil... 5 saniye bekleniyor...");
            System.Threading.Thread.Sleep(5000); 
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
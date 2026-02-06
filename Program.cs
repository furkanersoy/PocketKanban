using Microsoft.EntityFrameworkCore;
using PocketKanbanAPI.Data;
using PocketKanbanAPI.Models; 
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
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
            var context = services.GetRequiredService<AppDbContext>();
            
            Console.WriteLine($"Veritabanı kontrol ediliyor... ({i + 1}/6)");
            context.Database.Migrate();
            Console.WriteLine("🚀 Veritabanı hazır!");


            if (!context.Boards.Any())
            {
                Console.WriteLine("Veritabanı boş, varsayılan veriler ekleniyor...");

                var defaultBoard = new Board 
                { 
                    Title = "Ana Proje Panosu", 
                    CreatedAt = DateTime.Now 
                };
                context.Boards.Add(defaultBoard);
                context.SaveChanges();

                var columns = new List<Column>
                {
                    new Column { Title = "Yapılacaklar", OrderNo = 1, BoardId = defaultBoard.Id },
                    new Column { Title = "Sürüyor",      OrderNo = 2, BoardId = defaultBoard.Id },
                    new Column { Title = "Bitti",        OrderNo = 3, BoardId = defaultBoard.Id }
                };
                context.Columns.AddRange(columns);
                context.SaveChanges();

                Console.WriteLine("Varsayılan Pano ve Sütunlar oluşturuldu!");
            }
            
            break;
        }
        catch (Exception ex)
        {
            if (i == 5) throw; 
            Console.WriteLine("SQL Server bekleniyor...");
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
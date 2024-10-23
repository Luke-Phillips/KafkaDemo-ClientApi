using ClientApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientApi.Data;

public class ClientApiContext : DbContext
{
    public DbSet<SavedNumber> SavedNumbers { get; set; }

    public string DbPath { get; }

    public ClientApiContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "clientApi.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
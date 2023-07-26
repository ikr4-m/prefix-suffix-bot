using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PrefixSuffixBot.Helper;
public class DatabaseContext : DbContext
{
    public string Directory = Path.Join(AppDomain.CurrentDomain.BaseDirectory, "database.sqlite");
    public DbSet<Keyword> Keywords { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Directory};");
    }
}

public class Keyword
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key] public int Id { get; set; }
    public string KeywordText { get; set; } = null!;
    public bool IsUsed { get; set; } = false;
    public DateTime DateUsed { get; set; }
}
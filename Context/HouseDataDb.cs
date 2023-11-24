using Microsoft.EntityFrameworkCore;

class HouseDataDb : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Server=esp32mysql;Port=3306;Database=esp32db;User=root;Password=root;";

            optionsBuilder.UseMySql(connectionString,
                new MySqlServerVersion(new Version()));
        }
    }

    public HouseDataDb(DbContextOptions<HouseDataDb> options)
        : base(options) { }

    public DbSet<HouseData> HouseData => Set<HouseData>();
}
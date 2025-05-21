// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Design;
// using Microsoft.Extensions.Configuration;

// namespace PatientRecoverySystem.DoctorService.Data
// {
//     public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DoctorDbContext>
//     {
//         public DoctorDbContext CreateDbContext(string[] args)
//         {
//             IConfigurationRoot configuration = new ConfigurationBuilder()
//                 .SetBasePath(Directory.GetCurrentDirectory())
//                 .AddJsonFile("appsettings.json")
//                 .Build();

//             var builder = new DbContextOptionsBuilder<DoctorDbContext>();
//             var connectionString = configuration.GetConnectionString("DefaultConnection");

//             builder.UseSqlServer(connectionString);

//             return new DoctorDbContext(builder.Options);
//         }
//     }
// }
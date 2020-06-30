using System.Data.SqlClient;
using Contpaqi.Sql.ADD.OtherMetadata;

namespace ListaNegraSat.Infrastructure.ContpaqiAdd.Factories
{
    public static class AddOtherMetadataDbContextFactory
    {
        public static AddOtherMetadataDbContext Crear(string contpaqiAddConnectionString, string guidCompany)
        {
            var builder = new SqlConnectionStringBuilder(contpaqiAddConnectionString);
            builder.InitialCatalog = $"other_{guidCompany}_metadata";
            return new AddOtherMetadataDbContext(new SqlConnection(builder.ConnectionString), true);
        }
    }
}
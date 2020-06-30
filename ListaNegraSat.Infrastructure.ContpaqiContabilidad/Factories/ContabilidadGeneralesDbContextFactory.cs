using System.Data.SqlClient;
using Contpaqi.Sql.Contabilidad.Generales;

namespace ListaNegraSat.Infrastructure.ContpaqiContabilidad.Factories
{
    public static class ContabilidadGeneralesDbContextFactory
    {
        public static ContabilidadGeneralesDbContext Crear(string contpaqiConnectionString)
        {
            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) {InitialCatalog = "GeneralesSQL", PersistSecurityInfo = true};
            return new ContabilidadGeneralesDbContext(new SqlConnection(builder.ToString()), true);
        }
    }
}
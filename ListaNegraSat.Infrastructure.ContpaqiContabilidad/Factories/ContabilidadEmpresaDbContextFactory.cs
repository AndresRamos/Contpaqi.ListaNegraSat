using System;
using System.Data.SqlClient;
using Contpaqi.Sql.Contabilidad.Empresa;

namespace ListaNegraSat.Infrastructure.ContpaqiContabilidad.Factories
{
    public static class ContabilidadEmpresaDbContextFactory
    {
        public static ContabilidadEmpresaDbContext Crear(string contpaqiConnectionString, string initialCatalog)
        {
            if (contpaqiConnectionString == null)
            {
                throw new ArgumentNullException(nameof(contpaqiConnectionString));
            }

            if (initialCatalog == null)
            {
                throw new ArgumentNullException(nameof(initialCatalog));
            }

            var builder = new SqlConnectionStringBuilder(contpaqiConnectionString) {InitialCatalog = initialCatalog};
            var context = new ContabilidadEmpresaDbContext(new SqlConnection(builder.ToString()), true);
            return context;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contpaqi.ListaNegraSat.WpfApp.Models;
using Contpaqi.Sdk;
using Contpaqi.Sdk.Extras.Interfaces;
using Contpaqi.Sdk.Extras.Modelos;
using Contpaqi.Sdk.Extras.Repositorios;

namespace Contpaqi.ListaNegraSat.WpfApp.DAL
{
    public class ClienteContpaqRepositorio
    {
        private readonly ErrorContpaqiSdkRepositorio _errorContpaqiSdkRepositorio;
        private readonly IContpaqiSdk _sdk;

        public ClienteContpaqRepositorio(IContpaqiSdk sdk)
        {
            _sdk = sdk;
            _errorContpaqiSdkRepositorio = new ErrorContpaqiSdkRepositorio(sdk);
        }

        public List<ClienteContpaq> TraerClientes()
        {
            var clientesList = new List<ClienteContpaq>();
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fPosPrimerCteProv();

            clientesList.Add(LeerDatosClienteProveedorActual());
            while (_sdk.fPosSiguienteCteProv() == 0)
            {
                clientesList.Add(LeerDatosClienteProveedorActual());
                if (_sdk.fPosEOFCteProv() == 1)
                {
                    break;
                }
            }
            return clientesList;
        }

        private ClienteContpaq LeerDatosClienteProveedorActual()
        {
            var codigo = new StringBuilder(Constantes.kLongCodigo);
            var razonSocial = new StringBuilder(Constantes.kLongNombre);
            var rfc = new StringBuilder(Constantes.kLongRFC);
            var tipoCliente = new StringBuilder(7);
            var estatus = new StringBuilder(7);
            var id = new StringBuilder(12);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CCODIGOCLIENTE", codigo, Constantes.kLongCodigo);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CRAZONSOCIAL", razonSocial, Constantes.kLongNombre);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CRFC", rfc, Constantes.kLongRFC);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CTIPOCLIENTE", tipoCliente, 7);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CESTATUS", estatus, 7);
            _errorContpaqiSdkRepositorio.ResultadoSdk = _sdk.fLeeDatoCteProv("CIDCLIENTEPROVEEDOR", id, 12);
            var clienteProveedor = new ClienteContpaq(); ;
            clienteProveedor.Codigo = codigo.ToString();
            clienteProveedor.RazonSocial = razonSocial.ToString();
            clienteProveedor.Rfc = rfc.ToString();
            clienteProveedor.Tipo = int.Parse(tipoCliente.ToString());
            clienteProveedor.Estatus = int.Parse(estatus.ToString());
            clienteProveedor.Id = int.Parse(id.ToString());
            return clienteProveedor;
        }
    }
}

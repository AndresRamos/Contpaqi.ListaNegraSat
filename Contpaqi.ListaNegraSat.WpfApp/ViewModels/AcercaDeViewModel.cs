using System.Reflection;
using System.Text;
using Contpaqi.ListaNegraSat.WpfApp.Messages;
using GalaSoft.MvvmLight;

namespace Contpaqi.ListaNegraSat.WpfApp.ViewModels
{
    public class AcercaDeViewModel : ViewModelBase
    {
        private string _company;
        private string _copyright;
        private string _description;
        private string _product;
        private string _version;

        public AcercaDeViewModel()
        {
            GetAseemblyInfo();
        }

        public string Product
        {
            get => _product;
            set => Set(() => Product, ref _product, value);
        }

        public string Description
        {
            get => _description;
            set => Set(() => Description, ref _description, value);
        }

        public string Company
        {
            get => _company;
            set => Set(() => Company, ref _company, value);
        }

        public string Copyright
        {
            get => _copyright;
            set => Set(() => Copyright, ref _copyright, value);
        }

        public string Version
        {
            get => _version;
            set => Set(() => Version, ref _version, value);
        }

        public void ShowView()
        {
            MessengerInstance.Send(new ShowViewMessage(this));
        }

        private string _companyInfo;

        public string CompanyInfo
        {
            get => _companyInfo;
            set => Set(() => CompanyInfo, ref _companyInfo, value);
        }
        private void GetAseemblyInfo()
        {
            var product = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), true)[0] as AssemblyProductAttribute;
            var description = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true)[0] as AssemblyDescriptionAttribute;
            var company = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), true)[0] as AssemblyCompanyAttribute;
            var copyright = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0] as AssemblyCopyrightAttribute;
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            Product = product.Product;
            Description = description.Description;
            Company = company.Company;
            Copyright = copyright.Copyright;
            Version = version.ToString();

            var companyInfo = new StringBuilder();
            companyInfo.AppendLine("Soluciones a la medida especializados en los sistemas de Contpaqi.");
            companyInfo.AppendLine("www.arsoft.net");
            CompanyInfo = companyInfo.ToString();
        }
    }
}
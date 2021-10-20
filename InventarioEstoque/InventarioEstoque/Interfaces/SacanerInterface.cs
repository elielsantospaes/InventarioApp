using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InventarioEstoque.Interfaces
{
    public interface IScannerInterface
    {
        Task<string> CustomScan(Xamarin.Forms.ContentPage context);
    }
}

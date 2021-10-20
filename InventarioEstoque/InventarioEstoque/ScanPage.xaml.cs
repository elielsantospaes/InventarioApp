using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventarioEstoque
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ScanPage : ContentPage
	{

        // String que vai receber os dados do QRcode
        public static string codigo { get; set; }
        public static string modelo = null;

        private readonly Action<string> setResultAction;

        public ScanPage ()
		{
			InitializeComponent();
		}

        public ScanPage(Action<string> action)
        {
            InitializeComponent();
            this.setResultAction = action;
        }

        public static async Task<string> LeCodigo(INavigation navigation)
        {
            TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();

            void callback(string codigoLido)
            {
                completionSource.TrySetResult(codigoLido);
            }

            var popup = new ScanPage(callback);

            await navigation.PushModalAsync(popup);

            return await completionSource.Task;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            TimeSpan nofocus = new TimeSpan(0, 0, 0, 2, 0);
            Device.StartTimer(nofocus, () =>
            {
                scanner.AutoFocus();
                return true;
            });
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        //Método para escaneamento de códigos via câmera com o pacote ZXing
        void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                codigo = result.Text;

                string formato = result.BarcodeFormat.ToString();
                // Se o modelo lido corresponder a um dos modelos aceitos pelo programa, lê os dados e finaliza
                if ("EAN_13#QR_CODE".Contains(formato))
                {
                    modelo = (formato == "EAN_13" ? "EAN13" : "QR");
                    scanner.IsScanning = false;
                    setResultAction?.Invoke(codigo);
                    this.Navigation.PopModalAsync().ConfigureAwait(false);
                }
                // Caso contrário ajusta o foco e continua lendo
                else if (scanner.IsScanning)
                {

                }
            });
        }

        private void ImagebuttonTorch_Clicked(object sender, EventArgs e)
        {
            // Alterna estado da luz
            scanner.ToggleTorch();
        }
    }

}
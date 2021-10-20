using System;
using System.IO;
using Xamarin.Forms;
using SQLite;
using InventarioEstoque.Model;
using InventarioEstoque.Classes;

namespace InventarioEstoque
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            carrega_base_de_dados();
            NavigationPage.SetHasNavigationBar(this, false); // Elimina a barra superior
        }

        private async void Iniciar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InventarioPage());

        }

        public async void carrega_base_de_dados()
        {
            if (!File.Exists(Path.Combine(Constantes.path, "Dados.db")))
            {
                await DisplayAlert("Atenção", "O banco de dados não existe. É necessário criar um banco de dados para o processo de inventário", "Ok");
                BancoDados.cria_banco_de_dados();
            }
            else
            {
                await DisplayAlert("Atenção", "O banco de dados não existe. É necessário criar um banco de dados para o processo de inventário", "Ok");
            }

        }


    }
}

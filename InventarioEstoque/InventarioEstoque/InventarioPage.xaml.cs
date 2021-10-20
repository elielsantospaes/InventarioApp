using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using InventarioEstoque.Model;
using InventarioEstoque.Classes;
using System.IO;
using Xamarin.Essentials;
using System.Text;
namespace InventarioEstoque
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioPage : ContentPage
    {

        public string modelo = null;
        public string CodProduto = "";
        public List<Dados> tabela_de_dados = new List<Dados>();
        public Dados dados_lidos;
        public InventarioPage()
        {
            InitializeComponent();
            limpa_campos();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            // a tela de consulta produto fica esperando o retorno da tela de scan (naturalmente)
            CodProduto = await ScanPage.LeCodigo(this.Navigation);

            if (!string.IsNullOrEmpty(CodProduto))
            {
                // tratar o dado lido aqui.
                modelo = ScanPage.modelo; // guarda o modelo do código lido
                try
                {
                    // Modelo de dados que pode vir na etiqueta:
                    // Nos modelos QR:
                    // Pedido:2019/65587;Data/Hora:18/02/2019 10:19:16;Codigo:XAEATB 30X20;Descricao:CONEXAO DE ACO
                    // Codigo:AEA 10;Descricao:CONEXAO DE ACO;Data/Hora:02/04/2021 23:39:16
                    // No modelo EAN13
                    // 7898357417892

                    string[] dados = CodProduto.Split(';');
                    string codProduto = CodProduto; // caso não identifique o código padrão, usa o código lido
                    for (int i = 0; i < dados.Length; i++)
                    {
                        if (dados[i].Split(':')[0].ToUpper() == "CODIGO")
                        {
                            codProduto = dados[i].Split(':')[1];
                            break;
                        }
                    }

                    CodProd_Entry.Text = codProduto;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Atenção!", "Erro na leitura do código:\r\n" + ex.Message, "OK");
                }

            }
        }

        private async void Inventatiar_Clicked(object sender, EventArgs e)
        {
            // Verifica se os campos código e quantidade estão preenchidos antes de inventariar.
            if (String.IsNullOrEmpty(CodProd_Entry.Text) || String.IsNullOrEmpty(Qtd_Entry.Text))
            {
                await DisplayAlert("Atenção!", "O código e a quantidade devem ser informados! Preencha estes campos e tente novamente", "OK");
            }
            // Faz a gravação de um dado novo após a descrição
            else if (!String.IsNullOrEmpty(Descricao_Entry.Text))
            {
                grava_novo_produto();
            }
            else
            {
                if (existe_dado(CodProd_Entry.Text).Codigo == CodProd_Entry.Text)
                {
                    atualiza_produto();
                }
                else if (existe_dado(CodProd_Entry.Text).Encontrado == "Não")
                {
                    string mensagem = "Não existe registro do produto informado.\nSe tiver certeza que o código foi informado corretamente faça uma breve descrição do produto antes de inventaiar. Caso contrário, cancele e reinicie o processo de inventário";
                    var action = await DisplayAlert("Atenção!", mensagem, "Descrever", "Cancelar");
                    string string_task = action.ToString(); // Descrever = True ; Cancelar = False
                    switch (string_task)
                    {
                        case "True":
                            Descricao_Entry.IsVisible = true;
                            Descricao_Entry.Focus();
                            break;
                        case "False":
                            limpa_campos();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // Faz a leitura completa de um dado da tabela e exibe os campos na tela.
        private async void Leitura_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(CodProd_Entry.Text))
            {
                await DisplayAlert("Atenção!", "Informe o código do produto à ser consultado.", "OK");
            }
            else if (existe_dado(CodProd_Entry.Text).Codigo == CodProd_Entry.Text)
            {
                for (int i = 0; i < tabela_de_dados.Count; i++)
                {
                    if (tabela_de_dados[i].Codigo == CodProd_Entry.Text)
                    {
                        string dados_str = "id = " + tabela_de_dados[i].id.ToString() + "\n" +
                            "Código = " + tabela_de_dados[i].Codigo + "\n" +
                            "Decrição = " + tabela_de_dados[i].Descricao + "\n" +
                            "Quantidade = " + tabela_de_dados[i].Qtd.ToString() + "\n" +
                            "Encontrado = " + tabela_de_dados[i].Encontrado + "\n" +
                            "Leitura/Digitado = " + tabela_de_dados[i].LeituraDigitado + "\n" +
                            "Data_Hora = " + tabela_de_dados[i].DH_preenchimento.ToString();
                        await DisplayAlert("Atenção!", dados_str, "OK");
                        limpa_campos();
                    }
                }
            }
            else
            {
                await DisplayAlert("Atenção!", "Produto não encontrado", "OK");
            }

        }

        private async void Deletar_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(CodProd_Entry.Text))
            {
                await DisplayAlert("Atenção!", "Informe o código do produto a ser deletado.", "OK");
            }
            else if (existe_dado(CodProd_Entry.Text).Codigo == CodProd_Entry.Text)
            {
                dados_lidos = new Dados();
                dados_lidos = existe_dado(CodProd_Entry.Text);
                string mensagem = "O produto com código " + CodProd_Entry.Text + " será deletado dos registros.\nVeja abaixo a descrição e a quantidade do produto e escolha uma das opções:\n\nDescição: " + dados_lidos.Descricao + "\nQuantidade: " + dados_lidos.Qtd;
                var action = await DisplayAlert("Atenção!", mensagem, "Deletar", "Cancelar");
                string string_task = action.ToString();
                switch (string_task)
                {
                    case "True":
                        if (BancoDados.deletar_dados(dados_lidos))
                        {
                            await DisplayAlert("Atenção!", "Dados deletados com sucesso!", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Atencão!", "Falha ao deletar dados!", "OK");
                        }
                        limpa_campos();
                        break;
                    case "False":
                        limpa_campos();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                await DisplayAlert("Atenção!", "Produto não encontrado", "OK");
            }
        }

        // Exporta o banco de dados para um arquivo csv.
        private async void Exportar_csv_Clicked(object sender, EventArgs e)
        {
            string path_csv = Path.Combine(FileSystem.CacheDirectory, "Inventario.csv");
            string arquivo = Path.Combine(Constantes.path, "saida.csv");
            string dados_saida = "ID,Código,Descrição,Quantidade,Encontrado,Leitura/Digitado,Data_Hora\n";

            tabela_de_dados = BancoDados.le_dados_do_banco();

            for (int i = 0; i < tabela_de_dados.Count; i++)
            {
                dados_saida += tabela_de_dados[i].id + "," +
                 tabela_de_dados[i].Codigo + "," +
                    tabela_de_dados[i].Descricao + "," +
                    tabela_de_dados[i].Qtd.ToString() + "," +
                    tabela_de_dados[i].Encontrado + "," +
                    tabela_de_dados[i].LeituraDigitado + "," +
                    tabela_de_dados[i].DH_preenchimento.ToString() + "\n";
            }

            if (File.Exists(arquivo))
            {
                File.WriteAllText(path_csv, dados_saida);
                string leitura = File.ReadAllText(arquivo);
                //await DisplayAlert("Dados do arquivo .csv", leitura, "Ok");
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Exportando base de dados.",
                    File = new ShareFile(path_csv)
                });
            }
            else
            {
                await DisplayAlert("Dados do arquivo .csv", "Erro ao exportar o arquivo", "Ok");
            }

            string mensagem = "Banco de dados exportado com sucesso.\n\nDeseja limpar os dados locais?\nEcolha uma das opções.\n\n";
            var action = await DisplayAlert("Atenção!", mensagem, "Limpar", "Cancelar");
            string string_task = action.ToString();
            switch (string_task)
            {
                case "True":
                    limpa_banco_de_dados();
                    limpa_campos();
                    break;
                case "False":
                    limpa_campos();
                    break;
                default:
                    break;
            }
        }

        public Dados existe_dado(string codigo)
        {
            tabela_de_dados = BancoDados.le_dados_do_banco();
            if (tabela_de_dados.Count > 0)
            {
                for (int i = 0; i < tabela_de_dados.Count; i++)
                {
                    if (tabela_de_dados[i].Codigo == codigo)
                    {
                        return tabela_de_dados[i];
                    }
                }
            }
            dados_lidos = new Dados();
            dados_lidos.Encontrado = "Não";
            return dados_lidos;
        }

        private void Descricao_Entry_Completed(object sender, EventArgs e)
        {
            grava_novo_produto();
        }

        private async void Quantidade_Entry_Completed(object sender, EventArgs e)
        {
            if (existe_dado(CodProd_Entry.Text).Codigo == CodProd_Entry.Text)
            {
                atualiza_produto();
            }
            else if (existe_dado(CodProd_Entry.Text).Encontrado == "Não")
            {
                string mensagem = "Não existe registro do produto informado.\nSe tiver certeza que o código foi informado corretamente, faça uma breve descrição do produto antes de inventaiar. Caso contrário cancele e reinicie o processo de inventário";
                var action = await DisplayAlert("Atenção!", mensagem, "Descrever", "Cancelar");
                string string_task = action.ToString();
                switch (string_task)
                {
                    case "True":
                        Descricao_Entry.IsVisible = true;
                        Descricao_Entry.Focus();
                        break;
                    case "False":
                        limpa_campos();
                        break;
                    default:
                        break;
                }
            }
        }

        public async void atualiza_produto()
        {
            Dados dados = new Dados();
            dados.Codigo = CodProd_Entry.Text;
            dados_lidos = new Dados();
            dados_lidos = existe_dado(CodProd_Entry.Text);
            dados.Encontrado = "Sim";
            string mensagem = "Já existe um registro deste produto. Veja abaixo a descrição e a quantidade deste produto.\n\nDescrição: " + dados_lidos.Descricao + "\nQuantidade: " + dados_lidos.Qtd + "\n\n" +
                "Toque em ADICIONAR para somar a quantidade informada à quantidade existente \n\n" +
                "Toque em SUBSTITUIR para substituir a quantidade existente\n\n";
            var action = await DisplayAlert("Atenção!", mensagem, "Adicionar", "Substituir");//Substituir = False, Adicionar = True
            string string_task = action.ToString();
            switch (string_task)
            {
                case "True":
                    dados.Qtd = dados_lidos.Qtd + int.Parse(Qtd_Entry.Text);
                    break;
                case "False":
                    dados.Qtd = int.Parse(Qtd_Entry.Text);
                    break;
                default:
                    break;
            }
            if (modelo == null)
            {
                dados.LeituraDigitado = "Digitado";
            }
            else
            {
                dados.LeituraDigitado = "Leitura";
            }

            dados.DH_preenchimento = DateTime.Now;
            if (BancoDados.atualizar_dados(dados))
            {
                await DisplayAlert("Antenção!", "Gravação concluido com sucesso!", "OK");
            }
            else
            {
                await DisplayAlert("Atenção!", "Falha no gravação dos dados", "OK");
            }
            limpa_campos();
        }

        public async void grava_novo_produto()
        {
            Dados dados = new Dados();
            dados.Codigo = CodProd_Entry.Text;
            dados.Encontrado = "Não";
            dados.Descricao = Descricao_Entry.Text;
            Descricao_Entry.IsVisible = false;
            dados.Qtd = int.Parse(Qtd_Entry.Text);
            if (modelo == null)
            {
                dados.LeituraDigitado = "Digitado";
            }
            else
            {
                dados.LeituraDigitado = "Leitura";
            }
            dados.DH_preenchimento = DateTime.Now;
            if (BancoDados.insere_dados_no_banco(dados))
            {
                await DisplayAlert("Atenção!", "Gravação concluida com sucesso", "OK");
            }
            else
            {
                await DisplayAlert("Atenção!", "Falha na gravação dos dados.", "OK");
            }
            limpa_campos();
        }

        public void limpa_campos()
        {
            modelo = null;
            CodProd_Entry.Text = "";
            Descricao_Entry.Text = null;
            Qtd_Entry.Text = "";
            tabela_de_dados.Clear();
            CodProd_Entry.Focus();
        }

        public async void limpa_banco_de_dados()
        {
            Dados dados = new Dados();
            string mensagem = "Todos so dados gravados no smartphone serão apagados, preservando apenas o CÓDIGO e a DESCRIÇÃO dos produtos. Deseja continuar?\n\n";
            var action = await DisplayAlert("Atenção!", mensagem, "Sim", "Cancelar");
            string string_task = action.ToString();
            switch (string_task)
            {
                case "True":
                    tabela_de_dados = BancoDados.le_dados_do_banco();
                    for (int i = 0; i < tabela_de_dados.Count; i++)
                    {
                        dados.Codigo = tabela_de_dados[i].Codigo;
                        dados.Descricao = tabela_de_dados[i].Descricao;
                        dados.Qtd = 0;
                        dados.LeituraDigitado = "";
                        dados.Encontrado = "";
                        dados.DH_preenchimento = new DateTime();
                        BancoDados.atualizar_dados(dados);
                        limpa_campos();
                    }
                    break;
                case "False":
                    limpa_campos();
                    break;
                default:
                    break;
            }
        }
    }
}
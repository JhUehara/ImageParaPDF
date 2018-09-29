using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace ImageToPdf
{
    public partial class MainForm : Form
    {
        //Variaveis para serem utilizadas para armazenar os caminhos de origem e destino
        private string srcFile, destFile;
        bool success = false;

        public MainForm()
        {
            InitializeComponent();
        }
        
        //Botao para escolher uma imagem de origem
        private void btnSelectSrc_Click(object sender, EventArgs e)
        {
            //chama openfiledialog para e aguarda um retorno positivo
            if (ofdSrcFile.ShowDialog() != DialogResult.OK)
                return;

            //variavel declarada no inicio do codigo recebe o caminho do arquivo completo
            srcFile = ofdSrcFile.FileName;
            txbxSrcFile.Text = srcFile;
            
            //preenchimento da textbox de destino adicionando a extensão .pdf no arquivo
            txbxDestFile.Text =

                //exibe o Path com o diretorio "apenas as pastas onde o arquivo será salvo"
                Path.GetDirectoryName(srcFile) + "\\" +
                
                //biblioteca Path retira a extensão do arquivo e apresenta somente o nome do arquivo para inclusão do .pdf
                Path.GetFileNameWithoutExtension(srcFile) + ".pdf";
            destFile = txbxDestFile.Text;   
        }

        //Botão para adicionar o caminho de destino do arquivo pdf
        private void btnSelectDest_Click(object sender, EventArgs e)
        {
            if (sfdDestFile.ShowDialog() != DialogResult.OK)
                return;

            // Adiciona o caminho alterado, na FONTE existe um erro pois não possui extensão configurada
            destFile = sfdDestFile.FileName;
            txbxDestFile.Text = destFile;
        }

        //Botão que inicia conversão do arquivo
        private void btnConvert_Click(object sender, EventArgs e)
        {
           
            //Caixa de ferramenta para exibir um ErroProvider
            errProv.Clear();

            //Condição: caso não exista uma caminho de origem ele fará 
            if (txbxSrcFile.Text.Length == 0)
            {
                //Solicita um arquivo e retorna
                errProv.SetError(txbxSrcFile, "Please point source file.");
                return;
            }

            //Fará o mesmo caso não exista destino
            else if (txbxDestFile.Text.Length == 0)
            {   
                //Solitica um destino e retorna
                errProv.SetError(txbxDestFile, "Please point destination file.");
                return;
            }

            success = false;
            //BackgroundWorker inicia um processo em segundo plano e envia os dados de origem e destino
            bw.RunWorkerAsync(new string[2] { srcFile, destFile });

            //toolStripProgressBar exibe o progresso da conversão da imagem
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;

        }

        //Evento do BackgroundWork que irá receber as variaveis e converter o arquivo
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //Recebe os caminhos de origem e destino
                string source = (e.Argument as string[])[0];
                string destinaton = (e.Argument as string[])[1];

                // instancia a biblioteca PdfDocument
                PdfDocument doc = new PdfDocument();
                
                //adiciona a imagem em uma pagina pdf
                doc.Pages.Add(new PdfPage());

                //Variaveis do tipo imagem para receber a imagem da origem e salvar dentro do PDF
                XGraphics xgr = XGraphics.FromPdfPage(doc.Pages[0]);
                XImage img = XImage.FromFile(source);
                xgr.DrawImage(img, 0, 0);

                //Salva o documento no destino
                doc.Save(destinaton);

                //Fecha o documento
                doc.Close();

                //Booleano é setado como verdadeiro
                success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //BackgroundWorker para apresentar o enchimento da barra de progresso
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
            toolStripProgressBar1.Value = 0;

            if (success)
                MessageBox.Show("The converion ended successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Botão para visualizar o arquivo parece estar quebrado
        private void button1_Click(object sender, EventArgs e)
        {
            errProv.Clear();

            if (rbSrc.Checked && txbxSrcFile.Text.Length == 0)
            {
                errProv.SetError(txbxSrcFile, "Please point source file.");
                return;
            }
            else if (rbDest.Checked && txbxDestFile.Text.Length == 0)
            {
                errProv.SetError(txbxDestFile, "Please point destination file.");
                return;
            }

            try
            {
                if (rbSrc.Checked)
                    Process.Start(srcFile);
                else if (rbDest.Checked)
                    Process.Start(destFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);            
            }
        }
    }
}
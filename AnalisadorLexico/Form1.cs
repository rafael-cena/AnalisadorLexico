using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalisadorLexico
{
    public partial class Form1 : Form
    {
        int index = 1;
        int linhaLogErro = 8;

        public Form1()
        {
            InitializeComponent();

            // Define o painel de números para desenhar
            pnNumbers.Paint += PnNumbers_Paint;

            // Força redesenho ao digitar
            rtbEditor.TextChanged += (s, e) => pnNumbers.Invalidate();

            // Força redesenho ao rolar (scroll)
            rtbEditor.VScroll += (s, e) => pnNumbers.Invalidate();

            // Garante que o painel de números seja transparente ou da cor certa
            pnNumbers.BackColor = Color.LightGray;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void PnNumbers_Paint(object sender, PaintEventArgs e)
        {
            // Calcula o número de linhas e desenha-as
            int firstVisibleLine = rtbEditor.GetLineFromCharIndex(rtbEditor.GetCharIndexFromPosition(new Point(0, 0)));
            int lastVisibleLine = rtbEditor.GetLineFromCharIndex(rtbEditor.GetCharIndexFromPosition(new Point(0, rtbEditor.Height)));

            for (int i = firstVisibleLine; i <= lastVisibleLine; i++)
            {
                Point pos = rtbEditor.GetPositionFromCharIndex(rtbEditor.GetFirstCharIndexFromLine(i));

                // Desenha o número da linha ao lado
                e.Graphics.DrawString(
                    (i + 1).ToString(),
                    rtbEditor.Font,
                    Brushes.Black,
                    pnNumbers.Width - 25, // Alinhamento à direita do painel
                    pos.Y
                );
            }
        }

        private void btSalvar_Click(object sender, EventArgs e)
        {

        }

        private void btSalvarComo_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Configure dialog properties
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Save Text File";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.FileName = labelNome.Text;
            saveFileDialog1.RestoreDirectory = true;
            saveFileDialog1.DefaultExt = "txt";

            // Show dialog and check result
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Save file using the selected path
                File.WriteAllText(saveFileDialog1.FileName, rtbEditor.Text);
            }
        }

        private void btCarregar_Click(object sender, EventArgs e)
        {
            string selectedFilePath = "";
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = openFileDialog1.FileName;
                string content = File.ReadAllText(selectedFilePath);
                rtbEditor.Text = content;
            }
        }

        private void btLimpar_Click(object sender, EventArgs e)
        {
            rtbEditor.Clear();
        }

        private void btExecutar_Click(object sender, EventArgs e)
        {
            verificaBlocoPrincipal();
        }

        private void criarLogErro (String mensagem, int linhaErro, int colunaErro)
        {
            Label label = new Label();

            label.Text = "Erro (" + colunaErro + ", " + linhaErro + "): " + mensagem;
            label.Location = new Point(8, linhaLogErro); // Posição dentro do Panel
            label.AutoSize = true; // Ajusta o tamanho ao texto
            label.Name = "labelErro"+linhaLogErro/16;
            linhaLogErro += 16;

            pLogErro.Visible = true;
            pLogErro.Controls.Add(label);
        }

        private bool verificaBlocoPrincipal()
        {
            string codigo = rtbEditor.Text;
            string palavraLida = "";
            int pos = 0;
            while (pos < codigo.Length && codigo[pos] != ' ')
            {
                palavraLida += codigo[pos];
                pos++;
            }
            if (palavraLida != "BLOCO")
            {
                // criarLogErro(mensagem: string, linhaErro: int, colErro: int)
                criarLogErro("Erro de Gramática: 'BLOCO' deve ser utilizado para iniciar um programa!", 1, 1);
                criarLogErro("Erro de Gramática: 'BLOCO' deve ser utilizado para iniciar um programa!", 1, 1);
                return false;
            }
            pos++;
            palavraLida = "";
            while (pos < codigo.Length && codigo[pos] != '.')
            {
                palavraLida += codigo[pos];
                pos++;
            }
            labelNome.Text = palavraLida;
            pos++;
            palavraLida = "";
            while (pos < codigo.Length && codigo[pos] != '{')
            {
                palavraLida += codigo[pos];
                pos++;
            }
            if (pos >= codigo.Length)
            {
                return false;
            }
            pos++;
            while (pos < codigo.Length && codigo[pos] != '}')
            {
                palavraLida += codigo[pos];
                pos++;
            }
            if (pos >= codigo.Length)
            {
                return false;
            }
            return true;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            pLogErro.Visible = false;
        }
    }
}

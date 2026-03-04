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

        public enum TipoToken {
            BLOCO,
            IDENTIFICADOR,
            PONTO,
            ABRE_CHAVE,
            FECHA_CHAVE,
            EOF
        }

        public class Token
        {
            public TipoToken Tipo{ get; set; }
            public string Lexema { get; set; }
            public int Linha { get; set; }
            public int Coluna { get; set; }

            public Token(TipoToken tipo, string lexema, int linha, int coluna)
            {
                Tipo = tipo;
                Lexema = lexema;
                Linha = linha;
                Coluna = coluna;
            }
        }
        public class Lexer
        {
            private string codigo;
            private int pos = 0;
            private int linha = 1;
            private int coluna = 1;

            public Lexer(string codigoFonte)
            {
                codigo = codigoFonte;
            }

            public List<Token> Analisar()
            {
                List<Token> tokens = new List<Token>();

                while (pos < codigo.Length)
                {
                    char atual = codigo[pos];

                    if (char.IsWhiteSpace(atual))
                    {
                        if (atual == '\n')
                        {
                            linha++;
                            coluna = 1;
                        }
                        else
                            coluna++;          
                    }
                    if (char.IsLetter(atual))
                    {
                        string lexema = "";
                        int colInicio = coluna;

                        while (pos < codigo.Length && (char.IsLetterOrDigit(codigo[pos]) || codigo[pos] == '_'))
                        {
                            lexema += codigo[pos];
                            pos++;
                            coluna++;
                        }

                        if (lexema == "BLOCO")
                            tokens.Add(new Token(TipoToken.BLOCO, lexema, linha, colInicio));
                        else
                        {
                            tokens.Add(new Token(TipoToken.IDENTIFICADOR, lexema, linha, colInicio));
                            pos--;
                        }
                    }
                    switch (atual)
                    {
                        case '.':
                            tokens.Add(new Token(TipoToken.PONTO, ".", linha, coluna));
                            break;

                        case '{':
                            tokens.Add(new Token(TipoToken.ABRE_CHAVE, "{", linha, coluna));
                            break;

                        case '}':
                            tokens.Add(new Token(TipoToken.FECHA_CHAVE, "}", linha, coluna));
                            break;
                    }
                    pos++;
                    coluna++;
                }
                tokens.Add(new Token(TipoToken.EOF, "", linha, coluna));
                return tokens;
            }
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
            btFechar_Click(null, null);
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
            Lexer lexer = new Lexer(rtbEditor.Text);
            List<Token> tokens = lexer.Analisar();
            int quantTokens = tokens.Count;
            int i = 0;

            if (tokens[i].Tipo != TipoToken.BLOCO)
            {
                criarLogErro("Esperado 'BLOCO'", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            i++;
            if (tokens[i].Tipo != TipoToken.IDENTIFICADOR)
            {
                criarLogErro("Esperado identificador após BLOCO", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            labelNome.Text = tokens[i].Lexema;
            i++;
            if (tokens[i].Tipo != TipoToken.PONTO)
            {
                criarLogErro("Esperado '.' após identificador", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            i++;
            if (tokens[i].Tipo != TipoToken.ABRE_CHAVE)
            {
                criarLogErro("Esperado '{'", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            i++;
            if (tokens[i].Tipo != TipoToken.FECHA_CHAVE)
            {
                criarLogErro("Esperado '}'", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            if (tokens[quantTokens-1].Tipo != TipoToken.EOF)
            {
                criarLogErro("Esperado 'EOF'", tokens[quantTokens-1].Linha, tokens[quantTokens-1].Coluna);
                return false;
            }
            return true;
        }

        private void btFechar_Click(object sender, EventArgs e)
        {
            pLogErro.Visible = false;
            linhaLogErro = 8;
            for (int i = pLogErro.Controls.Count - 1; i > 0; i --)
                if (pLogErro.Controls[i] is Label)
                {
                    var l = pLogErro.Controls[i];
                    pLogErro.Controls.Remove(l);
                    l.Dispose();
                }
        }
    }
}

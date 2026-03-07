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
            ABRE_CHAVE,
            BLOCO,
            DOIS_PONTOS,
            FECHA_CHAVE,
            IDENTIFICADOR_NOME,
            IDENTIFICADOR_TIPO,
            IGUAL,
            PONTO,
            PONTO_VIRGULA,
            VAR,
            VIRGULA,
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
                    if (char.IsLetterOrDigit(atual))
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
                            if(lexema=="VAR")
                            {
                                tokens.Add(new Token(TipoToken.VAR, lexema, linha, colInicio));
                            }
                            else
                            {
                                if(lexema=="int"||lexema == "float"||lexema == "bool"|| lexema == "String"||lexema == "char"||lexema == "void")
                                {
                                    tokens.Add(new Token(TipoToken.IDENTIFICADOR_TIPO, lexema, linha, colInicio));
                                }
                                else
                                {
                                    tokens.Add(new Token(TipoToken.IDENTIFICADOR_NOME, lexema, linha, colInicio));
                                }
                            }
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
                        
                        case ':':
                            tokens.Add(new Token(TipoToken.DOIS_PONTOS, ":", linha, coluna));
                            break;

                        case ';':
                            tokens.Add(new Token(TipoToken.PONTO_VIRGULA, ":", linha, coluna));
                            break;

                        case '=':
                            tokens.Add(new Token(TipoToken.IGUAL, "=", linha, coluna));
                            break;

                        case ',':
                            tokens.Add(new Token(TipoToken.VIRGULA, ",", linha, coluna));
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
            verificaBloco();
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

        private void verificarSucesso ()
        {
            if (!pLogErro.Visible)
            {
                Label label = new Label();

                label.Text = "Código compilado sem erros!";
                label.Location = new Point(8, 8);
                label.AutoSize = true;
                label.Name = "labelSucesso";

                pLogErro.Visible = true;
                pLogErro.Controls.Add(label);
            }
        }

        private bool verificaBloco()
        {
            Lexer lexer = new Lexer(rtbEditor.Text);
            List<Token> tokens = lexer.Analisar();
            int quantTokens = tokens.Count;
            int i = 0;

            //VERIFIÇÃO BLOCO INICIAL
            if (tokens[i].Tipo != TipoToken.BLOCO)
            {
                criarLogErro("Esperado 'BLOCO'", tokens[i].Linha, tokens[i].Coluna);
                return false;
            }
            i++;
            if (tokens[i].Tipo != TipoToken.IDENTIFICADOR_NOME)
            {
                criarLogErro("Esperado identificador de nome após BLOCO", tokens[i].Linha, tokens[i].Coluna);
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
            //VERIFICAÇÃO DO QUE POSSIVELMENTE ESTA DENTRO DO BLOCO CASO NAO TENHA NADA PROCURA POR }
            if(tokens[i].Tipo == TipoToken.VAR)
            {
                i++;
                if (tokens[i].Tipo != TipoToken.IDENTIFICADOR_TIPO)
                {
                    criarLogErro("Esperado identificador do tipo da variavel após VAR", tokens[i].Linha, tokens[i].Coluna);
                    return false;
                }
                i++;
                if (tokens[i].Tipo != TipoToken.DOIS_PONTOS)
                {
                    criarLogErro("Esperado ':' apos identificador do tipo da variavel", tokens[i].Linha, tokens[i].Coluna);
                    return false;
                }
                i++;
                if(tokens[i].Tipo != TipoToken.IDENTIFICADOR_NOME)
                {
                    criarLogErro("Esperado identificador de nome apos ':'", tokens[i].Linha, tokens[i].Coluna);
                    return false;
                }
                i++;
                if (tokens[i].Tipo == TipoToken.IGUAL)
                {
                    i++;
                    if (tokens[i].Tipo != TipoToken.IDENTIFICADOR_NOME)//VALOR DA VARIAVEL
                    {
                        criarLogErro("Esperado valor da variavel apos '='", tokens[i].Linha, tokens[i].Coluna);
                        return false;
                    }
                    i++;
                }
                while (tokens[i].Tipo == TipoToken.VIRGULA)
                {
                    i++;
                    if (tokens[i].Tipo != TipoToken.IDENTIFICADOR_NOME)//NOME DA VARIAVEL
                    {
                        criarLogErro("Esperado identificador após ','", tokens[i].Linha, tokens[i].Coluna);
                        return false;
                    }
                    i++;
                    if (tokens[i].Tipo == TipoToken.IGUAL)
                    {
                        i++;
                        if (tokens[i].Tipo != TipoToken.IDENTIFICADOR_NOME)
                        {
                            criarLogErro("Esperado valor de inicialização", tokens[i].Linha, tokens[i].Coluna);
                            return false;
                        }
                        i++;
                    }
                }
                if (tokens[i].Tipo != TipoToken.PONTO_VIRGULA)
                {
                    criarLogErro("Esperado ';' no fim da linha", tokens[i].Linha, tokens[i].Coluna);
                    return false;
                }
                i++;
            }
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
            //FIM DA VERIFICAÇÃO
            verificarSucesso();

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

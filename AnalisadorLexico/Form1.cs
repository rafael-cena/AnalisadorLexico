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
            FECHA_CHAVE,
            ABRE_PARENTESE,
            FECHA_PARENTESE,
            BLOCO,
            VAR,
            IF,
            ELSE,
            WHILE,
            FOR,
            RETURN,
            IDENTIFICADOR_NOME,
            IDENTIFICADOR_TIPO,
            NUMERO,
            STRING,
            BOOLEANO,
            IGUAL,
            IGUAL_IGUAL,
            DIFERENTE,
            MENOR,
            MENOR_IGUAL,
            MAIOR,
            MAIOR_IGUAL,
            MAIS,
            MENOS,
            MULT,
            DIV,
            AND,
            OR,
            DOIS_PONTOS,
            PONTO,
            VIRGULA,
            PONTO_VIRGULA,
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

        public class ErroLexico
        {
            public string Mensagem { get; set; }
            public int Linha { get; set; }
            public int Coluna { get; set; }

            public ErroLexico(string mensagem, int linha, int coluna)
            {
                Mensagem = mensagem;
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
            public List<ErroLexico> errosLexicos = new List<ErroLexico>();
            public Lexer(string codigoFonte)
            {
                codigo = codigoFonte;
            }

            private char Proximo()
            {
                if (pos + 1 >= codigo.Length) 
                    return '\0';
                return codigo[pos + 1];
            }

            public List<Token> Analisar()
            {
                List<Token> tokens = new List<Token>();


                while (pos < codigo.Length)
                {
                    char atual = codigo[pos];

                    if (char.IsWhiteSpace(atual))
                    {
                        atual = codigo[pos];
                        if (atual == '\n')
                        {
                            linha++;
                            coluna = 1;
                        }
                        else
                            coluna++;
                        pos++;
                        continue;
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
                        switch (lexema)
                        {
                            case "BLOCO":
                                tokens.Add(new Token(TipoToken.BLOCO, lexema, linha, colInicio));
                                break;
                            case "VAR":
                                tokens.Add(new Token(TipoToken.VAR, lexema, linha, colInicio));
                                break;
                            case "IF":
                                tokens.Add(new Token(TipoToken.IF, lexema, linha, colInicio));
                                break;
                            case "ELSE":
                                tokens.Add(new Token(TipoToken.ELSE, lexema, linha, colInicio));
                                break;
                            case "WHILE":
                                tokens.Add(new Token(TipoToken.WHILE, lexema, linha, colInicio));
                                break;
                            case "FOR":
                                tokens.Add(new Token(TipoToken.FOR, lexema, linha, colInicio));
                                break;
                            case "RETURN":
                                tokens.Add(new Token(TipoToken.RETURN, lexema, linha, colInicio));
                                break;
                            case "True":
                            case "False":
                                tokens.Add(new Token(TipoToken.BOOLEANO, lexema, linha, colInicio));
                                break;
                            case "int":
                            case "float":
                            case "bool":
                            case "String":
                            case "char":
                            case "void":
                                tokens.Add(new Token(TipoToken.IDENTIFICADOR_TIPO, lexema, linha, colInicio));
                                break;
                            case "AND":
                                tokens.Add(new Token(TipoToken.AND, lexema, linha, colInicio));
                                break;
                            case "OR":
                                tokens.Add(new Token(TipoToken.OR, lexema, linha, colInicio));
                                break;
                            default:
                                tokens.Add(new Token(TipoToken.IDENTIFICADOR_NOME, lexema, linha, colInicio));
                                break;
                        }
                        continue;
                    }
                    // NUMEROS
                    if (char.IsDigit(atual))
                    {
                        int colInicio = coluna;
                        string numero = "";

                        while (pos < codigo.Length && char.IsDigit(codigo[pos]))
                        {
                            atual = codigo[pos];
                            numero += atual;
                            pos++;
                            coluna++;
                        }
                        atual = codigo[pos];
                        if (atual == '.')
                        {
                            numero += '.';
                            pos++;
                            coluna++;

                            while (pos < codigo.Length && char.IsDigit(codigo[pos]))
                            {
                                atual = codigo[pos];
                                numero += atual;
                                pos++;
                                coluna++;
                            }
                        }
                        tokens.Add(new Token(TipoToken.NUMERO, numero, linha, colInicio));
                        continue;
                    }
                    if (atual == '"')
                    {
                        int colInicio = coluna;
                        string texto = "";

                        pos++;
                        coluna++;
                        atual=codigo[pos];
                        while (pos < codigo.Length-1 && atual != '"')//ABRE ASPAS DA STRING E LE STRING ATE FECHA ASPAS OU ATE O FIM DO CODIGO
                        {
                            texto += atual;
                            pos++;
                            coluna++;
                            atual = codigo[pos];
                        }

                        if (atual == '"')//FECHA ASPAS
                        {
                            pos++;
                            coluna++;
                            tokens.Add(new Token(TipoToken.STRING, texto, linha, colInicio));
                        }
                        else//NÃO ECONTROU FECHA ASPAS
                        {
                            errosLexicos.Add(new ErroLexico($"String não fechada na linha '{linha}'", linha,coluna));
                        }
                        continue;
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
                            tokens.Add(new Token(TipoToken.PONTO_VIRGULA, ";", linha, coluna));
                            break;

                        case '=':
                            if (Proximo() == '=')
                            {
                                tokens.Add(new Token(TipoToken.IGUAL_IGUAL, "==", linha, coluna));
                                pos++;
                                coluna++;
                            }
                            else
                                tokens.Add(new Token(TipoToken.IGUAL, "=", linha, coluna));
                            break;

                        case ',':
                            tokens.Add(new Token(TipoToken.VIRGULA, ",", linha, coluna));
                            break;

                        case '(': tokens.Add(new Token(TipoToken.ABRE_PARENTESE, "(", linha, coluna)); 
                            break;

                        case ')': tokens.Add(new Token(TipoToken.FECHA_PARENTESE, ")", linha, coluna)); 
                            break;

                        case '+': tokens.Add(new Token(TipoToken.MAIS, "+", linha, coluna)); 
                            break;

                        case '-': tokens.Add(new Token(TipoToken.MENOS, "-", linha, coluna)); 
                            break;

                        case '*': tokens.Add(new Token(TipoToken.MULT, "*", linha, coluna)); 
                            break;

                        case '/': tokens.Add(new Token(TipoToken.DIV, "/", linha, coluna)); 
                            break;

                        case '!':
                            if (Proximo() == '=')
                            {
                                tokens.Add(new Token(TipoToken.DIFERENTE, "!=", linha, coluna));
                                pos++;
                                coluna++;
                            }
                            break;

                        case '<':
                            if (Proximo() == '=')
                            {
                                tokens.Add(new Token(TipoToken.MENOR_IGUAL, "<=", linha, coluna));
                                pos++;
                                coluna++;
                            }
                            else
                                tokens.Add(new Token(TipoToken.MENOR, "<", linha, coluna));
                            break;

                        case '>':
                            if (Proximo() == '=')
                            {
                                tokens.Add(new Token(TipoToken.MAIOR_IGUAL, ">=", linha, coluna));
                                pos++;
                                coluna++;
                            }
                            else
                                tokens.Add(new Token(TipoToken.MAIOR, ">", linha, coluna));
                            break;

                        default:
                            errosLexicos.Add(new ErroLexico($"Símbolo inválido '{atual}' na linha '{linha}', coluna '{coluna}'",linha,coluna));
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
            // limpar o editor de texto
            rtbEditor.Clear();
            // limpar log de erros
            btFechar_Click(null, null);
        }

        private void btExecutar_Click(object sender, EventArgs e)
        {
            btFechar_Click(null, null);
            Lexer lexer = new Lexer(rtbEditor.Text);
            List<Token> tokens = lexer.Analisar();
            if (lexer.errosLexicos.Count > 0)
            {
                foreach(var erro in lexer.errosLexicos)
                {
                    criarLogErro(erro.Mensagem, erro.Linha, erro.Coluna);
                }
            }
            else
            {
                Parser parser = new Parser(tokens);
                parser.Parse();

                // verifica erros sintaticos
                if (parser.erros.Count > 0)
                {
                    foreach (var erro in parser.erros)
                    {
                        criarLogErro(erro.Mensagem, erro.Linha, erro.Coluna);
                    }
                }
                else
                {
                    // se não houver erros, mostra sucesso e lista de tokens
                    verificarSucesso(tokens);
                }
            }
        }

        private void criarLogErro (String mensagem, int linhaErro, int colunaErro)
        {
            Label label = new Label();

            label.Text = "Erro (" + colunaErro + ", " + linhaErro + "): " + mensagem;
            label.Font = new Font("Consolas", 9, FontStyle.Regular);
            label.Location = new Point(8, linhaLogErro); // Posição dentro do Panel
            label.AutoSize = true; // Ajusta o tamanho ao texto
            label.Name = "labelErro"+linhaLogErro/16;
            linhaLogErro += 16;

            pLogErro.Visible = true;
            pLogErro.Controls.Add(label);
        }

        private void verificarSucesso(List<Token> tokens)
        {
            if (!pLogErro.Visible)
            {
                Label label = new Label();

                label.Text = "Código compilado sem erros!";
                label.Location = new Point(8, 8);
                label.AutoSize = true;
                label.Name = "labelSucesso";
                linhaLogErro += 16;
                pLogErro.Controls.Add(label);

                Label lHead = new Label();
                lHead.Text = "Lexema  |  Categoria  |  Linha  |  Coluna";
                lHead.Location = new Point(8, linhaLogErro);
                lHead.AutoSize = true;
                lHead.Name = "labelHead";
                lHead.Font = new Font(lHead.Font, FontStyle.Bold);
                linhaLogErro += 16;
                pLogErro.Controls.Add(lHead);

                for (int i = 0; i < tokens.Count; i++, linhaLogErro += 16)
                {
                    Label l = new Label();

                    l.Text = tokens[i].Lexema + "  |  " + tokens[i].Tipo + "  |  " + tokens[i].Linha + "  |  " + tokens[i].Coluna;
                    l.Location = new Point(8, linhaLogErro); // Posição dentro do Panel
                    l.AutoSize = true; // Ajusta o tamanho ao texto
                    l.Name = "lRelReconhecimento" + linhaLogErro / 16;

                    pLogErro.Controls.Add(l);
                }

                pLogErro.Visible = true;
            }
        }

        public class Parser
        {
            private List<Token> tokens;
            private int pos = 0;

            public List<ErroLexico> erros = new List<ErroLexico>();

            // controla se já está em pânico para evitar vários erros em cascata
            private bool emPanico = false;

            public Parser(List<Token> tokensEntrada)
            {
                tokens = tokensEntrada;
            }

            private Token Atual()
            {
                if (pos >= tokens.Count)
                    return tokens[tokens.Count - 1];

                return tokens[pos];
            }

            private void Avancar()
            {
                if (pos < tokens.Count)
                    pos++;
            }

            private bool Aceitar(TipoToken tipo)
            {
                if (Atual().Tipo == tipo)
                {
                    Avancar();
                    emPanico = false; // se conseguiu casar token, sai do pânico
                    return true;
                }
                return false;
            }

            // MODO PÂNICO
            private bool EhTokenSincronizacao(TipoToken tipo)
            {
                return tipo == TipoToken.VAR ||
                       tipo == TipoToken.IDENTIFICADOR_NOME ||
                       tipo == TipoToken.IDENTIFICADOR_TIPO ||
                       tipo == TipoToken.IF ||
                       tipo == TipoToken.WHILE ||
                       tipo == TipoToken.FOR ||
                       tipo == TipoToken.RETURN ||
                       tipo == TipoToken.ABRE_CHAVE ||
                       tipo == TipoToken.FECHA_CHAVE ||
                       tipo == TipoToken.ABRE_PARENTESE||
                       tipo == TipoToken.FECHA_PARENTESE||
                       tipo == TipoToken.PONTO_VIRGULA ||
                       tipo == TipoToken.EOF;
            }

            private void Sincronizar()
            {
                while (Atual().Tipo != TipoToken.EOF && !EhTokenSincronizacao(Atual().Tipo))
                {
                    Avancar();
                }
                emPanico = false;
            }

            private void RegistrarErro(string mensagem)
            {
                if (!emPanico)
                {
                    erros.Add(new ErroLexico(mensagem, Atual().Linha, Atual().Coluna));
                    emPanico = true;
                }
            }

            private void Esperar(TipoToken tipo, string mensagem)
            {
                if (!Aceitar(tipo))
                {
                    RegistrarErro(mensagem);
                    Sincronizar();
                }
            }

            private bool EstaNoInicioDeStatement(TipoToken tipo)
            {
                return tipo == TipoToken.VAR ||
                       tipo == TipoToken.IDENTIFICADOR_NOME ||
                       tipo == TipoToken.IDENTIFICADOR_TIPO ||
                       tipo == TipoToken.IF ||
                       tipo == TipoToken.WHILE ||
                       tipo == TipoToken.FOR;
            }

            // S = bloco_principal
            public void Parse()
            {
                BlocoPrincipal();

                if (Atual().Tipo != TipoToken.EOF)
                {
                    RegistrarErro("Tokens após o fim do bloco principal");
                    Sincronizar();
                }
            }

            // atribuicao_var = "=" parametro_var ";"
            private void AtribuicaoVar()
            {
                Esperar(TipoToken.IGUAL, "Esperado '=' para iniciar a atribuição");

                if (!emPanico)
                    ParametroVar();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' ao final da atribuição");
            }

            // bloco_identificador_variavel = “VAR” identificador_tipo ":" lista_var ";"
            private void BlocoIdentificadorVariavel()
            {
                Esperar(TipoToken.VAR, "Esperado VAR");

                if (!emPanico)
                    Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo da variável");

                if (!emPanico)
                    Esperar(TipoToken.DOIS_PONTOS, "Esperado ':' após tipo");

                if (!emPanico)
                    ListaVar();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' ao final da declaração de variável");
            }

            // bloco_principal = "BLOCO" identificador_nomes "." bloco_statement
            private void BlocoPrincipal()
            {
                Esperar(TipoToken.BLOCO, "Esperado BLOCO");

                if (!emPanico)
                    Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador do bloco");

                if (!emPanico)
                    Esperar(TipoToken.PONTO, "Esperado '.'");

                if (!emPanico)
                    BlocoStatement();
            }

            // bloco_statement = "{" sequencia_statement [statement_return] "}"
            private void BlocoStatement()
            {
                Esperar(TipoToken.ABRE_CHAVE, "Esperado '{'");

                if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                    SequenciaStatement();

                if (!emPanico && Atual().Tipo == TipoToken.RETURN)
                    StatementReturn();

                Esperar(TipoToken.FECHA_CHAVE, "Esperado '}'");
            }

            // chamada_funcao = "(" [lista_param_funcao] ")" ";"
            private void ChamadaFuncao()
            {
                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' para iniciar chamada");

                if (!emPanico && Atual().Tipo != TipoToken.FECHA_PARENTESE)
                {
                    ListaParamFuncao();
                }

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' para fechar chamada de função");
                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após chamada de função");
            }

            // declaracao = bloco_identificador_variavel
            private void Declaracao()
            {
                BlocoIdentificadorVariavel();
            }

            // declaracao_funcao = identificador_tipo identificador_nomes "(" [lista_param_funcao] ")" bloco_statement
            private void DeclaracaoFuncao()
            {
                Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo de retorno da função");
                if (!emPanico)
                    Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome da função");

                if (!emPanico)
                    Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após o nome da função");

                if (!emPanico && Atual().Tipo == TipoToken.IDENTIFICADOR_TIPO)
                {
                    ListaParamFuncao();
                }

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após os parâmetros");

                if (!emPanico)
                    BlocoStatement();
            }

            // expressao = expressao_simples [operador_relacional expressao_simples]
            private void Expressao()
            {
                ExpressaoSimples();

                if (!emPanico && IsOperadorRelacional(Atual().Tipo))
                {
                    OperadorRelacional();
                    if (!emPanico)
                        ExpressaoSimples();
                }
            }

            // expressao_simples = ["-"] termo {operador_aditivo termo}
            private void ExpressaoSimples()
            {
                if (Atual().Tipo == TipoToken.MENOS)
                {
                    Avancar();
                }

                Termo();

                while (!emPanico && (Atual().Tipo == TipoToken.MAIS || Atual().Tipo == TipoToken.MENOS || Atual().Tipo == TipoToken.OR))
                {
                    OperadorAditivo();
                    if (!emPanico)
                        Termo();
                }
            }

            // fator = numero | identificador_nomes | "(" expressao ")"
            private void Fator()
            {
                if (Atual().Tipo == TipoToken.NUMERO || Atual().Tipo == TipoToken.STRING || Atual().Tipo == TipoToken.BOOLEANO)
                {
                    Avancar();
                }
                else if (Atual().Tipo == TipoToken.IDENTIFICADOR_NOME)
                {
                    Avancar();
                }
                else if (Aceitar(TipoToken.ABRE_PARENTESE))
                {
                    Expressao();
                    Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')'");
                }
                else
                {
                    RegistrarErro("Esperado um valor, identificador ou '('");
                    Sincronizar();
                }
            }

            // lista_param_funcao = identificador_tipo identificador_nomes {"," identificador_tipo identificador_nomes}
            private void ListaParamFuncao()
            {
                Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo do parâmetro");
                if (!emPanico)
                    Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome do parâmetro");

                while (!emPanico && Aceitar(TipoToken.VIRGULA))
                {
                    Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo do parâmetro após ','");
                    if (!emPanico)
                        Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome do parâmetro após o tipo");
                }
            }

            // lista_var = identificador_nomes ["=" parametro_var] {"," identificador_nomes ["=" parametro_var]}
            private void ListaVar()
            {
                VarItem();

                while (!emPanico && Aceitar(TipoToken.VIRGULA))
                {
                    VarItem();
                }
            }

            // operador_aditivo = "+" | "-" | "OR"
            private void OperadorAditivo()
            {
                if (Atual().Tipo == TipoToken.MAIS || Atual().Tipo == TipoToken.MENOS || Atual().Tipo == TipoToken.OR)
                {
                    Avancar();
                }
                else
                {
                    RegistrarErro("Esperado operador aditivo (+, -, OR)");
                    Sincronizar();
                }
            }

            // operador_multiplicativo = "*" | "/" | "AND"
            private void OperadorMultiplicativo()
            {
                if (Atual().Tipo == TipoToken.MULT || Atual().Tipo == TipoToken.DIV || Atual().Tipo == TipoToken.AND)
                {
                    Avancar();
                }
                else
                {
                    RegistrarErro("Esperado operador multiplicativo (*, /, AND)");
                    Sincronizar();
                }
            }

            // operador_relacional = "==" | "<" | "<=" | ">" | ">=" | "!="
            private void OperadorRelacional()
            {
                if (IsOperadorRelacional(Atual().Tipo))
                {
                    Avancar();
                }
                else
                {
                    RegistrarErro("Esperado operador relacional (==, !=, <, >, <=, >=)");
                    Sincronizar();
                }
            }

            private bool IsOperadorRelacional(TipoToken tipo)
            {
                return tipo == TipoToken.IGUAL_IGUAL ||
                       tipo == TipoToken.DIFERENTE ||
                       tipo == TipoToken.MENOR ||
                       tipo == TipoToken.MENOR_IGUAL ||
                       tipo == TipoToken.MAIOR ||
                       tipo == TipoToken.MAIOR_IGUAL;
            }

            // parametro_var = numero | string | booleano | identificador_nomes
            private void ParametroVar()
            {
                if (Atual().Tipo == TipoToken.NUMERO || Atual().Tipo == TipoToken.STRING || Atual().Tipo == TipoToken.BOOLEANO || Atual().Tipo == TipoToken.IDENTIFICADOR_NOME)
                {
                    Avancar();
                }
                else
                {
                    RegistrarErro("Esperado valor");
                    Sincronizar();
                }
            }

            // sequencia_statement = statement {";" statement}
            private void SequenciaStatement()
            {
                while (EstaNoInicioDeStatement(Atual().Tipo))
                {
                    Statement();

                    // se o statement não consumir ';' internamente, tentamos consumir aqui
                    if (Atual().Tipo == TipoToken.PONTO_VIRGULA)
                    {
                        Avancar();
                    }

                    // se caiu em pânico e parou em ';', avança para tentar continuar
                    if (Atual().Tipo == TipoToken.PONTO_VIRGULA)
                    {
                        Avancar();
                    }

                    // se fechou bloco, encerra
                    if (Atual().Tipo == TipoToken.FECHA_CHAVE)
                        break;
                }
            }

            // statement = declaracao | statement_simples | statement_composto
            private void Statement()
            {
                if (Atual().Tipo == TipoToken.VAR)
                {
                    Declaracao();
                }
                else if (Atual().Tipo == TipoToken.IDENTIFICADOR_NOME)
                {
                    StatementSimples();
                }
                else if (Atual().Tipo == TipoToken.IF || Atual().Tipo == TipoToken.WHILE || Atual().Tipo == TipoToken.FOR || Atual().Tipo == TipoToken.IDENTIFICADOR_TIPO)
                {
                    StatementComposto();
                }
                else
                {
                    RegistrarErro("Statement inválido");
                    Sincronizar();

                    if (Atual().Tipo == TipoToken.PONTO_VIRGULA)
                        Avancar();
                }
            }

            // statement_composto = statement_repeticao | statement_condicional | declaracao_funcao
            private void StatementComposto()
            {
                switch (Atual().Tipo)
                {
                    case TipoToken.IF:
                        StatementCondicional();
                        break;
                    case TipoToken.WHILE:
                        StatementWhile();
                        break;
                    case TipoToken.FOR:
                        StatementFor();
                        break;
                    case TipoToken.IDENTIFICADOR_TIPO:
                        DeclaracaoFuncao();
                        break;
                    default:
                        RegistrarErro("Esperado IF, WHILE, FOR ou declaração de função");
                        Sincronizar();
                        break;
                }
            }

            // statement_condicional = statement_if
            private void StatementCondicional()
            {
                StatementIf();
            }

            // statement_for = FOR(...)
            private void StatementFor()
            {
                Esperar(TipoToken.FOR, "Esperado 'FOR'");
                if (!emPanico) Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após FOR");

                if (!emPanico) Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador de inicialização");
                if (!emPanico) Esperar(TipoToken.IGUAL, "Esperado '=' na inicialização do FOR");
                if (!emPanico) Expressao();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após inicialização do FOR");

                if (!emPanico) Expressao();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após condição do FOR");

                if (!emPanico) Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador de incremento");
                if (!emPanico) Esperar(TipoToken.IGUAL, "Esperado '=' no incremento do FOR");
                if (!emPanico) Expressao();

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após cláusulas do FOR");

                if (!emPanico) Esperar(TipoToken.ABRE_CHAVE, "Esperado '{' para iniciar bloco do FOR");

                if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                    SequenciaStatement();

                Esperar(TipoToken.FECHA_CHAVE, "Esperado '}' para fechar bloco do FOR");
            }

            // statement_if = IF(...) { statement } [ELSE { statement }]
            private void StatementIf()
            {
                Esperar(TipoToken.IF, "Esperado 'IF'");
                if (!emPanico) Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após IF");
                if (!emPanico) Expressao();
                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após expressão do IF");

                if (!emPanico) Esperar(TipoToken.ABRE_CHAVE, "Esperado '{' para iniciar bloco do IF");

                if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                    SequenciaStatement();

                Esperar(TipoToken.FECHA_CHAVE, "Esperado '}' para fechar bloco do IF");

                if (Atual().Tipo == TipoToken.ELSE)
                {
                    Avancar();
                    Esperar(TipoToken.ABRE_CHAVE, "Esperado '{' após ELSE");

                    if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                        SequenciaStatement();

                    Esperar(TipoToken.FECHA_CHAVE, "Esperado '}' após fechar bloco do ELSE");
                }
            }

            // statement_repeticao = statement_while | statement_for
            private void StatementRepeticao()
            {
                if (Atual().Tipo == TipoToken.WHILE)
                {
                    StatementWhile();
                }
                else if (Atual().Tipo == TipoToken.FOR)
                {
                    StatementFor();
                }
            }

            // statement_return = "RETURN" parametro_var
            private void StatementReturn()
            {
                Esperar(TipoToken.RETURN, "Esperado RETURN");
                if (!emPanico)
                    ParametroVar();
            }

            // statement_simples = identificador_nomes (atribuicao_var | chamada_funcao)
            private void StatementSimples()
            {
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador no início do statement");

                if (Atual().Tipo == TipoToken.IGUAL)
                {
                    AtribuicaoVar();
                }
                else if (Atual().Tipo == TipoToken.ABRE_PARENTESE)
                {
                    ChamadaFuncao();
                }
                else
                {
                    RegistrarErro("Statement simples inválido");
                    Sincronizar();

                    if (Atual().Tipo == TipoToken.PONTO_VIRGULA)
                        Avancar();
                }
            }

            // statement_while = WHILE(...) { statement }
            private void StatementWhile()
            {
                Esperar(TipoToken.WHILE, "Esperado 'WHILE'");
                if (!emPanico) Esperar(TipoToken.ABRE_PARENTESE, "Esperado '('");

                if (!emPanico) Expressao();

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')'");
                if (!emPanico) Esperar(TipoToken.ABRE_CHAVE, "Esperado '{'");

                if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                    SequenciaStatement();

                Esperar(TipoToken.FECHA_CHAVE, "Esperado '}'");
            }

            // termo = fator {operador_multiplicativo fator}
            private void Termo()
            {
                Fator();

                while (!emPanico && (Atual().Tipo == TipoToken.MULT || Atual().Tipo == TipoToken.DIV || Atual().Tipo == TipoToken.AND))
                {
                    OperadorMultiplicativo();
                    if (!emPanico)
                        Fator();
                }
            }

            // var_item = identificador_nomes ["=" parametro_var]
            private void VarItem()
            {
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome da variável");

                if (!emPanico && Aceitar(TipoToken.IGUAL))
                {
                    ParametroVar();
                }
            }
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

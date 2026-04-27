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
using static AnalisadorLexico.Form1;

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

        public enum TipoToken
        {
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
            public TipoToken Tipo { get; set; }
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
                        atual = codigo[pos];
                        while (pos < codigo.Length - 1 && atual != '"')//ABRE ASPAS DA STRING E LE STRING ATE FECHA ASPAS OU ATE O FIM DO CODIGO
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
                            errosLexicos.Add(new ErroLexico($"String não fechada na linha '{linha}'", linha, coluna));
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

                        case '(':
                            tokens.Add(new Token(TipoToken.ABRE_PARENTESE, "(", linha, coluna));
                            break;

                        case ')':
                            tokens.Add(new Token(TipoToken.FECHA_PARENTESE, ")", linha, coluna));
                            break;

                        case '+':
                            tokens.Add(new Token(TipoToken.MAIS, "+", linha, coluna));
                            break;

                        case '-':
                            tokens.Add(new Token(TipoToken.MENOS, "-", linha, coluna));
                            break;

                        case '*':
                            tokens.Add(new Token(TipoToken.MULT, "*", linha, coluna));
                            break;

                        case '/':
                            tokens.Add(new Token(TipoToken.DIV, "/", linha, coluna));
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
                            errosLexicos.Add(new ErroLexico($"Símbolo inválido '{atual}' na linha '{linha}', coluna '{coluna}'", linha, coluna));
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
                foreach (var erro in lexer.errosLexicos)
                {
                    criarLogErro(erro.Mensagem, erro.Linha, erro.Coluna);
                }
            }
            else
            {
                Parser parser = new Parser(tokens);
                var ast = parser.Parse();

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
                    //verifica erros semanticos
                    AnalisadorSemantico sem = new AnalisadorSemantico();
                    sem.Analisar(ast);
                    if (sem.erros.Count > 0)
                    {
                        foreach (var erroSem in sem.erros)
                            criarLogErro(erroSem.Mensagem, erroSem.Linha, erroSem.Coluna);
                    }
                    else
                    {
                        verificarSucesso(tokens);
                    }
                }
            }
        }

        private void criarLogErro(String mensagem, int linhaErro, int colunaErro)
        {
            Label label = new Label();

            label.Text = "Erro (" + colunaErro + ", " + linhaErro + "): " + mensagem;
            label.Font = new Font("Consolas", 9, FontStyle.Regular);
            label.Location = new Point(8, linhaLogErro); // Posição dentro do Panel
            label.AutoSize = true; // Ajusta o tamanho ao texto
            label.Name = "labelErro" + linhaLogErro / 16;
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


        public abstract class NoAST
        {
            public int Linha { get; set; }
            public int Coluna { get; set; }
        }

        public class NoBlocoPrincipal : NoAST
        {
            public string Nome { get; set; }
            public NoBlocoStatement Corpo { get; set; }
        }

        public class NoBlocoStatement : NoAST
        {
            public List<NoAST> Statements { get; set; } = new List<NoAST>();
            public NoReturn StatementReturn { get; set; }
        }

        public class NoDeclaracaoVar : NoAST
        {
            public string Tipo { get; set; }
            public List<NoVarItem> Variaveis { get; set; } = new List<NoVarItem>();
        }

        public class NoVarItem : NoAST
        {
            public string Nome { get; set; }
            public NoExpressao ValorInicial { get; set; }
        }

        public class NoAtribuicao : NoAST
        {
            public string Nome { get; set; }
            public NoExpressao Valor { get; set; }
        }

        public class NoChamadaFuncao : NoAST
        {
            public string Nome { get; set; }
            public List<NoExpressao> Argumentos { get; set; } = new List<NoExpressao>();
        }

        public class NoDeclaracaoFuncao : NoAST
        {
            public string TipoRetorno { get; set; }
            public string Nome { get; set; }
            public List<NoParametroFuncao> Parametros { get; set; } = new List<NoParametroFuncao>();
            public NoBlocoStatement Corpo { get; set; }
        }

        public class NoParametroFuncao : NoAST
        {
            public string Tipo { get; set; }
            public string Nome { get; set; }
        }

        public class NoIf : NoAST
        {
            public NoExpressao Condicao { get; set; }
            public NoBlocoStatement BlocoThen { get; set; }
            public NoBlocoStatement BlocoElse { get; set; }
        }

        public class NoWhile : NoAST
        {
            public NoExpressao Condicao { get; set; }
            public NoBlocoStatement Corpo { get; set; }
        }

        public class NoFor : NoAST
        {
            public string NomeInicializacao { get; set; }
            public NoExpressao ValorInicializacao { get; set; }

            public NoExpressao Condicao { get; set; }

            public string NomeIncremento { get; set; }
            public NoExpressao ValorIncremento { get; set; }

            public NoBlocoStatement Corpo { get; set; }
        }

        public class NoReturn : NoAST
        {
            public NoExpressao Valor { get; set; }
        }

        public abstract class NoExpressao : NoAST { }

        public class NoNumero : NoExpressao
        {
            public string Valor { get; set; }
        }

        public class NoString : NoExpressao
        {
            public string Valor { get; set; }
        }

        public class NoBoolean : NoExpressao
        {
            public string Valor { get; set; }
        }

        public class NoIdentificador : NoExpressao
        {
            public string Nome { get; set; }
        }

        public class NoBinario : NoExpressao
        {
            public NoExpressao Esquerda { get; set; }
            public string Operador { get; set; }
            public NoExpressao Direita { get; set; }
        }

        public class NoUnario : NoExpressao
        {
            public string Operador { get; set; }
            public NoExpressao Operando { get; set; }
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
                       tipo == TipoToken.ABRE_PARENTESE ||
                       tipo == TipoToken.FECHA_PARENTESE ||
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
            public NoBlocoPrincipal Parse()
            {
                var inicio = BlocoPrincipal();

                if (Atual().Tipo != TipoToken.EOF)
                {
                    RegistrarErro("Tokens após fim do programa");
                    Sincronizar();
                }

                return inicio;
            }

            // atribuicao_var = "=" parametro_var ";"
            private NoAtribuicao AtribuicaoVar(string nome, Token tNome)
            {
                Esperar(TipoToken.IGUAL, "Esperado '=' para iniciar a atribuição");

                NoExpressao valor = Expressao();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' ao final da atribuição");

                return new NoAtribuicao
                {
                    Nome = nome,
                    Valor = valor,
                    Linha = tNome.Linha,
                    Coluna = tNome.Coluna
                };
            }

            // bloco_identificador_variavel = “VAR” identificador_tipo ":" lista_var ";"
            private NoDeclaracaoVar BlocoIdentificadorVariavel()
            {
                Token tVar = Atual();
                Esperar(TipoToken.VAR, "Esperado VAR");

                string tipo = Atual().Lexema;
                Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo da variável");

                Esperar(TipoToken.DOIS_PONTOS, "Esperado ':' após tipo");

                var decl = new NoDeclaracaoVar
                {
                    Tipo = tipo,
                    Linha = tVar.Linha,
                    Coluna = tVar.Coluna
                };

                decl.Variaveis = ListaVar();

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' ao final da declaração de variável");
                return decl;
            }

            // bloco_principal = "BLOCO" identificador_nomes "." bloco_statement
            private NoBlocoPrincipal BlocoPrincipal()
            {
                Token tBloco = Atual();
                Esperar(TipoToken.BLOCO, "Esperado BLOCO");

                string nome = Atual().Lexema;
                Token tNome = Atual();
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador do bloco");

                Esperar(TipoToken.PONTO, "Esperado '.'");

                var bloco = new NoBlocoPrincipal
                {
                    Nome = nome,
                    Linha = tBloco.Linha,
                    Coluna = tBloco.Coluna
                };

                bloco.Corpo = BlocoStatement();
                return bloco;
            }

            // bloco_statement = "{" sequencia_statement [statement_return] "}"
            private NoBlocoStatement BlocoStatement()
            {
                Token tAbre = Atual();
                var bloco = new NoBlocoStatement();

                Esperar(TipoToken.ABRE_CHAVE, "Esperado '{'");

                bloco.Linha = tAbre.Linha;
                bloco.Coluna = tAbre.Coluna;

                if (!emPanico && EstaNoInicioDeStatement(Atual().Tipo))
                    bloco.Statements = SequenciaStatement();

                if (!emPanico && Atual().Tipo == TipoToken.RETURN)
                    bloco.StatementReturn = StatementReturn();

                Esperar(TipoToken.FECHA_CHAVE, "Esperado '}'");
                return bloco;
            }

            // chamada_funcao = "(" [lista_param_funcao] ")" ";"
            private NoChamadaFuncao ChamadaFuncao(string nome, Token tNome)
            {
                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' para iniciar chamada");

                var chamada = new NoChamadaFuncao
                {
                    Nome = nome,
                    Linha = tNome.Linha,
                    Coluna = tNome.Coluna
                };

                if (!emPanico && Atual().Tipo != TipoToken.FECHA_PARENTESE)
                    chamada.Argumentos = ListaArgumentosChamada();

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' para fechar chamada de função");
                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após chamada de função");

                return chamada;
            }
            private List<NoExpressao> ListaArgumentosChamada()
            {
                var args = new List<NoExpressao>();

                args.Add(Expressao());

                while (!emPanico && Aceitar(TipoToken.VIRGULA))
                {
                    args.Add(Expressao());
                }

                return args;
            }
            // declaracao = bloco_identificador_variavel
            private NoDeclaracaoVar Declaracao()
            {
                return BlocoIdentificadorVariavel();
            }

            // declaracao_funcao = identificador_tipo identificador_nomes "(" [lista_param_funcao] ")" bloco_statement
            private NoDeclaracaoFuncao DeclaracaoFuncao()
            {
                string tipoRetorno = Atual().Lexema;
                Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo de retorno da função");

                string nome = Atual().Lexema;
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome da função");

                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após o nome da função");

                var func = new NoDeclaracaoFuncao
                {
                    TipoRetorno = tipoRetorno,
                    Nome = nome
                };

                if (!emPanico && Atual().Tipo != TipoToken.FECHA_PARENTESE)
                {
                    func.Parametros = ListaParamFuncao();
                }

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após os parâmetros");

                func.Corpo = BlocoStatement();

                return func;
            }

            // expressao = expressao_simples [operador_relacional expressao_simples]
            private NoExpressao Expressao()
            {
                NoExpressao esquerda = ExpressaoSimples();

                if (!emPanico && IsOperadorRelacional(Atual().Tipo))
                {
                    Token tOp = Atual();
                    string op = OperadorRelacional();
                    NoExpressao direita = ExpressaoSimples();

                    return new NoBinario
                    {
                        Linha = tOp.Linha,
                        Coluna = tOp.Coluna,
                        Esquerda = esquerda,
                        Operador = op,
                        Direita = direita
                    };
                }

                return esquerda;
            }

            // expressao_simples = ["-"] termo {operador_aditivo termo}
            private NoExpressao ExpressaoSimples()
            {
                bool temMenosUnario = false;
                Token tMenos = Atual();

                if (Atual().Tipo == TipoToken.MENOS)
                {
                    temMenosUnario = true;
                    Avancar();
                }

                NoExpressao atual = Termo();

                if (temMenosUnario)
                {
                    atual = new NoUnario
                    {
                        Linha = tMenos.Linha,
                        Coluna = tMenos.Coluna,
                        Operador = "-",
                        Operando = atual
                    };
                }

                while (!emPanico && (Atual().Tipo == TipoToken.MAIS || Atual().Tipo == TipoToken.MENOS || Atual().Tipo == TipoToken.OR))
                {
                    Token tOp = Atual();
                    string op = OperadorAditivo();
                    NoExpressao direito = Termo();

                    atual = new NoBinario
                    {
                        Linha = tOp.Linha,
                        Coluna = tOp.Coluna,
                        Esquerda = atual,
                        Operador = op,
                        Direita = direito
                    };
                }

                return atual;
            }

            // fator = numero | identificador_nomes | "(" expressao ")"
            private NoExpressao Fator()
            {
                var token = Atual();

                if (Aceitar(TipoToken.NUMERO))
                {
                    return new NoNumero
                    {
                        Valor = token.Lexema,
                        Linha = token.Linha,
                        Coluna = token.Coluna
                    };
                }

                if (Aceitar(TipoToken.STRING))
                {
                    return new NoString
                    {
                        Valor = token.Lexema,
                        Linha = token.Linha,
                        Coluna = token.Coluna
                    };
                }

                if (Aceitar(TipoToken.BOOLEANO))
                {
                    return new NoBoolean
                    {
                        Valor = token.Lexema,
                        Linha = token.Linha,
                        Coluna = token.Coluna
                    };
                }

                if (Aceitar(TipoToken.IDENTIFICADOR_NOME))
                {
                    return new NoIdentificador
                    {
                        Nome = token.Lexema,
                        Linha = token.Linha,
                        Coluna = token.Coluna
                    };
                }

                if (Aceitar(TipoToken.ABRE_PARENTESE))
                {
                    var expr = Expressao();
                    Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')'");
                    return expr;
                }

                RegistrarErro("Esperado um valor, identificador ou '('");
                Sincronizar();

                return new NoNumero { Valor = "0" };
            }

            // lista_param_funcao = identificador_tipo identificador_nomes {"," identificador_tipo identificador_nomes}
            private List<NoParametroFuncao> ListaParamFuncao()
            {
                var lista = new List<NoParametroFuncao>();

                lista.Add(ParametroFuncao());

                while (!emPanico && Aceitar(TipoToken.VIRGULA))
                {
                    lista.Add(ParametroFuncao());
                }

                return lista;
            }

            private NoParametroFuncao ParametroFuncao()
            {
                string tipo = Atual().Lexema;
                Esperar(TipoToken.IDENTIFICADOR_TIPO, "Esperado tipo do parâmetro");

                string nome = Atual().Lexema;
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome do parâmetro");

                return new NoParametroFuncao
                {
                    Tipo = tipo,
                    Nome = nome
                };
            }

            // lista_var = identificador_nomes ["=" parametro_var] {"," identificador_nomes ["=" parametro_var]}
            private List<NoVarItem> ListaVar()
            {
                var lista = new List<NoVarItem>();

                lista.Add(VarItem());

                while (!emPanico && Aceitar(TipoToken.VIRGULA))
                {
                    lista.Add(VarItem());
                }

                return lista;
            }

            // operador_aditivo = "+" | "-" | "OR"
            private string OperadorAditivo()
            {
                if (Atual().Tipo == TipoToken.MAIS || Atual().Tipo == TipoToken.MENOS || Atual().Tipo == TipoToken.OR)
                {
                    string op = Atual().Lexema;
                    Avancar();
                    return op;
                }

                RegistrarErro("Esperado operador aditivo (+, -, OR)");
                Sincronizar();
                return "+";
            }

            // operador_multiplicativo = "*" | "/" | "AND"
            private string OperadorMultiplicativo()
            {
                if (Atual().Tipo == TipoToken.MULT || Atual().Tipo == TipoToken.DIV || Atual().Tipo == TipoToken.AND)
                {
                    string op = Atual().Lexema;
                    Avancar();
                    return op;
                }
                RegistrarErro("Esperado operador multiplicativo (*, /, AND)");
                Sincronizar();
                return "*";
            }

            // operador_relacional = "==" | "<" | "<=" | ">" | ">=" | "!="
            private string OperadorRelacional()
            {
                if (IsOperadorRelacional(Atual().Tipo))
                {
                    string op = Atual().Lexema;
                    Avancar();
                    return op;
                }

                RegistrarErro("Esperado operador relacional");
                Sincronizar();
                return "==";
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
            private NoExpressao ParametroVar()
            {
                if (Atual().Tipo == TipoToken.NUMERO)
                {
                    var t = Atual();
                    Avancar();
                    return new NoNumero { Valor = t.Lexema };
                }

                if (Atual().Tipo == TipoToken.STRING)
                {
                    var t = Atual();
                    Avancar();
                    return new NoString { Valor = t.Lexema };
                }

                if (Atual().Tipo == TipoToken.BOOLEANO)
                {
                    var t = Atual();
                    Avancar();
                    return new NoBoolean { Valor = t.Lexema };
                }

                if (Atual().Tipo == TipoToken.IDENTIFICADOR_NOME)
                {
                    var t = Atual();
                    Avancar();
                    return new NoIdentificador { Nome = t.Lexema };
                }
                else
                {
                    RegistrarErro("Esperado valor");
                    Sincronizar();
                    return new NoNumero { Valor = "0" };
                }
            }

            // sequencia_statement = statement {";" statement}
            private List<NoAST> SequenciaStatement()
            {
                var statements = new List<NoAST>();

                while (EstaNoInicioDeStatement(Atual().Tipo))
                {
                    var stmt = Statement();
                    if (stmt != null)
                        statements.Add(stmt);

                    if (Atual().Tipo == TipoToken.FECHA_CHAVE || Atual().Tipo == TipoToken.RETURN)
                        break;
                }

                return statements;
            }

            // statement = declaracao | statement_simples | statement_composto
            private NoAST Statement()
            {
                if (Atual().Tipo == TipoToken.VAR)
                    return Declaracao();

                if (Atual().Tipo == TipoToken.IDENTIFICADOR_NOME)
                    return StatementSimples();

                if (Atual().Tipo == TipoToken.IF ||
                    Atual().Tipo == TipoToken.WHILE ||
                    Atual().Tipo == TipoToken.FOR ||
                    Atual().Tipo == TipoToken.IDENTIFICADOR_TIPO)
                {
                    return StatementComposto();
                }

                RegistrarErro("Statement inválido");
                Sincronizar();
                return null;
            }

            // statement_composto = statement_repeticao | statement_condicional | declaracao_funcao
            private NoAST StatementComposto()
            {
                switch (Atual().Tipo)
                {
                    case TipoToken.IF:
                        return StatementIf();

                    case TipoToken.WHILE:
                        return StatementWhile();

                    case TipoToken.FOR:
                        return StatementFor();

                    case TipoToken.IDENTIFICADOR_TIPO:
                        return DeclaracaoFuncao();

                    default:
                        RegistrarErro("Esperado IF, WHILE, FOR ou declaração de função");
                        Sincronizar();
                        return null;
                }
            }

            // statement_condicional = statement_if
            private void StatementCondicional()
            {
                StatementIf();
            }

            // statement_for = FOR(...)
            private NoFor StatementFor()
            {
                Token tFor = Atual();
                Esperar(TipoToken.FOR, "Esperado 'FOR'");
                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após FOR");

                string nomeInicial = Atual().Lexema;
                Token tNomeIni = Atual();
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador de inicialização no FOR");
                Esperar(TipoToken.IGUAL, "Esperado '=' na inicialização do FOR");

                var valorInicial = Expressao();
                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após inicialização do FOR");

                var condicao = Expressao();
                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após condição do FOR");

                string nomeIncremento = Atual().Lexema;
                Token tNomeInc = Atual();
                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador de incremento no FOR");
                Esperar(TipoToken.IGUAL, "Esperado '=' no incremento do FOR");

                var valorIncremento = Expressao();
                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após cláusulas do FOR");

                var noFor = new NoFor
                {
                    Linha = tFor.Linha,
                    Coluna = tFor.Coluna,
                    NomeInicializacao = nomeInicial,
                    ValorInicializacao = valorInicial,
                    Condicao = condicao,
                    NomeIncremento = nomeIncremento,
                    ValorIncremento = valorIncremento
                };

                noFor.Corpo = BlocoStatement();
                return noFor;
            }

            // statement_if = IF(...) { statement } [ELSE { statement }]
            private NoIf StatementIf()
            {
                Token tIf = Atual();
                Esperar(TipoToken.IF, "Esperado 'IF'");
                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após IF");

                var noIf = new NoIf
                {
                    Linha = tIf.Linha,
                    Coluna = tIf.Coluna,
                    Condicao = Expressao()
                };

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após expressão do IF");
                noIf.BlocoThen = BlocoStatement();

                if (Atual().Tipo == TipoToken.ELSE)
                {
                    Token tElse = Atual();
                    Avancar();
                    noIf.BlocoElse = BlocoStatement();
                    if (noIf.BlocoElse != null && noIf.BlocoElse.Linha == 0)
                    {
                        noIf.BlocoElse.Linha = tElse.Linha;
                        noIf.BlocoElse.Coluna = tElse.Coluna;
                    }
                }

                return noIf;
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
            private NoReturn StatementReturn()
            {
                Token tReturn = Atual();
                Esperar(TipoToken.RETURN, "Esperado RETURN");

                var noReturn = new NoReturn
                {
                    Linha = tReturn.Linha,
                    Coluna = tReturn.Coluna,
                    Valor = Expressao()
                };

                Esperar(TipoToken.PONTO_VIRGULA, "Esperado ';' após RETURN");
                return noReturn;
            }

            // statement_simples = identificador_nomes (atribuicao_var | chamada_funcao)
            private NoAST StatementSimples()
            {
                Token tNome = Atual();
                string nome = Atual().Lexema;

                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado identificador no início do statement");

                if (Atual().Tipo == TipoToken.IGUAL)
                    return AtribuicaoVar(nome, tNome);

                if (Atual().Tipo == TipoToken.ABRE_PARENTESE)
                    return ChamadaFuncao(nome, tNome);

                RegistrarErro("Statement simples inválido");
                Sincronizar();
                return null;
            }

            // statement_while = WHILE(...) { statement }
            private NoWhile StatementWhile()
            {
                Token tWhile = Atual();
                Esperar(TipoToken.WHILE, "Esperado 'WHILE'");
                Esperar(TipoToken.ABRE_PARENTESE, "Esperado '(' após WHILE");

                var noWhile = new NoWhile
                {
                    Linha = tWhile.Linha,
                    Coluna = tWhile.Coluna,
                    Condicao = Expressao()
                };

                Esperar(TipoToken.FECHA_PARENTESE, "Esperado ')' após expressão do WHILE");
                noWhile.Corpo = BlocoStatement();
                return noWhile;
            }

            // termo = fator {operador_multiplicativo fator}
            private NoExpressao Termo()
            {
                NoExpressao atual = Fator();

                while (!emPanico && (Atual().Tipo == TipoToken.MULT ||
                                     Atual().Tipo == TipoToken.DIV ||
                                     Atual().Tipo == TipoToken.AND))
                {
                    Token tOp = Atual();
                    string op = OperadorMultiplicativo();
                    NoExpressao direito = Fator();

                    atual = new NoBinario
                    {
                        Linha = tOp.Linha,
                        Coluna = tOp.Coluna,
                        Esquerda = atual,
                        Operador = op,
                        Direita = direito
                    };
                }

                return atual;
            }

            // var_item = identificador_nomes ["=" parametro_var]
            private NoVarItem VarItem()
            {
                Token tNome = Atual();
                string nome = Atual().Lexema;

                Esperar(TipoToken.IDENTIFICADOR_NOME, "Esperado nome da variável");

                NoExpressao valor = null;

                if (!emPanico && Aceitar(TipoToken.IGUAL))
                    valor = Expressao();

                return new NoVarItem
                {
                    Nome = nome,
                    ValorInicial = valor,
                    Linha = tNome.Linha,
                    Coluna = tNome.Coluna
                };
            }
        }

        public class Simbolo
        {
            public string Nome { get; set; }
            public string Tipo { get; set; }
            public bool Inicializado { get; set; }
            public bool Utilizado { get; set; }
            public bool EhFuncao { get; set; }
            public int Linha { get; set; }
            public int Coluna { get; set; }

            public List<string> Parametros { get; set; } = new List<string>();
        }

        public class TabelaSimbolos
        {
            private Dictionary<string, Simbolo> tabela = new Dictionary<string, Simbolo>();

            public bool Declarar(Simbolo s)
            {
                if (tabela.ContainsKey(s.Nome))
                    return false;

                tabela[s.Nome] = s;
                return true;
            }

            public Simbolo Obter(string nome)
            {
                return tabela.ContainsKey(nome) ? tabela[nome] : null;
            }

            public IEnumerable<Simbolo> Todos() => tabela.Values;
        }

        public class ErroSemantico
        {
            public string Mensagem { get; set; }
            public int Linha { get; set; }
            public int Coluna { get; set; }

            public ErroSemantico(string msg, int linha, int coluna)
            {
                Mensagem = msg;
                Linha = linha;
                Coluna = coluna;
            }
        }

        public class AnalisadorSemantico
        {
            private TabelaSimbolos tabela = new TabelaSimbolos();
            public List<ErroSemantico> erros = new List<ErroSemantico>();

            public void Analisar(NoBlocoPrincipal raiz)
            {
                AnalisarBloco(raiz.Corpo);
                foreach (var s in tabela.Todos())
                {
                    if (!s.Utilizado && !s.EhFuncao)
                        erros.Add(new ErroSemantico($"Variável '{s.Nome}' declarada mas não utilizada", s.Linha, s.Coluna));
                }
            }

            private void AnalisarBloco(NoBlocoStatement bloco)
            {
                foreach (var stmt in bloco.Statements)
                    AnalisarStatement(stmt);

                if (bloco.StatementReturn != null)
                    AnalisarReturn(bloco.StatementReturn);
            }

            private void AnalisarStatement(NoAST stmt)
            {
                switch (stmt)
                {
                    case NoDeclaracaoVar d:
                        AnalisarDeclaracao(d);
                        break;

                    case NoAtribuicao a:
                        AnalisarAtribuicao(a);
                        break;

                    case NoChamadaFuncao c:
                        AnalisarChamada(c);
                        break;

                    case NoIf i:
                        AnalisarIf(i);
                        break;

                    case NoWhile w:
                        AnalisarWhile(w);
                        break;

                    case NoFor f:
                        AnalisarFor(f);
                        break;

                    case NoDeclaracaoFuncao func:
                        AnalisarFuncao(func);
                        break;
                }
            }

            // =========================
            // DECLARAÇÃO
            // =========================
            private void AnalisarDeclaracao(NoDeclaracaoVar d)
            {
                foreach (var v in d.Variaveis)
                {
                    var simbolo = new Simbolo
                    {
                        Nome = v.Nome,
                        Tipo = d.Tipo,
                        Inicializado = false,
                        Linha = v.Linha,
                        Coluna = v.Coluna
                    };

                    if (!tabela.Declarar(simbolo))
                    {
                        erros.Add(new ErroSemantico($"Variável '{v.Nome}' já declarada", v.Linha, v.Coluna));
                        continue;
                    }

                    if (v.ValorInicial != null)
                    {
                        string tipoExpr = AnalisarExpressao(v.ValorInicial);

                        if (!TiposCompativeis(d.Tipo, tipoExpr))
                            erros.Add(new ErroSemantico($"Tipo incompatível na inicialização de '{v.Nome}'", v.Linha, v.Coluna));

                        simbolo.Inicializado = true;
                    }
                }
            }

            // =========================
            // ATRIBUIÇÃO
            // =========================
            private void AnalisarAtribuicao(NoAtribuicao a)
            {
                var s = tabela.Obter(a.Nome);

                if (s == null)
                {
                    erros.Add(new ErroSemantico($"Variável '{a.Nome}' não declarada", a.Linha, a.Coluna));
                    return;
                }

                string tipoExpr = AnalisarExpressao(a.Valor);

                if (!TiposCompativeis(s.Tipo, tipoExpr))
                    erros.Add(new ErroSemantico($"Tipo incompatível na atribuição para '{a.Nome}'", a.Linha, a.Coluna));

                s.Inicializado = true;
            }

            // =========================
            // EXPRESSÕES
            // =========================
            private string AnalisarExpressao(NoExpressao expr)
            {
                switch (expr)
                {
                    case NoNumero n:
                        return n.Valor.Contains(".") ? "float" : "int";

                    case NoString str:
                        return "String";

                    case NoBoolean boole:
                        return "bool";

                    case NoIdentificador id:
                        var s = tabela.Obter(id.Nome);

                        if (s == null)
                        {
                            erros.Add(new ErroSemantico($"Variável '{id.Nome}' não declarada", expr.Linha, expr.Coluna));
                            return "erro";
                        }

                        if (!s.Inicializado)
                            erros.Add(new ErroSemantico($"Variável '{id.Nome}' usada sem inicialização", expr.Linha, expr.Coluna));

                        s.Utilizado = true;
                        return s.Tipo;

                    case NoBinario b:
                        string t1 = AnalisarExpressao(b.Esquerda);
                        string t2 = AnalisarExpressao(b.Direita);

                        if (!TiposCompativeis(t1, t2))
                            erros.Add(new ErroSemantico($"Tipos incompatíveis em operação '{b.Operador}'", b.Linha, b.Coluna));

                        return ResultadoOperacao(t1, t2);

                    case NoUnario u:
                        return AnalisarExpressao(u.Operando);
                }

                return "erro";
            }

            private bool TiposCompativeis(string t1, string t2)
            {
                if (t1 == t2) return true;

                // casting implícito
                if (t1 == "float" && t2 == "int")
                    return true;

                return false;
            }

            private string ResultadoOperacao(string t1, string t2)
            {
                if (t1 == "float" || t2 == "float")
                    return "float";

                return t1;
            }

            // =========================
            // FUNÇÕES
            // =========================
            private void AnalisarFuncao(NoDeclaracaoFuncao f)
            {
                var simbolo = new Simbolo
                {
                    Nome = f.Nome,
                    Tipo = f.TipoRetorno,
                    EhFuncao = true,
                    Parametros = f.Parametros.Select(p => p.Tipo).ToList()
                };

                if (!tabela.Declarar(simbolo))
                {
                    erros.Add(new ErroSemantico($"Função '{f.Nome}' já declarada", f.Linha, f.Coluna));
                    return;
                }

                // escopo único
                foreach (var p in f.Parametros)
                {
                    tabela.Declarar(new Simbolo
                    {
                        Nome = p.Nome,
                        Tipo = p.Tipo,
                        Inicializado = true
                    });
                }

                AnalisarBloco(f.Corpo);
            }

            private void AnalisarChamada(NoChamadaFuncao c)
            {
                var s = tabela.Obter(c.Nome);

                if (s == null || !s.EhFuncao)
                {
                    erros.Add(new ErroSemantico($"Função '{c.Nome}' não declarada", c.Linha, c.Coluna));
                    return;
                }

                if (c.Argumentos.Count != s.Parametros.Count)
                {
                    erros.Add(new ErroSemantico($"Número de parâmetros incorreto em '{c.Nome}'", c.Linha, c.Coluna));
                    return;
                }

                for (int i = 0; i < c.Argumentos.Count; i++)
                {
                    string tipoArg = AnalisarExpressao(c.Argumentos[i]);

                    if (!TiposCompativeis(s.Parametros[i], tipoArg))
                    {
                        erros.Add(new ErroSemantico($"Tipo inválido no parâmetro {i + 1} de '{c.Nome}'", c.Linha, c.Coluna));
                    }
                }
            }

            // =========================
            // CONTROLE DE FLUXO
            // =========================
            private void AnalisarIf(NoIf i)
            {
                string tipo = AnalisarExpressao(i.Condicao);

                if (tipo != "bool")
                    erros.Add(new ErroSemantico("Condição do IF deve ser booleana", i.Linha, i.Coluna));

                AnalisarBloco(i.BlocoThen);

                if (i.BlocoElse != null)
                    AnalisarBloco(i.BlocoElse);
            }

            private void AnalisarWhile(NoWhile w)
            {
                string tipo = AnalisarExpressao(w.Condicao);

                if (tipo != "bool")
                    erros.Add(new ErroSemantico("Condição do WHILE deve ser booleana", w.Linha, w.Coluna));

                AnalisarBloco(w.Corpo);
            }

            private void AnalisarFor(NoFor f)
            {
                AnalisarExpressao(f.ValorInicializacao);
                AnalisarExpressao(f.Condicao);
                AnalisarExpressao(f.ValorIncremento);

                AnalisarBloco(f.Corpo);
            }

            private void AnalisarReturn(NoReturn r)
            {
                AnalisarExpressao(r.Valor);
            }
        }

        private void btFechar_Click(object sender, EventArgs e)
        {
            pLogErro.Visible = false;
            linhaLogErro = 8;
            for (int i = pLogErro.Controls.Count - 1; i > 0; i--)
                if (pLogErro.Controls[i] is Label)
                {
                    var l = pLogErro.Controls[i];
                    pLogErro.Controls.Remove(l);
                    l.Dispose();
                }
        }
    }
}

namespace AnalisadorLexico
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtbEditor = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnNumbers = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arqMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.btSalvar = new System.Windows.Forms.ToolStripMenuItem();
            this.btSalvarComo = new System.Windows.Forms.ToolStripMenuItem();
            this.btCarregar = new System.Windows.Forms.ToolStripMenuItem();
            this.btLimpar = new System.Windows.Forms.ToolStripMenuItem();
            this.btExecutar = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.labelNome = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbEditor
            // 
            this.rtbEditor.AcceptsTab = true;
            this.rtbEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbEditor.BackColor = System.Drawing.SystemColors.ControlText;
            this.rtbEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbEditor.ForeColor = System.Drawing.SystemColors.Control;
            this.rtbEditor.Location = new System.Drawing.Point(23, 28);
            this.rtbEditor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rtbEditor.Name = "rtbEditor";
            this.rtbEditor.Size = new System.Drawing.Size(578, 336);
            this.rtbEditor.TabIndex = 0;
            this.rtbEditor.Text = "";
            this.rtbEditor.ZoomFactor = 1.2F;
            this.rtbEditor.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Controls.Add(this.labelNome);
            this.panel2.Controls.Add(this.menuStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 28);
            this.panel2.TabIndex = 2;
            // 
            // pnNumbers
            // 
            this.pnNumbers.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.pnNumbers.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnNumbers.Location = new System.Drawing.Point(0, 28);
            this.pnNumbers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnNumbers.Name = "pnNumbers";
            this.pnNumbers.Size = new System.Drawing.Size(26, 338);
            this.pnNumbers.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arqMenu,
            this.btExecutar});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(600, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arqMenu
            // 
            this.arqMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btSalvar,
            this.btSalvarComo,
            this.btCarregar,
            this.btLimpar});
            this.arqMenu.Name = "arqMenu";
            this.arqMenu.Size = new System.Drawing.Size(61, 20);
            this.arqMenu.Text = "Arquivo";
            // 
            // btSalvar
            // 
            this.btSalvar.Name = "btSalvar";
            this.btSalvar.Size = new System.Drawing.Size(180, 22);
            this.btSalvar.Text = "Salvar";
            this.btSalvar.Click += new System.EventHandler(this.btSalvar_Click);
            // 
            // btSalvarComo
            // 
            this.btSalvarComo.Name = "btSalvarComo";
            this.btSalvarComo.Size = new System.Drawing.Size(180, 22);
            this.btSalvarComo.Text = "Salvar Como";
            this.btSalvarComo.Click += new System.EventHandler(this.btSalvarComo_Click);
            // 
            // btCarregar
            // 
            this.btCarregar.Name = "btCarregar";
            this.btCarregar.Size = new System.Drawing.Size(180, 22);
            this.btCarregar.Text = "Carregar";
            this.btCarregar.Click += new System.EventHandler(this.btCarregar_Click);
            // 
            // btLimpar
            // 
            this.btLimpar.Name = "btLimpar";
            this.btLimpar.Size = new System.Drawing.Size(180, 22);
            this.btLimpar.Text = "Limpar";
            this.btLimpar.Click += new System.EventHandler(this.btLimpar_Click);
            // 
            // btExecutar
            // 
            this.btExecutar.Name = "btExecutar";
            this.btExecutar.Size = new System.Drawing.Size(64, 20);
            this.btExecutar.Text = "Executar";
            this.btExecutar.Click += new System.EventHandler(this.btExecutar_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // labelNome
            // 
            this.labelNome.AutoSize = true;
            this.labelNome.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelNome.Location = new System.Drawing.Point(508, 5);
            this.labelNome.Name = "labelNome";
            this.labelNome.Size = new System.Drawing.Size(59, 13);
            this.labelNome.TabIndex = 1;
            this.labelNome.Text = "Sem Nome";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(600, 366);
            this.Controls.Add(this.pnNumbers);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.rtbEditor);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbEditor;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnNumbers;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arqMenu;
        private System.Windows.Forms.ToolStripMenuItem btSalvar;
        private System.Windows.Forms.ToolStripMenuItem btSalvarComo;
        private System.Windows.Forms.ToolStripMenuItem btCarregar;
        private System.Windows.Forms.ToolStripMenuItem btLimpar;
        private System.Windows.Forms.ToolStripMenuItem btExecutar;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label labelNome;
    }
}


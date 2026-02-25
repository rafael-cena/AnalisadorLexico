using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnalisadorLexico
{
    public partial class Form1 : Form
    {
        int index = 1;

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
    }
}

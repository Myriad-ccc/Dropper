using System;
using System.Drawing;
using System.Windows.Forms;

namespace Dropper
{
    public class Card : Button
    {
        public bool On { get; set; } = false;
        public static Card[,] deck;
        public int DeckRow { get; set; }
        public int DeckCol { get; set; }

        private static readonly Color defaultBG = QOL.Colors.SameRGB(35);

        public Card(int deckRow, int deckCol)
        {
            DeckRow = deckRow;
            DeckCol = deckCol;

            BackColor = defaultBG;
            FlatStyle = FlatStyle.Popup;
            Size = new Size(24, 24);
            TabStop = false;
            UseCompatibleTextRendering = true;
            TextAlign = ContentAlignment.TopCenter;
            Font = new Font(QOL.VCROSDMONO, 12f);

            MouseClick += (s, ev) => SetActive();
        }

        public void Toggle()
        {
            On = !On;
            BackColor = On ? Color.CornflowerBlue : defaultBG;
        }

        public static Action<int, int> Activated;
        public void SetActive()
        {
            foreach (Card card in deck)
                if (card.On)
                    card.Toggle();
            Toggle();
            Activated?.Invoke(DeckRow, DeckCol);
        }
    }
}

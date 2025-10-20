namespace Dropper
{
    public class Floor : CustomPanel
    {
        public new int Height { get; set; } = 32;

        public Floor()
        {
            BackColor = QOL.RGB(40);
        }
    }
}

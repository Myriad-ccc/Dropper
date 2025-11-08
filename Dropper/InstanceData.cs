using System.Collections.Generic;
using System.Drawing;

namespace Dropper
{
    public class InstanceData
    {
        public class BlockData
        {
            public RectangleF Bounds { get; set; }
            public bool Active { get; set; }
            public float Weight { get; set; }
            public float OriginalWeight { get; set; }
            public Block.GravityMode Gravity { get; set; }
            public int CrackCount { get; set; }
            public Color ActiveColor { get; set; }
            public Color InactiveColor { get; set; }
            public Color ActiveBorderColor { get; set; }
            public Color InactiveBorderColor { get; set; }
        }

        public class AppState
        {
            //public Point WindowLocation { get; set; } disabling this since its annoying
            //TODO:add currently active toolbar panels
            public int GravityX { get; set; }
            public int GravityY { get; set; }
            public List<BlockData> Blocks { get; set; } = [];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MessingAroundwithXBOX
{
    class Star : Sprite
    {
        Sprite[] stars;

        public Star(Texture2D texture, Vector2 position, Color color, bool visible = true)
            : base(texture, position, color, visible)
        {

        }
    }
}

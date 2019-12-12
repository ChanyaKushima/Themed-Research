using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Games.Object.Visual
{
    public class LogTextBox : TextBlock
    {
        protected override void OnRender(DrawingContext dc)
        {
            DrawTextBlock(dc);
            throw new NotImplementedException();
        }
    }
}

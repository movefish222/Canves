using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using Canves.Core;

namespace Canves{
    public class CanvObject : GObject{
        public List<Color> colors = new List<Color>();
        public bool visal = true;
        virtual public void Render(Graphics g, Vector2 position){
        }
        public CanvObject(){
            parent = new GObject();
        }
    }
}
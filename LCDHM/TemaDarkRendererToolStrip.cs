using System.Drawing;
using System.Windows.Forms;

namespace LCDHM {
    internal class TemaDarkRendererToolStrip : ProfessionalColorTable {
        readonly Color Fundo = Color.FromArgb(255, 27, 27, 28);
        readonly Color Selecionado = Color.FromArgb(255, 51, 51, 52);


        public override Color ToolStripDropDownBackground {
            get {
                return Fundo;
            }
        }

        public override Color ImageMarginGradientBegin {
            get {
                return Fundo;
            }
        }

        public override Color ImageMarginGradientMiddle {
            get {
                return Fundo;
            }
        }

        public override Color ImageMarginGradientEnd {
            get {
                return Fundo;
            }
        }

        public override Color MenuBorder {
            get {
                return Fundo;
            }
        }

        public override Color MenuItemBorder {
            get {
                return Fundo;
            }
        }

        public override Color MenuItemSelected {
            get {
                return Selecionado;
            }
        }

        public override Color MenuStripGradientBegin {
            get {
                return Fundo;
            }
        }

        public override Color MenuStripGradientEnd {
            get {
                return Fundo;
            }
        }

        public override Color MenuItemSelectedGradientBegin {
            get {
                return Selecionado;
            }
        }

        public override Color MenuItemSelectedGradientEnd {
            get {
                return Selecionado;
            }
        }

        public override Color MenuItemPressedGradientBegin {
            get {
                return Fundo;
            }
        }

        public override Color MenuItemPressedGradientEnd {
            get {
                return Fundo;
            }
        }
    }
}
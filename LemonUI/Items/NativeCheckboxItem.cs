using LemonUI.Elements;
using System;
using System.Drawing;

namespace LemonUI.Items
{
    /// <summary>
    /// Rockstar-like checkbox item.
    /// </summary>
    public class NativeCheckboxItem : NativeItem, ICheckboxItem
    {
        #region Fields

        /// <summary>
        /// The image shown on the checkbox.
        /// </summary>
        internal protected ScaledTexture check = null;
        /// <summary>
        /// If this item is checked or not.
        /// </summary>
        private bool checked_ = false;

        #endregion

        #region Properties

        /// <summary>
        /// If this item is checked or not.
        /// </summary>
        public bool Checked
        {
            get => checked_;
            set
            {
                if (checked_ == value)
                {
                    return;
                }
                checked_ = value;
                CheckboxChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the checkbox changes.
        /// </summary>
        public event EventHandler CheckboxChanged;

        #endregion

        #region Constructor

        public NativeCheckboxItem(string title) : this(title, "", false)
        {
        }

        public NativeCheckboxItem(string title, bool check) : this(title, "", check)
        {
        }

        public NativeCheckboxItem(string title, string description) : this(title, description, false)
        {
        }

        public NativeCheckboxItem(string title, string description, bool check) : base(title, description)
        {
            Checked = check;
            this.check = new ScaledTexture(PointF.Empty, SizeF.Empty, "commonmenu", "");
            Activated += Toggle;
        }

        #endregion

        #region Internal Functions

        /// <summary>
        /// Inverts the checkbox activation.
        /// </summary>
        private void Toggle(object sender, EventArgs e) => Checked = !Checked;
        /// <summary>
        /// Updates the texture of the sprite.
        /// </summary>
        internal protected void UpdateTexture(bool selected)
        {
            if (selected)
            {
                check.Texture = Checked ? "shop_box_tickb" : "shop_box_blankb";
            }
            else
            {
                check.Texture = Checked ? "shop_box_tick" : "shop_box_blank";
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Draws the Checkbox on the screen.
        /// </summary>
        public override void Draw()
        {
            base.Draw();
            check.Draw();
        }

        #endregion
    }
}

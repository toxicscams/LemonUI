#if FIVEM
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using Font = CitizenFX.Core.UI.Font;
#elif SHVDN2
using GTA;
using GTA.Native;
using Font = GTA.Font;
#elif SHVDN3
using GTA;
using GTA.Native;
using GTA.UI;
using Font = GTA.UI.Font;
#endif
using LemonUI.Elements;
using LemonUI.Extensions;
using LemonUI.Items;
using LemonUI.Scaleform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace LemonUI.Menus
{
    /// <summary>
    /// Menu that looks like the ones used by Rockstar.
    /// </summary>
    public class NativeMenu : IMenu<NativeItem>, IProcessable
    {
        #region Internal Fields

        /// <summary>
        /// A Pink-Purple used for debugging.
        /// </summary>
        internal static readonly Color colorDebug = Color.FromArgb(255, 0, 255);
        /// <summary>
        /// The White color.
        /// </summary>
        internal static readonly Color colorWhite = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// The White Smoke color.
        /// </summary>
        internal static readonly Color colorWhiteSmoke = Color.FromArgb(245, 245, 245);
        /// <summary>
        /// The Black color.
        /// </summary>
        internal static readonly Color colorBlack = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// The color for disabled items.
        /// </summary>
        internal static readonly Color colorDisabled = Color.FromArgb(163, 159, 148);

        /// <summary>
        /// Sound played when the user selects an option.
        /// </summary>
        internal static readonly Sound.Sound soundSelect = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "SELECT");
        /// <summary>
        /// Sound played when the user returns or closes the menu.
        /// </summary>
        internal static readonly Sound.Sound soundBack = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "BACK");
        /// <summary>
        /// Sound played when the menu goes up and down.
        /// </summary>
        internal static readonly Sound.Sound soundUpDown = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_UP_DOWN");
        /// <summary>
        /// Sound played when the items are moved to the left and right.
        /// </summary>
        internal static readonly Sound.Sound soundLeftRight = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "NAV_LEFT_RIGHT");
        /// <summary>
        /// Sound played when an item is selected.
        /// </summary>
        internal static readonly Sound.Sound soundSelected = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "SELECT");
        /// <summary>
        /// Sound played when an item is disabled.
        /// </summary>
        internal static readonly Sound.Sound soundError = new Sound.Sound("HUD_FRONTEND_DEFAULT_SOUNDSET", "ERROR");

        /// <summary>
        /// The controls required by the menu with both a gamepad and mouse + keyboard.
        /// </summary>
        internal static List<Control> controlsRequired = new List<Control>
        {
            Control.FrontendAccept,
            Control.FrontendAxisX,
            Control.FrontendAxisY,
            Control.FrontendDown,
            Control.FrontendUp,
            Control.FrontendLeft,
            Control.FrontendRight,
            Control.FrontendCancel,
            Control.FrontendSelect,
            Control.CursorScrollDown,
            Control.CursorScrollUp,
            Control.CursorX,
            Control.CursorY,
            Control.MoveUpDown,
            Control.MoveLeftRight,
            Control.Sprint,
            Control.Jump,
            Control.Enter,
            Control.VehicleExit,
            Control.VehicleAccelerate,
            Control.VehicleBrake,
            Control.VehicleMoveLeftRight,
            Control.VehicleFlyYawLeft,
            Control.FlyLeftRight,
            Control.FlyUpDown,
            Control.VehicleFlyYawRight,
            Control.VehicleHandbrake
        };
        /// <summary>
        /// Controls required for the camera to work.
        /// </summary>
        internal static readonly List<Control> controlsCamera = new List<Control>
        {
            Control.LookUpDown,
            Control.LookLeftRight,
        };
        /// <summary>
        /// The controls required in a gamepad.
        /// </summary>
        internal static readonly List<Control> controlsGamepad = new List<Control>
        {
            Control.Aim,
            Control.Attack
        };

        #endregion

        #region Constant fields

        /// <summary>
        /// The height of the menu subtitle background.
        /// </summary>
        internal const float subtitleHeight = 38;
        /// <summary>
        /// The height of one of the items in the screen.
        /// </summary>
        internal const float itemHeight = 37.4f;
        /// <summary>
        /// The height difference between the description and the end of the items.
        /// </summary>
        internal const float heightDiffDescImg = 4;
        /// <summary>
        /// The height difference between the background and text of the description.
        /// </summary>
        internal const float heightDiffDescTxt = 3;
        /// <summary>
        /// The X position of the description text.
        /// </summary>
        internal const float posXDescTxt = 6;
        /// <summary>
        /// The offset to the X value of the item title.
        /// </summary>
        internal const float itemOffsetX = 6;
        /// <summary>
        /// The offset to the Y value of the item title.
        /// </summary>
        internal const float itemOffsetY = 3;

        #endregion

        #region Private Fields

        /// <summary>
        /// If the menu is visible or not.
        /// </summary>
        private bool visible = false;
        /// <summary>
        /// The index of the selected item in the menu.
        /// </summary>
        private int index = -1;
        /// <summary>
        /// The width of the menu itself.
        /// </summary>
        private float width = 433;
        /// <summary>
        /// The alignment of the menu.
        /// </summary>
        private Alignment alignment = Alignment.Left;
        /// <summary>
        /// The banner of the menu.
        /// </summary>
        private IScreenDrawable bannerImage = null;
        /// <summary>
        /// The text of the menu.
        /// </summary>
        private ScaledText bannerText = null;
        /// <summary>
        /// The background of the drawable text.
        /// </summary>
        private IScreenDrawable subtitleImage = null;
        /// <summary>
        /// The text of the subtitle.
        /// </summary>
        private ScaledText subtitleText = null;
        /// <summary>
        /// The image on the background of the menu.
        /// </summary>
        private ScaledTexture backgroundImage = null;
        /// <summary>
        /// The rectangle that shows the currently selected item.
        /// </summary>
        private ScaledTexture selectedRect = null;
        /// <summary>
        /// The rectangle with the description text.
        /// </summary>
        private ScaledTexture descriptionRect = null;
        /// <summary>
        /// The text with the description text.
        /// </summary>
        private ScaledText descriptionText = null;
        /// <summary>
        /// The instructional buttons for the menu.
        /// </summary>
        private InstructionalButtons buttons = null;
        /// <summary>
        /// The maximum allowed number of items in the menu at once.
        /// </summary>
        private int maxItems = 10;
        /// <summary>
        /// The first item in the menu.
        /// </summary>
        private int firstItem = 0;
        /// <summary>
        /// A specific menu to open during the next tick.
        /// </summary>
        private NativeMenu openNextTick = null;

        #endregion

        #region Private Properties

        /// <summary>
        /// The items that are visible and useable on the screen.
        /// </summary>
        private IEnumerable<NativeItem> VisibleItems
        {
            get
            {
                // Iterate over the number of items while staying under the maximum
                for (int i = 0; i < MaxItems; i++)
                {
                    // Calculate the start of our items
                    int start = firstItem + i;

                    // If the number of items is over the ones in the list, something went wrong
                    // TODO: Decide what to do in this case (exception? silently ignore?)
                    if (start >= Items.Count)
                    {
                        break;
                    }

                    // Otherwise, return it as part of the iterator
                    yield return Items[start];
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// If the menu is visible on the screen.
        /// </summary>
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible == value)
                {
                    return;
                }
                visible = value;
                if (value)
                {
                    soundSelect.PlayFrontend();
                    Shown?.Invoke(this, EventArgs.Empty);
                    TriggerSelectedItem();
                }
                else
                {
                    Close();
                }
            }
        }
        /// <summary>
        /// The title of the menu.
        /// </summary>
        public string Title
        {
            get
            {
                if (bannerText == null)
                {
                    throw new NullReferenceException("The Text parameter of this Menu is null.");
                }
                return bannerText.Text;
            }
            set
            {
                bannerText.Text = value;
            }
        }
        /// <summary>
        /// The banner shown at the top of the menu.
        /// </summary>
        public IScreenDrawable Banner
        {
            get => bannerImage;
            set
            {
                bannerImage = value;
                Recalculate();
                RecalculateTexts();
            }
        }
        /// <summary>
        /// The text object on top of the banner.
        /// </summary>
        public ScaledText Text
        {
            get => bannerText;
            set => bannerText = value;
        }
        /// <summary>
        /// Returns the currently selected item.
        /// </summary>
        public NativeItem SelectedItem
        {
            get
            {
                // If there are no items, return null
                if (Items.Count == 0)
                {
                    return null;
                }
                // Otherwise, return the correct item from the list
                else
                {
                    return Items[SelectedIndex];
                }
            }
        }
        /// <summary>
        /// The current index of the menu.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                if (Items == null || Items.Count == 0)
                {
                    return -1;
                }
                return index;
            }
            set
            {
                // If the list of items is empty, don't allow the user to set the index
                if (Items == null || Items.Count == 0)
                {
                    throw new InvalidOperationException("There are no items in this menu.");
                }
                // If the value is over or equal than the number of items, raise an exception
                else if (value >= Items.Count)
                {
                    throw new InvalidOperationException($"The index is over {Items.Count - 1}");
                }

                // Calculate the bounds of the menu
                int lower = firstItem;
                int upper = firstItem + maxItems;

                // Time to set the first item based on the total number of items
                // If the item is between the allowed values, do nothing because we are on the correct first item
                if (value >= lower && value < upper - 1)
                {
                }
                // If the upper bound + 1 equals the new index, increase it by one
                else if (upper == value)
                {
                    firstItem += 1;
                }
                // If the first item minus one equals the value, decrease it by one
                else if (lower - 1 == value)
                {
                    firstItem -= 1;
                }
                // Otherwise, set it somewhere
                else
                {
                    // If the value is under the max items, set it to zero
                    if (value < maxItems)
                    {
                        firstItem = 0;
                    }
                    // Otherwise, set it at the bottom
                    else
                    {
                        firstItem = value - maxItems + 1;
                    }
                }

                // Save the index
                index = value;

                // If the menu is visible, play the up and down sound
                if (Visible)
                {
                    soundUpDown.PlayFrontend();
                }

                // If an item was selected, set the description and trigger it
                if (SelectedItem != null)
                {
                    descriptionText.Text = SelectedItem.Description;
                    TriggerSelectedItem();
                }
                // Otherwise, set the generic description text
                else
                {
                    descriptionText.Text = NoItemsText;
                }

                // And update the items visually
                UpdateItems();
            }
        }
        /// <summary>
        /// The width of the menu.
        /// </summary>
        public float Width
        {
            get => width;
            set
            {
                width = value;
                Recalculate();
            }
        }
        /// <summary>
        /// The alignment of the menu.
        /// </summary>
        public Alignment Alignment
        {
            get => alignment;
            set
            {
                alignment = value;
                UpdateAlignment();
            }
        }
        /// <summary>
        /// The subtitle of the menu.
        /// </summary>
        public string Subtitle
        {
            get => subtitleText.Text;
            set => subtitleText.Text = value;
        }
        /// <summary>
        /// If the mouse should be used for navigating the menu.
        /// </summary>
        public bool UseMouse { get; set; } = true;
        /// <summary>
        /// The items that this menu contain.
        /// </summary>
        public List<NativeItem> Items { get; }
        /// <summary>
        /// Text shown when there are no items in the menu.
        /// </summary>
        public string NoItemsText { get; set; } = "There are no items available";
        /// <summary>
        /// The maximum allowed number of items in the menu at once.
        /// </summary>
        public int MaxItems
        {
            get => maxItems;
            set
            {
                // If the number is under one, raise an exception
                if (value < 1)
                {
                    throw new InvalidOperationException("The maximum numbers on the screen can't be under 1.");
                }
                // Otherwise, save it
                maxItems = value;
            }
        }
        /// <summary>
        /// If this menu should be aware of the Safe Zone.
        /// </summary>
        public bool SafeZoneAware { get; set; } = true;
        /// <summary>
        /// The instructional buttons shown in the bottom right.
        /// </summary>
        public InstructionalButtons Buttons => buttons;
        /// <summary>
        /// The parent menu of this menu.
        /// </summary>
        public NativeMenu Parent { get; set; } = null;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the menu is opened and shown to the user.
        /// </summary>
        public event EventHandler Shown;
        /// <summary>
        /// Event triggered when the menu starts closing.
        /// </summary>
        public event CancelEventHandler Closing;
        /// <summary>
        /// Event triggered when the menu finishes closing.
        /// </summary>
        public event EventHandler Closed;
        /// <summary>
        /// Event triggered when the index has been changed.
        /// </summary>
        public event SelectedEventHandler SelectedIndexChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new menu with the default banner texture.
        /// </summary>
        /// <param name="title">The title of the menu.</param>
        /// <param name="subtitle">Subtitle of this menu.</param>
        public NativeMenu(string title, string subtitle) : this(title, new ScaledTexture(PointF.Empty, new SizeF(0, 108), "commonmenu", "interaction_bgd"), subtitle)
        {
        }

        /// <summary>
        /// Creates a new menu with the specified banner texture.
        /// </summary>
        /// <param name="title">The title of the menu.</param>
        /// <param name="banner">The drawable to use as the banner.</param>
        /// <param name="subtitle">Subtitle of this menu.</param>
        public NativeMenu(string title, IScreenDrawable banner, string subtitle)
        {
            Items = new List<NativeItem>();
            bannerImage = banner;
            bannerText = new ScaledText(PointF.Empty, title, 1.02f, Font.HouseScript)
            {
                Color = colorWhite,
                Alignment = Alignment.Center
            };
            subtitleImage = new ScaledRectangle(PointF.Empty, SizeF.Empty)
            {
                Color = Color.FromArgb(0, 0, 0)
            };
            subtitleText = new ScaledText(PointF.Empty, subtitle, 0.345f, Font.ChaletLondon)
            {
                Color = colorWhiteSmoke
            };
            backgroundImage = new ScaledTexture(PointF.Empty, SizeF.Empty, "commonmenu", "gradient_bgd");
            selectedRect = new ScaledTexture(PointF.Empty, SizeF.Empty, "commonmenu", "gradient_nav");
            descriptionRect = new ScaledTexture(PointF.Empty, SizeF.Empty, "commonmenu", "gradient_bgd");
            descriptionText = new ScaledText(PointF.Empty, "", 0.351f);
            buttons = new InstructionalButtons(new InstructionalButton("Select", Control.PhoneSelect), new InstructionalButton("Back", Control.PhoneCancel))
            {
                Visible = true
            };
            Recalculate();
        }

        #endregion

        #region Tools

        /// <summary>
        /// Triggers the Selected event for the current item.
        /// </summary>
        private void TriggerSelectedItem()
        {
            // Get the currently selected item
            NativeItem item = SelectedItem;

            // If is null or the menu is closed, return
            if (item == null || !Visible)
            {
                return;
            }

            // Otherwise, trigger the selected event for this menu
            SelectedEventArgs args = new SelectedEventArgs(index, index - firstItem);
            SelectedItem.OnSelected(this, args);
            SelectedIndexChanged?.Invoke(this, args);
        }
        /// <summary>
        /// Updates the positions of the items.
        /// </summary>
        private void UpdateItems()
        {
            // Store the start positions for the X and Y axis
            float startX = alignment == Alignment.Right ? 1f.ToXAbsolute() - width : 0;
            float startY = 0;
            // Add the heights of the banner and subtitle (if they are there)
            if (bannerImage != null)
            {
                startY += bannerImage.Size.Height;
            }
            if (subtitleImage != null && !string.IsNullOrWhiteSpace(subtitleText.Text))
            {
                startY += subtitleImage.Size.Height;
            }

            // Set the position of the rectangle that marks the current item
            selectedRect.Position = new PointF(selectedRect.Position.X, startY + ((index - firstItem) * itemHeight));
            // And then do the description background and text
            float description = startY + ((Items.Count > maxItems ? maxItems : Items.Count) * itemHeight) + heightDiffDescImg;
            descriptionRect.Size = new SizeF(width, descriptionText.LineCount * 35);
            descriptionRect.Position = new PointF(descriptionRect.Position.X, description);
            descriptionText.Position = new PointF(startX + posXDescTxt, description + heightDiffDescTxt);

            // Calculate the start position for the item objects (checkbox for items, text and arrows for lists)
            float itemObjX = startX + width;

            // Iterate over the number of items while counting the number of them
            int i = 0;
            foreach (NativeItem item in VisibleItems)
            {
                // If this item is a checkbox
                if (item is NativeCheckboxItem checkbox)
                {
                    // Update the texture to show the selected and not selected design
                    checkbox.UpdateTexture(item == SelectedItem);
                    // And set the position and size of the checkbox
                    checkbox.check.Position = new PointF(itemObjX - 50, startY - 6);
                    checkbox.check.Size = new SizeF(50, 50);
                }
                // If this item is a slidable
                if (item is NativeSlidableItem slidable)
                {
                    // Set the sizes of the arrows
                    slidable.arrowLeft.Size = item == SelectedItem ? new SizeF(30, 30) : SizeF.Empty;
                    slidable.arrowRight.Size = item == SelectedItem ? new SizeF(30, 30) : SizeF.Empty;
                    // And set the positions of the left arrow
                    slidable.arrowRight.Position = new PointF(itemObjX - 35, startY + 4);
                }
                // If this item is a list
                if (item is NativeListItem list)
                {
                    // Set the color of the text
                    list.text.Color = item == SelectedItem ? colorBlack : colorWhiteSmoke;
                    // And set the position of the left arrow and text
                    float textWidth = list.arrowRight.Size.Width;
                    list.text.Position = new PointF(itemObjX - textWidth - 1 - list.text.Width, startY + 3);
                    list.arrowLeft.Position = new PointF(list.text.Position.X - list.arrowLeft.Size.Width, startY + 4);
                }

                // Convert it to a relative value
                item.title.Position = new PointF(startX + itemOffsetX, startY + itemOffsetY);
                // And select the correct color (just in case)
                Color color = colorWhiteSmoke;

                // If this matches the currently selected item
                if (i + firstItem == SelectedIndex)
                {
                    // Set the correct color to black
                    color = colorBlack;
                }
                // Set the color of the item
                item.title.Color = color;

                // Finally, increase the count by one and move to the next item
                i++;
                startY += itemHeight;
            }
        }
        /// <summary>
        /// Recalculates the text labels.
        /// </summary>
        public void RecalculateTexts()
        {
            float offset = alignment == Alignment.Right ? 1f.ToXAbsolute() - width : 0;
            float start = 0;

            if (bannerImage != null)
            {
                start += bannerImage.Size.Height;
            }

            if (bannerText != null)
            {
                bannerText.Position = new PointF(offset + 209, 22);
            }
            if (subtitleText != null)
            {
                start += 4.2f;
                subtitleText.Position = new PointF(offset + 6, start);
            }
        }
        /// <summary>
        /// Processes the button presses.
        /// </summary>
        private void ProcessControls()
        {
            // Start by disabling all of the controls
            Controls.DisableAll(2);
            // And enable only those required at all times and specific situations
            Controls.EnableThisFrame(controlsRequired);
            if (Controls.IsUsingController)
            {
                Controls.EnableThisFrame(controlsGamepad);
            }
            if (Controls.IsUsingController || !UseMouse)
            {
                Controls.EnableThisFrame(controlsCamera);
            }

            // Check if the controls necessary were pressed
            bool backPressed = Controls.IsJustPressed(Control.PhoneCancel) || Controls.IsJustPressed(Control.FrontendPause);
            bool upPressed = Controls.IsJustPressed(Control.PhoneUp) || Controls.IsJustPressed(Control.CursorScrollUp);
            bool downPressed = Controls.IsJustPressed(Control.PhoneDown) || Controls.IsJustPressed(Control.CursorScrollDown);
            bool selectPressed = Controls.IsJustPressed(Control.FrontendAccept) || Controls.IsJustPressed(Control.PhoneSelect);
            bool leftPressed = Controls.IsJustPressed(Control.PhoneLeft);
            bool rightPressed = Controls.IsJustPressed(Control.PhoneRight);

            // If the player pressed the back button, go back or close the menu
            if (backPressed)
            {
                Back();
                return;
            }

            // If the player pressed up, go to the previous item
            if (upPressed && !downPressed)
            {
                Previous();
                return;
            }
            // If he pressed down, go to the next item
            else if (downPressed && !upPressed)
            {
                Next();
                return;
            }

            // If the player pressed the left or right button, trigger the event and sound
            if (SelectedItem is ISlidableItem slidable)
            {
                if (leftPressed)
                {
                    if (SelectedItem.Enabled)
                    {
                        slidable.GoLeft();
                        soundLeftRight.PlayFrontend();
                        return;
                    }
                    else
                    {
                        soundError.PlayFrontend();
                        return;
                    }
                }
                if (rightPressed)
                {
                    if (SelectedItem.Enabled)
                    {
                        slidable.GoRight();
                        soundLeftRight.PlayFrontend();
                        return;
                    }
                    else
                    {
                        soundError.PlayFrontend();
                        return;
                    }
                }
            }

            // If the player is using the mouse controls and pressed select
            if (UseMouse)
            {
                // Enable the mouse cursor
#if FIVEM
                API.SetMouseCursorActiveThisFrame();
#elif SHVDN2
                Function.Call(Hash._SHOW_CURSOR_THIS_FRAME);
#elif SHVDN3
                Function.Call(Hash._SET_MOUSE_CURSOR_ACTIVE_THIS_FRAME);
#endif

                // If the select button was not pressed, return
                if (!selectPressed)
                {
                    return;
                }

                // Iterate over the items while keeping the index
                int i = 0;
                foreach (NativeItem item in VisibleItems)
                {
                    // If this is a list item and the user pressed the right arrow
                    if (item is NativeSlidableItem slidable1 && Resolution.IsCursorInBounds(slidable1.arrowRight.Position, slidable1.arrowRight.Size))
                    {
                        // If the item is enabled, move to the right
                        if (item.Enabled)
                        {
                            slidable1.GoRight();
                            soundLeftRight.PlayFrontend();
                            return;
                        }
                        // Otherwise, play the error sound
                        else
                        {
                            soundError.PlayFrontend();
                            return;
                        }
                    }
                    // If this is a list item and the user pressed the left arrow
                    else if (item is NativeSlidableItem slidable2 && Resolution.IsCursorInBounds(slidable2.arrowRight.Position, slidable2.arrowRight.Size))
                    {
                        // If the item is enabled, move to the left
                        if (item.Enabled)
                        {
                            slidable2.GoLeft();
                            soundLeftRight.PlayFrontend();
                            return;
                        }
                        // Otherwise, play the error sound
                        else
                        {
                            soundError.PlayFrontend();
                            return;
                        }
                    }
                    // If the user selected somewhere in the item area
                    else if (Resolution.IsCursorInBounds(item.title.Position.X - itemOffsetX, item.title.Position.Y - itemOffsetY, Width, itemHeight))
                    {
                        if (item == SelectedItem)
                        {
                            // If is enabled, activate it and play the select sound
                            if (item.Enabled)
                            {
                                item.OnActivated(this);
                                soundSelect.PlayFrontend();
                                if (item is NativeCheckboxItem checkboxItem)
                                {
                                    checkboxItem.UpdateTexture(true);
                                }
                                return;
                            }
                            // Otherwise, play the error sound
                            else
                            {
                                soundError.PlayFrontend();
                                return;
                            }
                        }
                        // Otherwise, change the index
                        else
                        {
                            SelectedIndex = firstItem + i;
                            soundUpDown.PlayFrontend();
                            return;
                        }
                    }

                    // Finally, increase the number
                    i++;
                }
            }

            // If the player selected an item
            if (selectPressed)
            {
                // If there is an item selected and is enabled, trigger it and play the selected sound
                if (SelectedItem != null && SelectedItem.Enabled)
                {
                    SelectedItem.OnActivated(this);
                    soundSelect.PlayFrontend();
                    if (SelectedItem is NativeCheckboxItem check)
                    {
                        check.UpdateTexture(true);
                    }
                    return;
                }
                // Otherwise, play the error sound
                else
                {
                    soundError.PlayFrontend();
                    return;
                }
            }
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Adds an item onto the menu.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(NativeItem item)
        {
            // If the item is already on the list, raise an exception
            if (Items.Contains(item))
            {
                throw new InvalidOperationException("The item is already part of the menu.");
            }
            // Also raise an exception if is null
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Otherwise, just add it
            Items.Add(item);
            // Set the correct index if this is the only item
            if (Items.Count != 0 && SelectedIndex == -1)
            {
                SelectedIndex = 0;
            }
            // And recalculate the positions
            Recalculate();
        }
        /// <summary>
        /// Adds a specific menu as a submenu with an item.
        /// </summary>
        /// <param name="menu">The menu to add.</param>
        /// <returns>The item that points to the submenu.</returns>
        public NativeItem AddSubMenu(NativeMenu menu)
        {
            // If the menu is null, raise an exception
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            // Create a new menu item
            NativeItem item = new NativeItem(menu.Subtitle);
            // Add the activated event
            item.Activated += (sender, e) =>
            {
                openNextTick = menu;
            };
            // Set this menu as the parent of the other one
            menu.Parent = this;
            // Add the item to this menu
            Add(item);
            // And return the new item
            return item;
        }
        /// <summary>
        /// Removes an item from the menu.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(NativeItem item)
        {
            // If the item is not in the menu, ignore it
            if (!Items.Contains(item))
            {
                return;
            }

            // Remove it if there
            // If not, ignore it
            Items.Remove(item);
            // If the index is higher or equal than the max number of items
            // Set the max - 1
            if (SelectedIndex >= Items.Count)
            {
                SelectedIndex = Items.Count - 1;
            }
        }
        /// <summary>
        /// Removes the items that match the predicate.
        /// </summary>
        /// <param name="pred">The function to use as a check.</param>
        public void Remove(Func<NativeItem, bool> pred)
        {
            // Create a copy of the list with the existing items and iterate them in order
            List<NativeItem> items = new List<NativeItem>(Items);
            foreach (NativeItem item in items)
            {
                // If the predicate says that this item should be removed, do it
                if (pred(item))
                {
                    Items.Remove(item);
                }
            }
            // Once we finish, make sure that the index is under the limit
            // Set it to the max if it does
            if (SelectedIndex >= Items.Count)
            {
                SelectedIndex = Items.Count - 1;
            }
        }
        /// <summary>
        /// Checks if an item is part of the menu.
        /// </summary>
        /// <param name="item">The item to check.</param>
        public bool Contains(NativeItem item) => Items.Contains(item);
        /// <summary>
        /// Draws the menu and handles the controls.
        /// </summary>
        public void Process()
        {
            // If the menu is not visible, return
            if (!visible)
            {
                return;
            }

            // If there is a submenu to open, show it and return
            if (openNextTick != null)
            {
                Visible = false;
                openNextTick.visible = true;
                openNextTick = null;
            }

            // Otherwise, draw the elements
            // Start by setting the alignment for the UI Elements
            // This is to make the UI appear in the correct position in non standard aspect ratios
            // For example: 21:6, 32:9, 11:4, 48:9 (3 16:9 in Surround) and more
            const int L = 76;
            const int T = 84;
            if (SafeZoneAware)
            {
#if FIVEM
                API.SetScriptGfxAlign(L, T);
                API.SetScriptGfxAlignParams(0, 0, 0, 0);
#elif SHVDN2
                Function.Call(Hash._SET_SCREEN_DRAW_POSITION, L, T);
                Function.Call(Hash._0xF5A2C681787E579D, 0, 0, 0, 0);
#elif SHVDN3
                Function.Call(Hash.SET_SCRIPT_GFX_ALIGN, L, T);
                Function.Call(Hash.SET_SCRIPT_GFX_ALIGN_PARAMS, 0, 0, 0, 0);
#endif
            }

            // Start with the banner
            if (bannerImage != null)
            {
                // Draw the background image and text if is present
                bannerImage.Draw();
                if (bannerText != null && !string.IsNullOrWhiteSpace(bannerText.Text))
                {
                    bannerText.Draw();
                }
            }
            // Continue with the subtitle image and text
            subtitleImage?.Draw();
            subtitleText?.Draw();
            // The background of the items
            backgroundImage?.Draw();

            // Then go for the visible items
            foreach (NativeItem item in VisibleItems)
            {
                item.Draw();
            }

            // If this menu has items, draw the rectangle for the selected item
            if (Items.Count > 0)
            {
                selectedRect?.Draw();
            }

            // If the selected item is a checkbox or a list, draw the elements again on top of the selection rectangle
            if (SelectedItem is NativeCheckboxItem checkbox)
            {
                checkbox.check.Draw();
            }
            if (SelectedItem is NativeSlidableItem slidable)
            {
                slidable.arrowLeft.Draw();
                slidable.arrowRight.Draw();
            }
            if (SelectedItem is NativeListItem list)
            {
                list.text.Draw();
            }

            // If the description rectangle and text is there and we have text to draw
            if (descriptionRect != null && descriptionText != null && !string.IsNullOrWhiteSpace(descriptionText.Text))
            {
                // Draw the text and the background
                descriptionRect.Draw();
                descriptionText.Draw();
            }

            // Time to work on the controls!
            ProcessControls();

            // Reset the alignment if this menu should be aware of the safe zone
            if (SafeZoneAware)
            {
#if FIVEM
                API.ResetScriptGfxAlign();
#elif SHVDN2
                Function.Call(Hash._0xE3A3DB414A373DAB);
#elif SHVDN3
                Function.Call(Hash.RESET_SCRIPT_GFX_ALIGN);
#endif
            }
            // And finish by drawing the instructional buttons
            buttons?.Draw();
        }
        /// <summary>
        /// Updates the alignment of the items.
        /// This will automatically recalculate the sizes and positions.
        /// </summary>
        public void UpdateAlignment()
        {
            if (bannerImage != null)
            {
                bannerImage.Alignment = alignment;
            }
            if (subtitleImage != null)
            {
                subtitleImage.Alignment = alignment;
            }
            if (backgroundImage != null)
            {
                backgroundImage.Alignment = alignment;
            }
            if (selectedRect != null)
            {
                selectedRect.Alignment = alignment;
            }
            if (descriptionRect != null)
            {
                descriptionRect.Alignment = alignment;
            }
            RecalculateTexts();
            UpdateItems();
        }
        /// <summary>
        /// Calculates the positions and sizes of the elements.
        /// </summary>
        public void Recalculate()
        {
            // Save the start of the X value
            float start = 0;

            // If there is a banner and is an element
            if (bannerImage != null && bannerImage is BaseElement bannerImageBase)
            {
                // Get the height of the banner
                float bannerHeight = bannerImageBase.Size.Height;
                // Set the position and size
                bannerImageBase.literalPosition = PointF.Empty;
                bannerImageBase.literalSize = new SizeF(width, bannerHeight);
                // Increaser the start location of the next item
                start += bannerHeight;
                bannerImageBase.Recalculate();
            }
            // If there is a subtitle image
            if (subtitleImage != null && subtitleImage is BaseElement subtitleImageBase)
            {
                // Set the position and start
                subtitleImageBase.literalPosition = new PointF(0, start);
                subtitleImageBase.literalSize = new SizeF(width, subtitleHeight);
                // Increase the start location
                start += subtitleHeight;
                subtitleImageBase.Recalculate();
            }
            // If there is a background image
            if (backgroundImage != null)
            {
                // See if we should draw the total number of items or the max allowed
                int count = Items.Count > maxItems ? maxItems : Items.Count;
                // Set the position and size
                backgroundImage.literalPosition = new PointF(0, start);
                backgroundImage.literalSize = new SizeF(width, itemHeight * count);
                backgroundImage.Recalculate();
            }
            // If there is a rectangle for the currently selected item
            if (selectedRect != null)
            {
                // Set the size
                selectedRect.literalPosition = new PointF(0, start);
                selectedRect.literalSize = new SizeF(width, itemHeight);
                selectedRect.Recalculate();
            }
            // If there is a description rectangle
            if (descriptionRect != null)
            {
                // Set the size
                descriptionRect.Size = new SizeF(width, 0);
            }
            // If there is an object for the description
            if (descriptionText != null)
            {
                // Set the width
                descriptionText.Position = new PointF(posXDescTxt, 0);
                // And set the correct word wrap
                descriptionText.WordWrap = width - posXDescTxt;
            }

            RecalculateTexts();
            UpdateItems();
        }
        /// <summary>
        /// Returns to the previous menu or closes the existing one.
        /// </summary>
        public void Back()
        {
            // Try to close the menu
            Close();
            // If this menu has been closed and there is a parent menu, open it
            if (!Visible && Parent != null)
            {
                Parent.Visible = true;
            }
        }
        /// <summary>
        /// Closes the menu.
        /// </summary>
        public void Close()
        {
            // Create a new set of event arguments
            CancelEventArgs args = new CancelEventArgs();
            // And trigger the event
            Closing?.Invoke(this, args);

            // If we need to cancel the closure of the menu, return
            if (args.Cancel)
            {
                return;
            }
            // Otherwise, close the menu
            visible = false;
            Closed?.Invoke(this, EventArgs.Empty);
            soundBack.PlayFrontend();
        }
        /// <summary>
        /// Moves to the previous item.
        /// Does nothing if the menu has no items.
        /// </summary>
        public void Previous()
        {
            // If there are no items, return
            if (Items.Count == 0)
            {
                return;
            }

            // If we are on the first item, go back to the last one
            if (SelectedIndex == 0)
            {
                SelectedIndex = Items.Count - 1;
            }
            // Otherwise, reduce it by one
            else
            {
                SelectedIndex -= 1;
            }
        }
        /// <summary>
        /// Moves to the next item.
        /// Does nothing if the menu has no items.
        /// </summary>
        public void Next()
        {
            // If there are no items, return
            if (Items.Count == 0)
            {
                return;
            }

            // If we are on the last item, go back to the first one
            if (Items.Count - 1 == SelectedIndex)
            {
                SelectedIndex = 0;
            }
            // Otherwise, increase it by one
            else
            {
                SelectedIndex += 1;
            }
        }

        #endregion
    }
}

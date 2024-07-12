using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Kronosta.TypableBooks
{
    internal class BookWriteMenu : IClickableMenu
    {
        public List<TextBox> lines;
        public List<ClickableComponent> linesCC;
        public ClickableTextureComponent saveButton;
        public ClickableTextureComponent signButton;
        public ClickableTextureComponent nextPage;
        public ClickableTextureComponent previousPage;
        public int page;
        public Item saveTo;
        public Dictionary<int, string> pages = new Dictionary<int, string>();
        public int maxPage;
        public static int numLines = 8;
        public BookWriteMenu(Item saveTo)
        {
            this.saveTo = saveTo;
            this.page = 0;
            this.maxPage = 0;
            ResetComponents();
            if (saveTo.modData.ContainsKey($"{ModEntry.ModID}/BookContents")) LoadFromItem();
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
        }

        public void ResetComponents()
        {
            var linesArr = new TextBox[numLines];
            var linesCCArr = new ClickableComponent[numLines];
            for (int i = 0; i < numLines; i++)
            {
                linesArr[i] = new TextBox(
                    ModEntry.SHelper.ModContent.Load<Texture2D>("assets/textBox.png"),
                    null,
                    Game1.smallFont,
                    Color.Black)
                {
                    X = xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth,
                    Y = yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64 + (64 * i),
                    Width = 768,
                    Text = "",
                };
                linesCCArr[i] = new ClickableComponent(new Rectangle(
                    xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth,
                    yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16 + 64 + (64 * i), 800, 48), "")
                {
                    myID = 510 + i,
                    upNeighborID = -99998,
                    leftNeighborID = -99998,
                    rightNeighborID = -99998,
                    downNeighborID = -99998
                };
            }
            lines = linesArr.ToList();
            linesCC = linesCCArr.ToList();
            saveButton = new ClickableTextureComponent("OK", new Rectangle(
                xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth,
                yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f)
            {
                myID = 505,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            //Duck feather sprite
            signButton = new ClickableTextureComponent("Save", new Rectangle(
                xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 64,
                yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 64, 64), null, null, Game1.objectSpriteSheet, Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 444, 16, 16), 4f)
            {
                myID = 506,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            previousPage = new ClickableTextureComponent("Previous", new Rectangle(
                xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 128,
                yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
            {
                myID = 507,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
            nextPage = new ClickableTextureComponent("Next", new Rectangle(
                xPositionOnScreen + 64 + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + 192,
                yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - 16, 64, 64), null, null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
            {
                myID = 508,
                upNeighborID = -99998,
                leftNeighborID = -99998,
                rightNeighborID = -99998,
                downNeighborID = -99998
            };
        }

        public override void receiveKeyPress(Keys key)
        {
            bool accumulator = false;
            for (int i = 0; i < numLines; i++)
                accumulator = accumulator || lines[i].Selected;
            if (!accumulator && !Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                base.receiveKeyPress(key);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            for (int i = 0; i < numLines; i++)
            {
                if (linesCC[i].containsPoint(x, y))
                {
                    for (int j = 0; j < numLines; j++)
                        lines[j].Selected = false;
                    lines[i].SelectMe();
                    return;
                }
            }
            if (saveButton.containsPoint(x, y))
            {
                SaveToItem();
                Game1.exitActiveMenu();
                return;
            }
            else if (nextPage.containsPoint(x, y))
            {
                pages[page] = lines
                    .Select(textBox => textBox.Text)
                    .Aggregate((t1, t2) => t1 + "[[LB]]" + t2);
                page++;
                for (int i = 0; i < numLines; i++)
                {
                    lines[i].Text = "";
                    lines[i].Selected = false;
                }
                if (page > 99) page = 99;
                bool newPage = false;
                if (page > maxPage)
                {
                    maxPage = page;
                    newPage = true;
                }
                if (!newPage)
                {
                    string[] pageLines = pages[page].Split("[[LB]]");
                    for (int i = 0; i < numLines; i++)
                        lines[i].Text = pageLines[i];
                }
                return;
            }
            else if (previousPage.containsPoint(x, y))
            {
                pages[page] = lines
                    .Select(textBox => textBox.Text)
                    .Aggregate((t1, t2) => t1 + "[[LB]]" + t2);
                page--;
                for (int i = 0; i < numLines; i++)
                {
                    lines[i].Text = "";
                    lines[i].Selected = false;
                }
                if (page < 0) page = 0;
                string[] pageLines = pages[page].Split("[[LB]]");
                for (int i = 0; i < numLines; i++)
                    lines[i].Text = pageLines[i];
                return;
            }
            else if (signButton.containsPoint(x, y))
            {
                SaveToItem();
                saveTo.ItemId = $"{ModEntry.ModID}_JournalSigned";
                if (saveTo.QualifiedItemId.StartsWith("(O)"))
                    ((StardewValley.Object)saveTo).displayNameFormat = $"[LocalizedText Strings\\Objects:{ModEntry.ModID}_JournalSigned-Name [EscapedText {Game1.player.Name}] [EscapedText {Game1.season.ToString()} {Game1.dayOfMonth}, Year {Game1.year}]]";
                Game1.exitActiveMenu();
            }
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            if (!Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }
            for (int i = 0; i < numLines; i++)
                lines[i].Draw(b);
            saveButton.draw(b);
            signButton.draw(b);
            previousPage.draw(b);
            nextPage.draw(b);
            b.DrawString(Game1.dialogueFont, $"{page + 1} of {maxPage + 1}",
                new Vector2(nextPage.bounds.Right + 64, nextPage.bounds.Top),
                Color.White);
            drawMouse(b);
        }

        public void SaveToItem()
        {
            pages[page] = lines
                    .Select(textBox => textBox.Text)
                    .Aggregate((t1, t2) => t1 + "[[LB]]" + t2);
            saveTo.modData[$"{ModEntry.ModID}/BookContents"] = pages
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Value)
                .Aggregate((t1, t2) => t1 + "[[PB]]" + t2);
        }

        public void LoadFromItem()
        {
            string[] pagesData = saveTo.modData[$"{ModEntry.ModID}/BookContents"].Split("[[PB]]");
            maxPage = pagesData.Length - 1;
            for (int i = 0; i < pagesData.Length; i++)
            {
                pages[i] = pagesData[i];
            }
            string[] linesData = pages[0].Split("[[LB]]");
            for (int i = 0; i < numLines && i < linesData.Length; i++)
            {
                lines[i].Text = linesData[i];
            }
        }
    }
}

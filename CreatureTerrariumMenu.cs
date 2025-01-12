using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace NatureInTheValley
{
	// Token: 0x02000017 RID: 23
	public class CreatureTerrariumMenu : InventoryMenu
	{
		// Token: 0x06000123 RID: 291 RVA: 0x0000C568 File Offset: 0x0000A768
		public CreatureTerrariumMenu() : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2, false, null, new InventoryMenu.highlightThisItem(CreatureTerrariumMenu.CheckDonated), Game1.player.Items.Count, Math.Max(1, Game1.player.Items.Count / 12), 0, 0, true)
		{
			base.initializeUpperRightCloseButton();
			this.exitFunction = delegate()
			{
				CreatureTerrariumMenu.ExitFunction(this.DonationCompletionType);
			};
			this.title = NatureInTheValleyEntry.staticHelper.Translation.Get("InsectariumC5");
			this.showGrayedOutSlots = true;
			base.initializeUpperRightCloseButton();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000289B File Offset: 0x00000A9B
		public static void ExitFunction(int d)
		{
			if (d == 0)
			{
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("IvyTerrariumMessage")));
				return;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000C618 File Offset: 0x0000A818
		public static bool CheckDonated(Item item)
		{
			string text;
			return item.Category == -81 && item.ItemId.Contains("NatInValley.Creature.") && Game1.currentLocation.modData.TryGetValue("NatureInTheValley/Donated/" + item.ItemId.Split("NatInValley.Creature.", StringSplitOptions.None)[1], out text);
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000C674 File Offset: 0x0000A874
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			Item itemAt = base.getItemAt(x, y);
			if (itemAt == null)
			{
				base.exitThisMenu(true);
				return;
			}
			if (!CreatureTerrariumMenu.CheckDonated(itemAt) || !Game1.player.couldInventoryAcceptThisItem(new Terrarium("Tera." + itemAt.ItemId, Vector2.Zero)))
			{
				return;
			}
			Game1.playSound("newArtifact", null);
			this.DonationCompletionType = 1;
			Item item = itemAt;
			int stack = item.Stack;
			item.Stack = stack - 1;
			if (itemAt.Stack <= 0)
			{
				Game1.player.removeItemFromInventory(itemAt);
			}
			Game1.player.addItemToInventory(new Terrarium("Tera." + itemAt.ItemId, Vector2.Zero));
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000C728 File Offset: 0x0000A928
		public override void draw(SpriteBatch b)
		{
			if (Game1.options.showMenuBackground)
			{
				base.drawBackground(b);
			}
			else
			{
				b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
			}
			Game1.drawDialogueBox(this.xPositionOnScreen - 64, this.yPositionOnScreen - 128, this.width + 128, this.height + 176, false, true, null, false, true, -1, -1, -1);
			SpriteText.drawStringWithScrollCenteredAt(b, this.title, this.xPositionOnScreen - 64 + (this.width + 128) / 2, this.yPositionOnScreen - 128 - 25, SpriteText.getWidthOfString(this.title, 999999) + 16, 1f, null, 0, 0.88f, false);
			base.draw(b);
			base.drawMouse(b, false, -1);
		}

		// Token: 0x04000085 RID: 133
		private int DonationCompletionType;

		// Token: 0x04000086 RID: 134
		private string title;
	}
}

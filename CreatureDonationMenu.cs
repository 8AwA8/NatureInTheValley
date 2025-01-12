using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;

namespace NatureInTheValley
{
	// Token: 0x0200000A RID: 10
	public class CreatureDonationMenu : InventoryMenu
	{
		// Token: 0x060000AB RID: 171 RVA: 0x00009A10 File Offset: 0x00007C10
		public CreatureDonationMenu() : base(Game1.viewport.Width / 2 - 384, Game1.viewport.Height / 2, false, null, new InventoryMenu.highlightThisItem(CreatureDonationMenu.CheckDonated), Game1.player.Items.Count, Math.Max(1, Game1.player.Items.Count / 12), 0, 0, true)
		{
			base.initializeUpperRightCloseButton();
			this.exitFunction = delegate()
			{
				CreatureDonationMenu.ExitFunction(this.DonationCompletionType);
			};
			this.showGrayedOutSlots = true;
			base.initializeUpperRightCloseButton();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00009AA0 File Offset: 0x00007CA0
		public static void ExitFunction(int d)
		{
			switch (d)
			{
			case 0:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("NoDonationResponse")));
				return;
			case 1:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("CommonDonationResponse")));
				return;
			case 2:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("UncommonDonationResponse")));
				return;
			case 3:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("RareDonationResponse")));
				return;
			case 4:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("VeryRareDonationResponse")));
				return;
			case 5:
				Game1.DrawDialogue(new Dialogue(Game1.currentLocation.getCharacterFromName("IvyInsectarium"), "ENGL", NatureInTheValleyEntry.staticHelper.Translation.Get("MythicDonationResponse")));
				return;
			default:
				return;
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x00009C1C File Offset: 0x00007E1C
		public static bool CheckDonated(Item item)
		{
			string text;
			return item.Category == -81 && item.ItemId.Contains("NatInValley.Creature.") && !Game1.currentLocation.modData.TryGetValue("NatureInTheValley/Donated/" + item.ItemId.Split("NatInValley.Creature.", StringSplitOptions.None)[1], out text);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00009C78 File Offset: 0x00007E78
		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			Item itemAt = base.getItemAt(x, y);
			if (itemAt == null)
			{
				base.exitThisMenu(true);
				return;
			}
			string text;
			if (!itemAt.ItemId.Contains("NatInValley.Creature.") || Game1.currentLocation.modData.TryGetValue("NatureInTheValley/Donated/" + itemAt.ItemId.Split("NatInValley.Creature.", StringSplitOptions.None)[1], out text))
			{
				return;
			}
			Game1.playSound("newArtifact", null);
			this.DonationCompletionType = Math.Max(this.DonationCompletionType, this.GetRarityCode(itemAt.getDescription()));
			Game1.Multiplayer.globalChatInfoMessageEvenInSinglePlayer("NITVDonate", new string[]
			{
				Game1.player.Name,
				itemAt.DisplayName
			});
			Item item = itemAt;
			int stack = item.Stack;
			item.Stack = stack - 1;
			Game1.currentLocation.modData["NatureInTheValley/Donated/" + itemAt.ItemId.Split("NatInValley.Creature.", StringSplitOptions.None)[1]] = "true";
			if (itemAt.Stack <= 0)
			{
				Game1.player.removeItemFromInventory(itemAt);
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00009D90 File Offset: 0x00007F90
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
			SpriteText.drawStringWithScrollCenteredAt(b, NatureInTheValleyEntry.staticHelper.Translation.Get("DonationTitle"), this.xPositionOnScreen - 64 + (this.width + 128) / 2, this.yPositionOnScreen - 128 - 25, SpriteText.getWidthOfString(NatureInTheValleyEntry.staticHelper.Translation.Get("DonationTitle"), 999999) + 16, 1f, null, 0, 0.88f, false);
			base.draw(b);
			base.drawMouse(b, false, -1);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00009EB0 File Offset: 0x000080B0
		public int GetRarityCode(string Desc)
		{
			if (Desc.Contains(NatureInTheValleyEntry.staticHelper.Translation.Get("Rarity.4")))
			{
				return 5;
			}
			if (Desc.Contains(NatureInTheValleyEntry.staticHelper.Translation.Get("Rarity.3")))
			{
				return 4;
			}
			if (Desc.Contains(NatureInTheValleyEntry.staticHelper.Translation.Get("Rarity.2")))
			{
				return 3;
			}
			if (Desc.Contains(NatureInTheValleyEntry.staticHelper.Translation.Get("Rarity.1")))
			{
				return 2;
			}
			return 1;
		}

		// Token: 0x0400004D RID: 77
		private int DonationCompletionType;
	}
}

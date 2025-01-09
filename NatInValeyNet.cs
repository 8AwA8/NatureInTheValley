using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x02000007 RID: 7
	[XmlType("Mods_NatInValeyNet")]
	[XmlRoot(ElementName = "natInValeyNet", Namespace = "")]
	[KnownType(typeof(NatInValeyNet))]
	[XmlInclude(typeof(NatInValeyNet))]
	public class NatInValeyNet : Tool
	{
		// Token: 0x06000064 RID: 100 RVA: 0x0000243A File Offset: 0x0000063A
		protected override Item GetOneNew()
		{
			return new NatInValeyNet();
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00002441 File Offset: 0x00000641
		protected override string loadDisplayName()
		{
			return this.helper.Translation.Get("NetName");
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000245D File Offset: 0x0000065D
		public override string getDescription()
		{
			return this.helper.Translation.Get("NetDescript");
		}

		// Token: 0x06000067 RID: 103 RVA: 0x0000847C File Offset: 0x0000667C
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			spriteBatch.Draw(NatInValeyNet.texture, location + new Vector2(32f, 32f), new Rectangle?(new Rectangle(0, 0, 16, 16)), Color.White * transparency, 0f, new Vector2(8f, 8f), (float)(4.0 * (double)scaleSize), SpriteEffects.None, layerDepth);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000084EC File Offset: 0x000066EC
		public override void actionWhenStopBeingHeld(Farmer who)
		{
			base.actionWhenStopBeingHeld(who);
			who.canMove = true;
			this.isHeld = false;
			if (who.UsingTool)
			{
				who.UsingTool = false;
				if (who.FarmerSprite.PauseForSingleAnimation)
				{
					who.FarmerSprite.PauseForSingleAnimation = false;
				}
			}
			if (who.hasBuff("NatCSpeedN"))
			{
				who.buffs.Remove("NatCSpeedN");
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00008554 File Offset: 0x00006754
		public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
		{
			who.jitterStrength = 1f;
			who.canMove = true;
			this.save = 0;
			this.isHeld = true;
			who.forceCanMove();
			if (!who.hasBuff("NatCSpeedN"))
			{
				who.applyBuff(new Buff("NatCSpeedN", null, null, 20000, null, -1, null, null, null, null)
				{
					effects = 
					{
						Speed = 
						{
							Value = -((float)who.Speed + who.buffs.Speed - 1.5f)
						}
					},
					millisecondsDuration = 20000
				});
			}
			return false;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00002479 File Offset: 0x00000679
		public virtual int salePrice()
		{
			return -1;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000085F8 File Offset: 0x000067F8
		public override bool onRelease(GameLocation location, int x, int y, Farmer who)
		{
			who.stopJittering();
			this.isHeld = false;
			who.canMove = false;
			who.jitterStrength = 0f;
			who.UsingTool = false;
			if (who.hasBuff("NatCSpeedN"))
			{
				who.buffs.Remove("NatCSpeedN");
			}
			who.canReleaseTool = false;
			switch (who.FacingDirection)
			{
			case 0:
				((FarmerSprite)who.Sprite).animateOnce(295, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(0, 0, who);
				break;
			case 1:
				((FarmerSprite)who.Sprite).animateOnce(296, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(1, 0, who);
				break;
			case 2:
				((FarmerSprite)who.Sprite).animateOnce(297, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(2, 0, who);
				break;
			case 3:
				((FarmerSprite)who.Sprite).animateOnce(298, 27f, 8, delegate(Farmer f)
				{
					this.EndOfUse(f);
				});
				this.Update(3, 0, who);
				break;
			}
			return false;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x0000874C File Offset: 0x0000694C
		public override void tickUpdate(GameTime time, Farmer who)
		{
			if (!this.isHeld || !who.hasBuff("NatCSpeedN"))
			{
				who.stopJittering();
				return;
			}
			who.jitterStrength = 0.5f;
			switch (who.FacingDirection)
			{
			case 0:
				who.FarmerSprite.setCurrentSingleFrame(36, 32000, false, false);
				return;
			case 1:
				who.FarmerSprite.setCurrentSingleFrame(48, 100, false, false);
				return;
			case 2:
				who.FarmerSprite.setCurrentSingleFrame(66, 32000, false, false);
				return;
			case 3:
				who.FarmerSprite.setCurrentSingleFrame(48, 100, false, true);
				return;
			default:
				return;
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x0000247C File Offset: 0x0000067C
		private void EndOfUse(Farmer who)
		{
			who.canMove = true;
			this.isHeld = false;
			who.jitterStrength = 0f;
			NatureInTheValleyEntry.TryCatch(who);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x0000249D File Offset: 0x0000069D
		public override bool canBeTrashed()
		{
			return true;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000249D File Offset: 0x0000069D
		public override bool canBeDropped()
		{
			return true;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000087EC File Offset: 0x000069EC
		public NatInValeyNet()
		{
			base.ItemId = "NIVNet";
			this.helper = NatureInTheValleyEntry.staticHelper;
			NatInValeyNet.texture = this.helper.ModContent.Load<Texture2D>("PNGs\\NormalNet");
			base.Category = -99;
			this.Name = "NatValleyNet";
			this.Stack = 1;
		}

		// Token: 0x04000040 RID: 64
		public static Texture2D texture;

		// Token: 0x04000041 RID: 65
		private IModHelper helper;

		// Token: 0x04000042 RID: 66
		public bool isHeld;

		// Token: 0x04000043 RID: 67
		private int save;

		// Token: 0x04000044 RID: 68
		public int variant;
	}
}

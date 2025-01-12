using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace NatureInTheValley
{
	// Token: 0x02000016 RID: 22
	[XmlType("Mods_Terrarium")]
	[XmlRoot(ElementName = "Terrarium", Namespace = "")]
	[KnownType(typeof(Terrarium))]
	[XmlInclude(typeof(Terrarium))]
	public class Terrarium : Furniture
	{
		// Token: 0x06000110 RID: 272 RVA: 0x0000BC88 File Offset: 0x00009E88
		public Terrarium(string itemId, Vector2 tile) : base(itemId, tile)
		{
			this.data = NatureInTheValleyEntry.staticCreatureData[itemId.Split("Tera.NatInValley.Creature.", StringSplitOptions.None)[1]];
			this.Sprite = new AnimatedSprite(this.data[15]);
			this.Sprite.SpriteHeight = 32;
			this.Sprite.SpriteWidth = 32;
			this.Sprite.framesPerAnimation = int.Parse(this.data[14]);
			this.defaultBoundingBox.Value = new Rectangle(this.defaultBoundingBox.X, this.defaultBoundingBox.Y, this.defaultBoundingBox.Width * 2, this.defaultBoundingBox.Height);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000BD5C File Offset: 0x00009F5C
		public override void actionOnPlayerEntryOrPlacement(GameLocation environment, bool dropDown)
		{
			this.data = NatureInTheValleyEntry.staticCreatureData[this.itemId.Value.Split("Tera.NatInValley.Creature.", StringSplitOptions.None)[1]];
			this.Sprite = new AnimatedSprite(this.data[15]);
			this.Sprite.SpriteHeight = 32;
			this.Sprite.SpriteWidth = 32;
			this.Sprite.framesPerAnimation = int.Parse(this.data[14]);
			base.actionOnPlayerEntryOrPlacement(environment, dropDown);
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000273C File Offset: 0x0000093C
		protected override Item GetOneNew()
		{
			return new Terrarium(base.ItemId, this.tileLocation.Value);
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000BDE8 File Offset: 0x00009FE8
		public override void updateWhenCurrentLocation(GameTime time)
		{
			GameLocation currentLocation = Game1.currentLocation;
			GameLocation location = this.Location;
			if (this.GetData()[6] != "true")
			{
				if (base.name.Contains("Chameleon"))
				{
					if (Game1.random.NextDouble() >= 0.15)
					{
						return;
					}
					this.Sprite.AnimateDown(time, 0, "");
				}
				else
				{
					this.Sprite.AnimateDown(time, 0, "");
				}
			}
			else if (!this.left)
			{
				this.xOffset -= 5f * float.Parse(this.GetData()[2]);
				this.Sprite.AnimateDown(time, 0, "");
				if (this.xOffset < -18f)
				{
					this.left = true;
				}
			}
			else
			{
				this.xOffset += 5f * float.Parse(this.GetData()[2]);
				this.Sprite.AnimateRight(time, 0, "");
				if (this.xOffset > 18f)
				{
					this.left = false;
				}
			}
			base.updateWhenCurrentLocation(time);
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00002754 File Offset: 0x00000954
		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
		{
			return base.placementAction(location, x, y, who);
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00002761 File Offset: 0x00000961
		public float GetGlassDrawLayer()
		{
			return this.GetBaseDrawLayer() + 0.0001f;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000BF18 File Offset: 0x0000A118
		public float GetBaseDrawLayer()
		{
			if (this.furniture_type.Value == 12)
			{
				return 2E-09f;
			}
			return (float)(this.boundingBox.Value.Bottom - ((this.furniture_type.Value == 6 || this.furniture_type.Value == 13) ? 48 : 8)) / 10000f;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000BF78 File Offset: 0x0000A178
		public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
		{
			Vector2 zero = Vector2.Zero;
			if (this.isTemporarilyInvisible)
			{
				return;
			}
			Vector2 vector = this.drawPosition.Value;
			if (!Furniture.isDrawingLocationFurniture)
			{
				vector = new Vector2((float)x, (float)y) * 64f;
				vector.Y -= (float)(this.sourceRect.Height * 4 - this.boundingBox.Height);
			}
			if (NatureInTheValleyEntry.staticConfig.useTerrariumWallpapers)
			{
				spriteBatch.Draw(this.GetBackBackTexture(), Game1.GlobalToLocal(Game1.viewport, vector + zero), new Rectangle?(new Rectangle(0, 0, 128, 128)), Color.White * alpha, 0f, Vector2.Zero, 1f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer() - 0.0002f);
			}
			spriteBatch.Draw(this.GetBackTexture(), Game1.GlobalToLocal(Game1.viewport, vector + zero), new Rectangle?(new Rectangle(0, 0, 128, 128)), Color.White * alpha, 0f, Vector2.Zero, 1f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer() - 0.0001f);
			spriteBatch.Draw(this.GetAddedTexture(), Game1.GlobalToLocal(Game1.viewport, vector + zero), new Rectangle?(new Rectangle(0, 0, 128, 128)), Color.White * alpha, 0f, Vector2.Zero, 1f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer() - 1E-06f);
			if (this.Sprite != null)
			{
				this.Sprite.draw(spriteBatch, Game1.GlobalToLocal(Game1.viewport, vector) + new Vector2(this.xOffset, (this.GetData()[1] == "true") ? 30f : 25f) + new Vector2(32f, 32f) * 1f / Math.Min(float.Parse(this.GetData()[4]), 2.7f) + new Vector2((float)this.Sprite.SpriteWidth / 2f, 0f), this.GetGlassDrawLayer(), 0, 0, Color.White * alpha, false, Math.Min(float.Parse(this.GetData()[4]), 2.7f), 0f, false);
			}
			else
			{
				this.Sprite = new AnimatedSprite(this.GetData()[15]);
				this.Sprite.SpriteHeight = 32;
				this.Sprite.SpriteWidth = 32;
				this.Sprite.framesPerAnimation = int.Parse(this.GetData()[14]);
				this.defaultBoundingBox.Value = new Rectangle(this.defaultBoundingBox.X, this.defaultBoundingBox.Y, this.defaultBoundingBox.Width * 2, this.defaultBoundingBox.Height);
			}
			spriteBatch.Draw(this.GetFrontTexture(), Game1.GlobalToLocal(Game1.viewport, vector + zero), new Rectangle?(new Rectangle(0, 0, 128, 128)), Color.White * alpha, 0f, Vector2.Zero, 1f, this.flipped.Value ? SpriteEffects.FlipHorizontally : SpriteEffects.None, this.GetGlassDrawLayer() + 0.0001f);
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000C318 File Offset: 0x0000A518
		public List<string> GetData()
		{
			if (this.data == null || this.data.Count < 1)
			{
				this.data = NatureInTheValleyEntry.staticCreatureData[this.itemId.Value.Split("Tera.NatInValley.Creature.", StringSplitOptions.None)[1]];
			}
			return this.data;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000276F File Offset: 0x0000096F
		public Texture2D GetFrontTexture()
		{
			if (this.FrontTexture == null)
			{
				this.FrontTexture = NatureInTheValleyEntry.staticHelper.ModContent.Load<Texture2D>("PNGs\\Terrarium");
			}
			return this.FrontTexture;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00002799 File Offset: 0x00000999
		public Texture2D GetBackTexture()
		{
			if (this.bgTexture == null)
			{
				this.bgTexture = NatureInTheValleyEntry.staticHelper.ModContent.Load<Texture2D>("PNGs\\Base" + this.GetLocation());
			}
			return this.bgTexture;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x000027CE File Offset: 0x000009CE
		public Terrarium()
		{
		}

		// Token: 0x0600011C RID: 284 RVA: 0x000027E8 File Offset: 0x000009E8
		public Texture2D GetAddedTexture()
		{
			if (this.AddedTexture == null)
			{
				this.AddedTexture = NatureInTheValleyEntry.staticHelper.ModContent.Load<Texture2D>("PNGs\\Added" + this.GetLocal());
			}
			return this.AddedTexture;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000C36C File Offset: 0x0000A56C
		private string GetLocation()
		{
			if (this.GetData()[11].Contains("3"))
			{
				return "3";
			}
			if (this.GetData()[11].Contains("4"))
			{
				return "4";
			}
			if (this.GetData()[11].Contains("2"))
			{
				return "2";
			}
			return "1";
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000C3DC File Offset: 0x0000A5DC
		private string GetLocal()
		{
			if (this.GetData()[17] == "1" && (this.GetLocation() == "2" || this.GetLocation() == "4"))
			{
				return "5";
			}
			if (this.GetData()[17].Contains("0"))
			{
				return "0";
			}
			if (this.GetData()[17].Contains("3"))
			{
				return "3";
			}
			if (this.GetData()[17].Contains("2"))
			{
				return "2";
			}
			if (this.GetData()[17].Contains("4"))
			{
				return "4";
			}
			return "1";
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000281D File Offset: 0x00000A1D
		public Texture2D GetBackBackTexture()
		{
			if (this.BackBackdrop == null)
			{
				this.BackBackdrop = NatureInTheValleyEntry.staticHelper.ModContent.Load<Texture2D>("PNGs\\BackBack" + this.GetLocation());
			}
			return this.BackBackdrop;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000C4B0 File Offset: 0x0000A6B0
		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
			if (base.ItemId.Contains("NatInValley.Creature."))
			{
				spriteBatch.Draw(this.GetItemTexture().GetTexture(), location + new Vector2(56f, 52f), new Rectangle?(this.GetItemTexture().GetSourceRect(0, null)), color * transparency * 0.8f, 0f, new Vector2((float)(this.sourceRect.Width / 2), (float)(this.sourceRect.Height / 2)), 3f, SpriteEffects.None, layerDepth);
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00002852 File Offset: 0x00000A52
		public ParsedItemData GetItemTexture()
		{
			if (this.itemTexture == null)
			{
				this.itemTexture = ItemRegistry.GetData(base.ItemId.Split("Tera.", StringSplitOptions.None)[1]);
			}
			return this.itemTexture;
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000122 RID: 290 RVA: 0x00002880 File Offset: 0x00000A80
		public override string DisplayName
		{
			get
			{
				return NatureInTheValleyEntry.staticHelper.Translation.Get("Terrarium");
			}
		}

		// Token: 0x0400007B RID: 123
		[XmlIgnore]
		public bool fishDirty = true;

		// Token: 0x0400007C RID: 124
		[XmlIgnore]
		private Texture2D bgTexture;

		// Token: 0x0400007D RID: 125
		public List<string> data = new List<string>();

		// Token: 0x0400007E RID: 126
		protected AnimatedSprite Sprite;

		// Token: 0x0400007F RID: 127
		[XmlIgnore]
		private Texture2D FrontTexture;

		// Token: 0x04000080 RID: 128
		[XmlIgnore]
		private Texture2D AddedTexture;

		// Token: 0x04000081 RID: 129
		[XmlIgnore]
		private float xOffset;

		// Token: 0x04000082 RID: 130
		[XmlIgnore]
		private bool left;

		// Token: 0x04000083 RID: 131
		[XmlIgnore]
		private Texture2D BackBackdrop;

		// Token: 0x04000084 RID: 132
		[XmlIgnore]
		private ParsedItemData itemTexture;
	}
}

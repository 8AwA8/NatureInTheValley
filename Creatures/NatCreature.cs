﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using xTile.Dimensions;

namespace Creatures
{
	// Token: 0x02000007 RID: 7
	public class NatCreature
	{
		// Token: 0x06000083 RID: 131 RVA: 0x00008A74 File Offset: 0x00006C74
		public virtual void Draw(SpriteBatch b)
		{
			Microsoft.Xna.Framework.Rectangle boundingBox = this.GetBoundingBox();
			this.Sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, this.Position) + new Vector2((float)this.Sprite.SpriteWidth / 2f, (float)((!this.IsGrounded) ? -90 : 0)), (float)boundingBox.Center.Y / 10000f, 0, this.heightOffGround, Color.White, false, this.scale * 1.2f, 0f, false);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00008B00 File Offset: 0x00006D00
		public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport)
		{
			Vector2 position = this.Position;
			return new Vector2(position.X - (float)viewport.X, position.Y - (float)viewport.Y);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00008B38 File Offset: 0x00006D38
		public virtual Microsoft.Xna.Framework.Rectangle GetBoundingBox()
		{
			if (this.Sprite == null)
			{
				return Microsoft.Xna.Framework.Rectangle.Empty;
			}
			Vector2 position = this.Position;
			int width = this.Sprite.SpriteWidth * 4 * 3 / 4;
			return new Microsoft.Xna.Framework.Rectangle((int)position.X + 8, (int)position.Y + 16, width, 32);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00008B88 File Offset: 0x00006D88
		public virtual void DrawShadow(SpriteBatch b)
		{
			if (this.scale < 0.9f)
			{
				return;
			}
			b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, this.Position) + new Vector2((float)this.Sprite.SpriteWidth * 1.4f + (float)this.shadowXOffset, (float)this.Sprite.SpriteHeight * 1.1f + (float)this.shadowYOffset), new Microsoft.Xna.Framework.Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0f, new Vector2((float)Game1.shadowTexture.Bounds.Center.X, (float)Game1.shadowTexture.Bounds.Center.Y), 2f, SpriteEffects.None, Math.Max(0f, (float)this.GetBoundingBox().Center.Y / 10000f) - 1E-06f);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00008C7C File Offset: 0x00006E7C
		public virtual void Update(GameTime time)
		{
			this.jumpTime += time.ElapsedGameTime.Milliseconds;
			if (this.isRunning)
			{
				this.desiredPos = this.Position + new Vector2(-200f, 0f);
				this.Position = Vector2.SmoothStep(this.Position, this.desiredPos, 10f * this.speed);
				return;
			}
			if (!this.isStatic && this.currentLocation != null)
			{
				Farmer farmer = Utility.isThereAFarmerWithinDistance(this.Position / 64f, this.PlayerRange, this.currentLocation);
				if (farmer != null)
				{
					if (!this.Dangerous && this.DoesRun && farmer.isMoving() && (!farmer.hasBuff("NatCSpeedN") || Vector2.Distance(farmer.Position, this.GetEffectivePosition()) < 28f))
					{
						this.isRunning = true;
						return;
					}
					if (this.Dangerous)
					{
						this.PlayerRange = this.savedRange * 3;
						if (Vector2.Distance(farmer.Position, this.Position) < 50f)
						{
							Bug bug = new Bug();
							farmer.takeDamage(25, true, bug);
							bug.Removed();
						}
						if (this.isMover && this.DoesRun)
						{
							this.isMoving = true;
							this.desiredPos = this.GetComplexDesiredForFarmer(farmer);
							if (this.desiredPos != Vector2.Zero)
							{
								this.Position = Vector2.Lerp(this.Position, this.desiredPos, this.speed * 4.25f * Math.Max(1f, Math.Min(2.75f, 170f / Vector2.Distance(this.desiredPos, this.Position))));
							}
							return;
						}
					}
				}
			}
			if (!this.isMover)
			{
				return;
			}
			if (this.desiredPos == Vector2.Zero)
			{
				Vector2 vector = new Vector2((float)Game1.random.Next(-180, 180), (float)Game1.random.Next(-180, 180));
				if (this.ValidPosition(this.GetEffectivePosition() + vector / 2f) && this.ValidPosition(this.GetEffectivePosition() + vector))
				{
					this.desiredPos = this.Position + vector;
				}
				else
				{
					this.desiredPos = Vector2.Zero;
				}
				if (this.oscillations > 200 && !this.isRunning && !this.isStatic)
				{
					this.isRunning = true;
					this.scale = 0f;
				}
				this.oscillations++;
				return;
			}
			if (this.stopTime > this.jumpTime)
			{
				this.isMoving = false;
				return;
			}
			this.isMoving = true;
			this.oscillations = 0;
			this.Position = Vector2.SmoothStep(this.Position, this.desiredPos, 10f * this.speed);
			if (Vector2.Distance(this.Position, this.desiredPos) < 10f || this.jumpTime > 5000)
			{
				this.desiredPos = Vector2.Zero;
				this.jumpTime = 0;
				return;
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00008FA0 File Offset: 0x000071A0
		public virtual void UpdateAnim(GameTime time)
		{
			if (this.isRunning)
			{
				this.Sprite.AnimateUp(time, 0, "");
				this.scale = this.scale * 0.96f - 0.0052f;
				return;
			}
			if (!this.isMover)
			{
				if (this.name == "Chameleon")
				{
					if (Game1.random.NextDouble() < 0.15)
					{
						this.Sprite.AnimateDown(time, 0, "");
					}
					return;
				}
				this.Sprite.AnimateDown(time, 0, "");
				return;
			}
			else if (this.desiredPos.X > this.Position.X)
			{
				if (this.isMoving)
				{
					this.Sprite.AnimateRight(time, 0, "");
					return;
				}
				this.Sprite.Animate(time, this.Sprite.framesPerAnimation, 1, 10f);
				return;
			}
			else
			{
				if (this.isMoving)
				{
					this.Sprite.AnimateDown(time, 0, "");
					return;
				}
				this.Sprite.Animate(time, 0, 1, 10f);
				return;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000090B8 File Offset: 0x000072B8
		public virtual bool TimeChange()
		{
			if (this.isStatic)
			{
				return false;
			}
			this.LifeTime += 10;
			if (Game1.timeOfDay > this.MaxTime)
			{
				this.LifeTime = 1500 * (this.Dangerous ? 2 : 1);
			}
			if (this.LifeTime >= 1500 * (this.Dangerous ? 2 : 1))
			{
				this.isRunning = true;
				if (this.LifeTime > 1500 * (this.Dangerous ? 2 : 1) + 10)
				{
					return true;
				}
			}
			return this.scale < 0.09f;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00009154 File Offset: 0x00007354
		private bool ValidPosition(Vector2 position)
		{
			new Location((int)(position.X / 64f), (int)(position.Y / 64f));
			return this.currentLocation.map != null && (this.LocalLocationCode == 3 ^ !this.currentLocation.isWaterTile((int)(position / 64f).X, (int)(position / 64f).Y)) && !this.currentLocation.IsTileOccupiedBy(new Vector2((float)((int)(position / 64f).X), (float)((int)(position / 64f).Y)), ~(CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring), CollisionMask.None, false) && this.currentLocation.isTilePassable(new Vector2((float)((int)(position / 64f).X), (float)((int)(position / 64f).Y)));
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000241C File Offset: 0x0000061C
		public GameLocation GetLocation()
		{
			return this.currentLocation;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00002424 File Offset: 0x00000624
		public void SetLocation(GameLocation l)
		{
			this.currentLocation = l;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000242D File Offset: 0x0000062D
		public int GetFrame()
		{
			return this.Sprite.currentFrame;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000243A File Offset: 0x0000063A
		public void SetSprite(int i)
		{
			this.Sprite.currentFrame = i;
			this.Sprite.UpdateSourceRect();
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00002453 File Offset: 0x00000653
		public Vector2 GetEffectivePosition()
		{
			return this.Position + new Vector2(8f, 8f) * 1.2f * this.scale;
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00009240 File Offset: 0x00007440
		private Vector2 GetComplexDesiredForFarmer(Farmer farmer)
		{
			if (Vector2.Distance(farmer.Position, this.Position) < 128f)
			{
				return farmer.Position;
			}
			Vector2 zero = Vector2.Zero;
			Vector2 zero2 = Vector2.Zero;
			if (farmer.Position.X > this.Position.X + 40f)
			{
				zero = new Vector2(64f, 0f);
			}
			else if (farmer.Position.X < this.Position.X - 40f)
			{
				zero = new Vector2(-64f, 0f);
			}
			if (farmer.Position.Y > this.Position.Y + 40f)
			{
				zero2 = new Vector2(0f, 64f);
			}
			else if (farmer.Position.Y < this.Position.Y - 40f)
			{
				zero2 = new Vector2(0f, -64f);
			}
			if (this.ValidPosition(this.GetEffectivePosition() + zero / 2f + zero2 / 2f))
			{
				if (this.ValidPosition(this.GetEffectivePosition() + zero + zero2))
				{
					return this.Position + zero + zero2;
				}
				return this.Position + zero / 2f + zero2 / 2f;
			}
			else if (this.ValidPosition(this.GetEffectivePosition() + zero2 / 2f))
			{
				if (this.ValidPosition(this.GetEffectivePosition() + zero2))
				{
					return this.Position + zero2;
				}
				return this.Position + zero2 / 2f;
			}
			else if (this.ValidPosition(this.GetEffectivePosition() + zero / 2f))
			{
				if (this.ValidPosition(this.GetEffectivePosition() + zero))
				{
					return this.Position + zero;
				}
				return this.Position + zero / 2f;
			}
			else
			{
				Vector2 vector = new Vector2((float)Game1.random.Next(-180, 180), (float)Game1.random.Next(-180, 180));
				if (this.ValidPosition(this.GetEffectivePosition() + vector / 2f) && this.ValidPosition(this.GetEffectivePosition() + vector))
				{
					return this.Position + vector;
				}
				return farmer.Position;
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000094DC File Offset: 0x000076DC
		public NatCreature(Vector2 position, GameLocation location, string Name, int rarity, bool isGrounded, float speed, int stopTime, float scale, bool DoesRun, bool isMover, int PlayerRange, bool dangerous, List<string> seasons, int weatherCode, List<string> locationCode, int MinTime, int MaxTime, int FramesPerAnim, string textureName, int offsetShad, int localLocationCode, int offsetShadY, bool isStatic)
		{
			this.Sprite = new AnimatedSprite();
			this.currentLocation = new GameLocation();
			this.speed = 0.0032f;
			this.desiredPos = default(Vector2);
			this.scale = 1f;
			this.Seasons = new List<string>();
			this.LocationCode = new List<string>();
			this.Position = position;
			this.currentLocation = location;
			this.Sprite = new AnimatedSprite(textureName);
			this.Sprite.SpriteHeight = 32;
			this.Sprite.SpriteWidth = 32;
			this.Sprite.framesPerAnimation = FramesPerAnim;
			this.IsGrounded = isGrounded;
			this.speed = speed;
			this.scale = scale;
			this.stopTime = stopTime;
			this.Rarity = rarity;
			this.name = Name;
			this.DoesRun = DoesRun;
			this.isMover = isMover;
			this.PlayerRange = PlayerRange;
			this.savedRange = PlayerRange;
			this.Dangerous = dangerous;
			this.Seasons.Clear();
			this.Seasons.AddRange(seasons);
			this.WeatherCode = weatherCode;
			this.LocationCode.AddRange(locationCode);
			this.MinTime = MinTime;
			this.MaxTime = MaxTime;
			this.shadowXOffset = offsetShad;
			this.LocalLocationCode = localLocationCode;
			this.shadowYOffset = offsetShadY;
			this.isStatic = isStatic;
			this.Sprite.UpdateSourceRect();
		}

		// Token: 0x04000028 RID: 40
		public float speed;

		// Token: 0x04000029 RID: 41
		public int stopTime;

		// Token: 0x0400002A RID: 42
		public int jumpTime;

		// Token: 0x0400002B RID: 43
		public float scale;

		// Token: 0x0400002C RID: 44
		private int LifeTime;

		// Token: 0x0400002D RID: 45
		public int shadowXOffset;

		// Token: 0x0400002E RID: 46
		public string name;

		// Token: 0x0400002F RID: 47
		public int LocalLocationCode;

		// Token: 0x04000030 RID: 48
		public int savedRange;

		// Token: 0x04000031 RID: 49
		public int shadowYOffset;

		// Token: 0x04000032 RID: 50
		public bool chasing;

		// Token: 0x04000033 RID: 51
		private int oscillations;

		// Token: 0x04000034 RID: 52
		public bool isStatic;

		// Token: 0x04000035 RID: 53
		protected AnimatedSprite Sprite;

		// Token: 0x04000036 RID: 54
		public int heightOffGround;

		// Token: 0x04000037 RID: 55
		public bool isMover;

		// Token: 0x04000038 RID: 56
		public bool IsEmoting;

		// Token: 0x04000039 RID: 57
		public bool DoesRun;

		// Token: 0x0400003A RID: 58
		public bool IsGrounded;

		// Token: 0x0400003B RID: 59
		protected GameLocation currentLocation;

		// Token: 0x0400003C RID: 60
		public Vector2 desiredPos;

		// Token: 0x0400003D RID: 61
		public bool isMoving;

		// Token: 0x0400003E RID: 62
		public List<string> Seasons;

		// Token: 0x0400003F RID: 63
		public int WeatherCode;

		// Token: 0x04000040 RID: 64
		public int Rarity;

		// Token: 0x04000041 RID: 65
		public int PlayerRange;

		// Token: 0x04000042 RID: 66
		public bool Dangerous;

		// Token: 0x04000043 RID: 67
		public int MinTime;

		// Token: 0x04000044 RID: 68
		public int MaxTime;

		// Token: 0x04000045 RID: 69
		public bool isRunning;

		// Token: 0x04000046 RID: 70
		public Vector2 Position;

		// Token: 0x04000047 RID: 71
		public List<string> LocationCode;
	}
}

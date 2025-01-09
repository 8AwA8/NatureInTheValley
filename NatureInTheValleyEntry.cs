﻿using System;
using System.Collections.Generic;
using Creatures;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Shops;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using xTile.Layers;

namespace NatureInTheValley
{
	// Token: 0x02000004 RID: 4
	internal sealed class NatureInTheValleyEntry : Mod
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002664 File Offset: 0x00000864
		public override void Entry(IModHelper helper)
		{
			this.helper = helper;
			NatInValleyConfig natInValleyConfig = helper.ReadConfig<NatInValleyConfig>();
			this.config = natInValleyConfig;
			NatureInTheValleyEntry.staticConfig = natInValleyConfig;
			NatureInTheValleyEntry.staticHelper = helper;
			if (!this.config.useOnlyContentPacks)
			{
				foreach (KeyValuePair<string, string> keyValuePair in helper.ModContent.Load<Dictionary<string, string>>("MainData.json"))
				{
					this.creatureData.TryAdd(keyValuePair.Key, new List<string>(keyValuePair.Value.Split('/', StringSplitOptions.None)));
				}
			}
			IList<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
			foreach (IContentPack contentPack in base.Helper.ContentPacks.GetOwned())
			{
				IMonitor monitor = base.Monitor;
				list.Add(contentPack.ReadJsonFile<Dictionary<string, string>>("content.json"));
			}
			foreach (Dictionary<string, string> dictionary in list)
			{
				foreach (KeyValuePair<string, string> keyValuePair2 in dictionary)
				{
					if (!this.creatureData.ContainsKey(keyValuePair2.Key))
					{
						this.creatureData.TryAdd(keyValuePair2.Key, new List<string>(keyValuePair2.Value.Split('/', StringSplitOptions.None)));
					}
					else
					{
						this.creatureData[keyValuePair2.Key] = new List<string>(keyValuePair2.Value.Split('/', StringSplitOptions.None));
					}
				}
			}
			NatureInTheValleyEntry.staticCreatureData = this.creatureData;
			helper.Events.Player.Warped += this.OnWarp;
			helper.Events.GameLoop.TimeChanged += this.TimeChange;
			helper.Events.Input.ButtonPressed += this.Pressed;
			helper.Events.GameLoop.OneSecondUpdateTicked += this.OneSecond;
			helper.Events.Multiplayer.ModMessageReceived += this.OnModMessagereceived;
			helper.Events.GameLoop.GameLaunched += this.OnLaunch;
			helper.Events.Display.MenuChanged += this.MenuChanged;
			helper.Events.World.LargeTerrainFeatureListChanged += this.RemovedLargeTerrain;
			helper.Events.GameLoop.DayStarted += this.DayStarted;
			helper.Events.Content.AssetRequested += this.OnAssetRequested;
			NatureInTheValleyEntry.netTexture = helper.ModContent.Load<Texture2D>("PNGs\\NormalNet");
			Harmony harmony = new Harmony("Nature.NatureInTheValley");
			GameLocation.RegisterTileAction("NatInValley_ReturnHome", new Func<GameLocation, string[], Farmer, Point, bool>(this.ReturnHome));
			GameLocation.RegisterTileAction("NatWarpInsect", new Func<GameLocation, string[], Farmer, Point, bool>(this.StartBossFadeHandle));
			GameLocation.RegisterTileAction("NIVOpenDonate", new Func<GameLocation, string[], Farmer, Point, bool>(this.OpenDonationWindow));
			helper.ConsoleCommands.Add("NatCreat_Summon", "Summons a creature.\n\nUsage: NatCreat_Summon <value>\n- value: the name.", new Action<string, string[]>(this.Command));
			helper.ConsoleCommands.Add("NatCreat_FillMuseum", "Summons a creature.\n\nUsage: NatCreat_Summon <value>\n- value: the name.", new Action<string, string[]>(this.CommandTwo));
			helper.ConsoleCommands.Add("NatCreat_EmptyMuseum", "Summons a creature.\n\nUsage: NatCreat_Summon <value>\n- value: the name.", new Action<string, string[]>(this.CommandThree));
			helper.ConsoleCommands.Add("NatCreat_List", "Summons a creature.\n\nUsage: NatCreat_Summon <value>\n- value: the name.", new Action<string, string[]>(this.CommandFour));
			harmony.Patch(AccessTools.Method(typeof(GameLocation), "draw", null, null), new HarmonyMethod(typeof(NatureInTheValleyEntry), "CreatureDrawer", null), null, null, null);
			harmony.Patch(AccessTools.Method(typeof(GameLocation), "UpdateWhenCurrentLocation", null, null), new HarmonyMethod(typeof(NatureInTheValleyEntry), "Ticking", null), null, null, null);
			harmony.Patch(AccessTools.Method(typeof(GameLocation), "answerDialogue", null, null), new HarmonyMethod(typeof(NatureInTheValleyEntry), "PostTextFunction", null), null, null, null);
			harmony.Patch(AccessTools.Method(typeof(BusStop), "checkAction", null, null), new HarmonyMethod(typeof(NatureInTheValleyEntry), "CoverCheckAction", null), null, null, null);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002B28 File Offset: 0x00000D28
		public NatureInTheValleyEntry()
		{
			this.mpCM = 1.0;
			this.creatureData = new Dictionary<string, List<string>>();
			this.desertLocationNames = new HashSet<string>
			{
				"Desert",
				"SkullCave",
				"DesertFestival"
			};
			this.forestLocationNames = new HashSet<string>
			{
				"SecretWoods",
				"Forest",
				"Backwoods",
				"Woods",
				"Mountain",
				"Farm_Forest",
				"NIVOuterInsec"
			};
			this.waterLocationNames = new HashSet<string>
			{
				"Beach",
				"BeachNightMarket",
				"IslandWest",
				"IslandSouth",
				"IslandSouthEast",
				"IslandSouthEastCave",
				"Farm_Beach"
			};
			this.underLocationNames = new HashSet<string>
			{
				"FarmCave",
				"Mine",
				"UndergroundMine",
				"BugLand",
				"WitchWarpCave",
				"SkullCave",
				"WitchSwamp",
				"MasteryCave",
				"IslandSoutheastCave"
			};
			this.treePos = new List<Vector2>();
			this.waterPos = new List<Vector2>();
			this.layersToSave = new List<Layer>();
			this.bushPos = new List<Vector2>();
			this.stumpPos = new List<Vector2>();
			this.dailyMod = 1f;
			this.possibleDailyMods = new List<float>
			{
				0.8f,
				0.8f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1.25f,
				1.25f,
				1.5f,
				1.5f
			};
			this.locationCap = 1;
			this.step = 1;
			this.spawnChance = 1.0;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002DB4 File Offset: 0x00000FB4
		public void OnWarp(object sender, WarpedEventArgs eventArgs)
		{
			if (eventArgs.NewLocation != null && eventArgs.NewLocation.Name != null)
			{
				if (eventArgs.NewLocation.Name == "BusStop")
				{
					this.setUpPositionalWarp(eventArgs.NewLocation);
				}
				if (eventArgs.NewLocation.Name == "NIVInnerInsec")
				{
					this.setUpInsectarium(eventArgs.NewLocation);
				}
				this.treePos = this.GetTrees(eventArgs.NewLocation);
				this.waterPos = this.GetWater(eventArgs.NewLocation);
				this.bushPos = this.GetBushes(eventArgs.NewLocation);
				this.stumpPos = this.GetStumps(eventArgs.NewLocation);
				if (eventArgs.NewLocation.map.GetLayer("Back") != null)
				{
					this.Tiles = eventArgs.NewLocation.map.GetLayer("Back").Tiles.Array.Length;
				}
				else
				{
					this.Tiles = 10;
				}
				this.locationCap = (int)((double)((float)(2 + this.Tiles / 1800)) * (double)Game1.numberOfPlayers() * (double)this.config.maxcreaturelLimitMultiplier * (double)this.dailyMod);
				this.spawnChance = 0.12 * (double)this.config.spawnRateMultiplier * this.func(this.Tiles) * 1.45 * (double)this.dailyMod;
				this.locationalData = this.CreaturesForArea(eventArgs.NewLocation);
				eventArgs.NewLocation.resourceClumps.OnValueRemoved -= new NetCollection<ResourceClump>.ContentsChangeEvent(this.RemovedResource);
				eventArgs.NewLocation.resourceClumps.OnValueRemoved += new NetCollection<ResourceClump>.ContentsChangeEvent(this.RemovedResource);
			}
			if (eventArgs.OldLocation != null)
			{
				eventArgs.OldLocation.resourceClumps.OnValueRemoved -= new NetCollection<ResourceClump>.ContentsChangeEvent(this.RemovedResource);
				this.ClearUnclaimedCreatures(eventArgs.OldLocation);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002FA0 File Offset: 0x000011A0
		private static void CreatureDrawer(GameLocation __instance, SpriteBatch b)
		{
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				if (natCreature.GetLocation() != null && natCreature.GetLocation().Name == __instance.Name)
				{
					natCreature.Draw(b);
					natCreature.DrawShadow(b);
				}
			}
			Farmer player = Game1.player;
			if (player.ActiveItem != null && player.ActiveItem is NatInValeyNet)
			{
				Texture2D texture = NatureInTheValleyEntry.netTexture;
				if ((player.ActiveItem as NatInValeyNet).isHeld)
				{
					NatureInTheValleyEntry.AnimatedBump.Value = 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(1f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 1:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-50f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.Hair, false));
						return;
					case 3:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-22f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
				else if (NatureInTheValleyEntry.AnimatedBump.Value > 0 && NatureInTheValleyEntry.AnimatedBump.Value < 10)
				{
					PerScreen<int> animatedBump = NatureInTheValleyEntry.AnimatedBump;
					int value = animatedBump.Value;
					animatedBump.Value = value + 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-20f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.ToolUp, false));
						return;
					case 1:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(0f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.ToolDown, false));
						return;
					case 3:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-85f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
				else if (NatureInTheValleyEntry.AnimatedBump.Value > 0 && NatureInTheValleyEntry.AnimatedBump.Value < 20)
				{
					PerScreen<int> animatedBump2 = NatureInTheValleyEntry.AnimatedBump;
					int value2 = animatedBump2.Value;
					animatedBump2.Value = value2 + 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-20f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.ToolUp, false));
						return;
					case 1:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(30f, -110f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.ToolDown, false));
						return;
					case 3:
						b.Draw(texture, Game1.GlobalToLocal(player.Position + new Vector2(-100f, -110f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
			}
			else if (player.ActiveItem != null && player.ActiveItem is NatInValeyGoldenNet)
			{
				Texture2D texture2 = NatInValeyGoldenNet.texture;
				if ((player.ActiveItem as NatInValeyGoldenNet).isHeld)
				{
					NatureInTheValleyEntry.AnimatedBump.Value = 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(1f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 1:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-50f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.Hair, false));
						return;
					case 3:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-22f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
				else if (NatureInTheValleyEntry.AnimatedBump.Value > 0 && NatureInTheValleyEntry.AnimatedBump.Value < 10)
				{
					PerScreen<int> animatedBump3 = NatureInTheValleyEntry.AnimatedBump;
					int value3 = animatedBump3.Value;
					animatedBump3.Value = value3 + 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-20f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.ToolUp, false));
						return;
					case 1:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(0f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.ToolDown, false));
						return;
					case 3:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-85f, -130f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(32, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.2f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
				else if (NatureInTheValleyEntry.AnimatedBump.Value > 0 && NatureInTheValleyEntry.AnimatedBump.Value < 20)
				{
					PerScreen<int> animatedBump4 = NatureInTheValleyEntry.AnimatedBump;
					int value4 = animatedBump4.Value;
					animatedBump4.Value = value4 + 1;
					switch (player.FacingDirection)
					{
					case 0:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-20f, -120f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 96, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.ToolUp, false));
						return;
					case 1:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(30f, -110f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 64, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					case 2:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-59f, -105f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 32, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer() - 1E-06f, FarmerRenderer.FarmerSpriteLayers.ToolDown, false));
						return;
					case 3:
						b.Draw(texture2, Game1.GlobalToLocal(player.Position + new Vector2(-100f, -110f)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(64, 128, 32, 32)), Color.White, 0f, Vector2.Zero, 4.4f, SpriteEffects.None, FarmerRenderer.GetLayerDepth(player.getDrawLayer(), FarmerRenderer.FarmerSpriteLayers.Tool, false));
						return;
					default:
						return;
					}
				}
			}
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002050 File Offset: 0x00000250
		static NatureInTheValleyEntry()
		{
			NatureInTheValleyEntry.staticCreatureData = new Dictionary<string, List<string>>();
			NatureInTheValleyEntry.creatures = new List<NatCreature>();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00003B14 File Offset: 0x00001D14
		private static void Ticking(GameLocation __instance, GameTime time)
		{
			if (__instance.wasUpdated || !Game1.shouldTimePass(false) || !Game1.IsMasterGame)
			{
				return;
			}
			if (Context.IsMultiplayer || Game1.currentLocation.Name == "NIVInnerInsec")
			{
				for (int i = 0; i < NatureInTheValleyEntry.creatures.Count; i++)
				{
					NatureInTheValleyEntry.creatures[i].Update(time);
					NatureInTheValleyEntry.creatures[i].UpdateAnim(time);
				}
			}
			else
			{
				for (int j = 0; j < NatureInTheValleyEntry.creatures.Count; j++)
				{
					if (Vector2.Distance(NatureInTheValleyEntry.creatures[j].Position, Game1.player.Position) < 1500f)
					{
						NatureInTheValleyEntry.creatures[j].Update(time);
						NatureInTheValleyEntry.creatures[j].UpdateAnim(time);
					}
				}
			}
			if (!Context.IsMultiplayer || Context.IsSplitScreen)
			{
				return;
			}
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			List<string> list3 = new List<string>();
			List<Vector2> list4 = new List<Vector2>();
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				list.Add(natCreature.GetLocation().Name);
				list3.Add(natCreature.name);
				list2.Add(natCreature.GetFrame());
				list4.Add(natCreature.Position);
			}
			Model model = new Model();
			model.locations = list;
			model.frames = list2;
			model.positions = list4;
			model.creatures = list3;
			NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<Model>(model, "FixListLocations", new string[]
			{
				"Nature.NatureInTheValley"
			}, null);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00003CE8 File Offset: 0x00001EE8
		public void TimeChange(object sender, TimeChangedEventArgs e)
		{
			if (Game1.currentLocation == null || Game1.currentLocation.Name == "NIVInnerInsec")
			{
				return;
			}
			this.treePos = this.GetTrees(Game1.currentLocation);
			this.mpCM = Math.Sqrt((double)Game1.currentLocation.farmers.Count);
			if (Context.IsMainPlayer)
			{
				for (int i = 0; i < NatureInTheValleyEntry.creatures.Count; i++)
				{
					if (Context.IsMainPlayer && NatureInTheValleyEntry.creatures[i].TimeChange())
					{
						NatureInTheValleyEntry.creatures.RemoveAt(i);
						i--;
					}
				}
			}
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00003D84 File Offset: 0x00001F84
		public void Instantiate(string name, Vector2 tile, GameLocation location)
		{
			List<string> list = this.creatureData[name];
			List<int> list2 = new List<int>();
			foreach (string s in list[11].Split(",", StringSplitOptions.None))
			{
				list2.Add(int.Parse(s));
			}
			NatureInTheValleyEntry.creatures.Add(new NatCreature(tile * 64f, location, name, int.Parse(list[0]), list[1] == "true", float.Parse(list[2]), int.Parse(list[3]), float.Parse(list[4]), list[5] == "true", list[6] == "true", int.Parse(list[7]), list[8] == "true", new List<string>(list[9].Split(',', StringSplitOptions.None)), int.Parse(list[10]), list2, int.Parse(list[12]), int.Parse(list[13]), int.Parse(list[14]), list[15], int.Parse(list[16]), int.Parse(list[17]), int.Parse(list[18]), false));
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00003EF0 File Offset: 0x000020F0
		public void SpawnCreatureInLocation(GameLocation location)
		{
			if (location == null)
			{
				return;
			}
			if (this.validLocation(location, 5) && this.locationalData.Count > 0)
			{
				Layer layer = location.map.RequireLayer("Back");
				Vector2 vector = new Vector2((float)Game1.random.Next(layer.TileWidth), (float)Game1.random.Next(layer.TileHeight));
				if (this.ValidPosition(vector, location))
				{
					this.TrySpawnFromArea(location, this.locationalData, layer, vector);
				}
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00003F6C File Offset: 0x0000216C
		public Dictionary<string, List<string>> CreaturesForArea(GameLocation location)
		{
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			string currentSeason = Game1.currentSeason;
			int timeOfDay = Game1.timeOfDay;
			bool flag = location.IsRainingHere();
			bool flag2 = location.IsLightningHere();
			bool flag3 = location.IsSnowingHere();
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
			{
				List<string> value = keyValuePair.Value;
				bool flag4 = false;
				foreach (string s in value[11].Split(",", StringSplitOptions.None))
				{
					if (this.validLocation(location, int.Parse(s)))
					{
						flag4 = true;
						break;
					}
				}
				if (flag4 && new List<string>(value[9].Split(",", StringSplitOptions.None)).Contains(currentSeason) && timeOfDay <= int.Parse(value[13]) && timeOfDay >= int.Parse(value[12]) && ((!flag && !flag2 && !flag3) || int.Parse(value[10]) != 1) && (flag || flag2 || flag3 || int.Parse(value[10]) != 2) && (flag2 || int.Parse(value[10]) != 3) && (flag3 || int.Parse(value[10]) != 4))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000410C File Offset: 0x0000230C
		public bool validLocation(GameLocation location, int code)
		{
			switch (code)
			{
			case 0:
				return (location.IsOutdoors || this.forestLocationNames.Contains(location.Name)) && !this.desertLocationNames.Contains(location.Name) && !this.waterLocationNames.Contains(location.Name);
			case 1:
			{
				int num = 0;
				if (this.treePos != null)
				{
					num = this.treePos.Count;
				}
				return this.forestLocationNames.Contains(location.Name) || num > 60;
			}
			case 2:
				return this.waterLocationNames.Contains(location.Name) || ((this.waterPos != null) ? ((float)this.waterPos.Count) : 0f) / (float)Math.Max(this.Tiles, 100) >= 0.4f;
			case 3:
				return this.underLocationNames.Contains(location.Name) || (location is MineShaft && (float)(location as MineShaft).mineLevel % 10f == 0f);
			case 4:
				return this.desertLocationNames.Contains(location.Name);
			case 5:
				return location.IsOutdoors || this.underLocationNames.Contains(location.Name) || (location is MineShaft && (float)(location as MineShaft).mineLevel % 10f == 0f);
			default:
				return location.IsOutdoors || this.forestLocationNames.Contains(location.Name) || this.waterLocationNames.Contains(location.Name) || this.underLocationNames.Contains(location.Name) || (location is MineShaft && (float)(location as MineShaft).mineLevel % 10f == 0f) || this.desertLocationNames.Contains(location.Name);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00004300 File Offset: 0x00002500
		public List<Vector2> GetTrees(GameLocation location)
		{
			List<Vector2> list = new List<Vector2>();
			try
			{
				using (NetDictionary<Vector2, TerrainFeature, NetRef<TerrainFeature>, SerializableDictionary<Vector2, TerrainFeature>, NetVector2Dictionary<TerrainFeature, NetRef<TerrainFeature>>>.ValuesCollection.Enumerator enumerator = location.terrainFeatures.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current is Tree && (enumerator.Current as Tree).growthStage.Value >= 5 && !(enumerator.Current as Tree).stump.Value)
						{
							list.Add(enumerator.Current.Tile);
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return list;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000043B8 File Offset: 0x000025B8
		public List<Vector2> GetBushes(GameLocation location)
		{
			List<Vector2> list = new List<Vector2>();
			foreach (LargeTerrainFeature largeTerrainFeature in location.largeTerrainFeatures)
			{
				if (largeTerrainFeature is Bush)
				{
					list.Add(largeTerrainFeature.Tile + new Vector2(-0.06f, 0.75f));
				}
			}
			return list;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00004434 File Offset: 0x00002634
		public void ClearUnclaimedCreatures(GameLocation location)
		{
			if (location.farmers.Count > 0)
			{
				return;
			}
			if (Context.IsMainPlayer)
			{
				for (int i = 0; i < NatureInTheValleyEntry.creatures.Count; i++)
				{
					if (NatureInTheValleyEntry.creatures[i].GetLocation() == location)
					{
						NatureInTheValleyEntry.creatures.RemoveAt(i);
						i--;
					}
				}
				return;
			}
			for (int j = 0; j < NatureInTheValleyEntry.creatures.Count; j++)
			{
				if (NatureInTheValleyEntry.creatures[j].GetLocation() == location)
				{
					base.Helper.Multiplayer.SendMessage<int>(j, "RemoveFromList", new string[]
					{
						"Nature.NatureInTheValley"
					}, null);
					NatureInTheValleyEntry.creatures.RemoveAt(j);
					j--;
				}
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000044EC File Offset: 0x000026EC
		public int HandleRarityChance(Dictionary<string, List<string>> data)
		{
			if (data.Count == 0)
			{
				return -1;
			}
			for (int i = 0; i < 30; i++)
			{
				int num = Game1.random.Next(0, data.Count);
				int num2 = int.Parse(new List<List<string>>(data.Values)[num][0]);
				if (num2 == 0)
				{
					return num;
				}
				if (num2 == 1)
				{
					if (Game1.random.NextDouble() < 0.58)
					{
						return num;
					}
				}
				else if (num2 == 2)
				{
					if (Game1.random.NextDouble() < 0.13)
					{
						return num;
					}
				}
				else if (num2 == 3)
				{
					if (Game1.random.NextDouble() < 0.05 && this.donatedCount > 25)
					{
						return num;
					}
				}
				else if (num2 == 4 && Game1.random.NextDouble() < 0.015 && this.donatedCount > 50)
				{
					return num;
				}
			}
			return -1;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x000045CC File Offset: 0x000027CC
		public void TrySpawnFromArea(GameLocation location, Dictionary<string, List<string>> data, Layer layer, Vector2 vector)
		{
			int num = this.HandleRarityChance(data);
			if (num == -1)
			{
				return;
			}
			KeyValuePair<string, List<string>> keyValuePair = new KeyValuePair<string, List<string>>(new List<string>(data.Keys)[num], new List<List<string>>(data.Values)[num]);
			if (int.Parse(keyValuePair.Value[17]) == 0)
			{
				this.Instantiate(keyValuePair.Key, vector + new Vector2(-0.25f, 0.25f), location);
				return;
			}
			if (int.Parse(keyValuePair.Value[17]) == 1)
			{
				List<Vector2> list = (this.treePos != null) ? this.treePos : this.GetTrees(location);
				if (list.Count < 2)
				{
					return;
				}
				vector = list[(int)(Game1.random.NextDouble() * (double)(list.Count - 1))] + new Vector2(-0.25f, 0.55f);
				if (this.CheckForBugAtTile(location, vector * 64f, 64f))
				{
					return;
				}
				this.Instantiate(keyValuePair.Key, vector, location);
				return;
			}
			else if (int.Parse(keyValuePair.Value[17]) == 2)
			{
				List<Vector2> list2 = (this.bushPos != null) ? this.bushPos : this.GetBushes(location);
				if (list2.Count < 2 || Game1.random.NextDouble() < 0.28)
				{
					return;
				}
				vector = list2[Game1.random.Next(list2.Count - 1)];
				if (this.CheckForBugAtTile(location, vector * 64f, 64f))
				{
					return;
				}
				this.Instantiate(keyValuePair.Key, vector, location);
				return;
			}
			else
			{
				if (int.Parse(keyValuePair.Value[17]) != 3)
				{
					if (int.Parse(keyValuePair.Value[17]) == 4)
					{
						List<Vector2> list3 = (this.stumpPos != null) ? this.stumpPos : this.GetStumps(location);
						if (list3.Count < 2)
						{
							return;
						}
						vector = list3[Game1.random.Next(list3.Count - 1)];
						if (this.CheckForBugAtTile(location, vector * 64f, 64f))
						{
							return;
						}
						this.Instantiate(keyValuePair.Key, vector, location);
					}
					return;
				}
				List<Vector2> list4 = (this.waterPos != null) ? this.waterPos : this.GetWater(location);
				if (list4 == null || list4.Count < 2 || Game1.random.NextDouble() < 0.1)
				{
					return;
				}
				vector = list4[Game1.random.Next(list4.Count - 1)];
				this.Instantiate(keyValuePair.Key, vector, location);
				return;
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00004878 File Offset: 0x00002A78
		public List<Vector2> GetStumps(GameLocation location)
		{
			List<Vector2> list = new List<Vector2>();
			foreach (ResourceClump resourceClump in location.resourceClumps)
			{
				if (resourceClump.parentSheetIndex.Value == 600)
				{
					list.Add(resourceClump.Tile + new Vector2(0.25f, 1.4f));
				}
			}
			return list;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00004900 File Offset: 0x00002B00
		public List<Vector2> GetWater(GameLocation location)
		{
			List<Vector2> list = new List<Vector2>();
			if (location.waterTiles == null || location.waterTiles.waterTiles == null)
			{
				return list;
			}
			for (int i = 0; i < location.map.Layers[0].LayerWidth; i++)
			{
				for (int j = 0; j < location.map.Layers[0].LayerHeight; j++)
				{
					if (location.waterTiles.waterTiles[i, j].isWater && location.waterTiles.waterTiles[i, j].isVisible && location.isOpenWater(i, j))
					{
						list.Add(new Vector2((float)i, (float)j));
					}
				}
			}
			return list;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000049BC File Offset: 0x00002BBC
		private bool CheckForBugAtTile(GameLocation location, Vector2 pos, float distance)
		{
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				if (natCreature.GetLocation() != null && natCreature.GetLocation().Name == location.Name && Vector2.Distance(pos, natCreature.Position) <= distance)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00004A40 File Offset: 0x00002C40
		public void RemovedResource(object sender)
		{
			this.stumpPos = this.GetStumps(Game1.currentLocation);
			List<Vector2> list = this.stumpPos;
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				if (natCreature.LocalLocationCode == 4 && !natCreature.isStatic && natCreature.GetLocation().Name == Game1.currentLocation.Name)
				{
					Vector2 value = natCreature.Position / 64f;
					bool flag = false;
					foreach (Vector2 value2 in list)
					{
						if (Vector2.Distance(value, value2) < 1.5f)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (Context.IsMainPlayer)
						{
							natCreature.isRunning = true;
						}
						else
						{
							NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<int>(NatureInTheValleyEntry.creatures.IndexOf(natCreature), "SetRun", new string[]
							{
								"Nature.NatureInTheValley"
							}, null);
						}
					}
				}
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00004B80 File Offset: 0x00002D80
		private void setUpPositionalWarp(GameLocation a)
		{
			if (!base.Helper.ModRegistry.IsLoaded("hootless.BusLocations") || !base.Helper.ModRegistry.IsLoaded("Nature.NIVBL"))
			{
				a.setMapTile(28, 10, 1032, "Front", "outdoors", null, false);
				a.setMapTile(28, 11, 1057, "Buildings", "outdoors", null, false);
				if (!a.overlayObjects.ContainsKey(new Vector2(28f, 12f)))
				{
					if (a.overlayObjects.ContainsKey(new Vector2(28f, 11f)))
					{
						a.overlayObjects.Remove(new Vector2(28f, 11f));
					}
					a.overlayObjects.Add(new Vector2(28f, 11f), new StardewValley.Object("NatInValley.Creature.AtlasMoth", 1, false, -1, 0));
				}
				a.setTileProperty(28, 11, "Buildings", "Action", "None");
				a.setTileProperty(28, 11, "Buildings", "Action", "NatWarpInsect");
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00004CA4 File Offset: 0x00002EA4
		private bool StartBossFadeHandle(GameLocation location, string[] args, Farmer player, Point point)
		{
			if (Game1.player.Money < 100)
			{
				Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:BusStop_NotEnoughMoneyForTicket"));
			}
			else
			{
				location.createQuestionDialogue(this.helper.Translation.Get("BusStop_InsectariumTravel"), location.createYesNoResponses(), "NatInsectarium");
			}
			return true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00004D04 File Offset: 0x00002F04
		private static void PostTextFunction(GameLocation __instance, Response answer)
		{
			if (__instance.lastQuestionKey == null || __instance.afterQuestion != null)
			{
				return;
			}
			if (ArgUtility.SplitBySpaceAndGet(__instance.lastQuestionKey, 0, null) + "_" + answer.responseKey == "NatInsectarium_Yes" && Game1.player.Money >= 100)
			{
				Game1.player.Money -= 75;
				Game1.warpFarmer("NIVOuterInsec", 24, 39, 0);
				return;
			}
			if (__instance.lastQuestionKey == "NatDesk")
			{
				if (answer.responseKey == "NIVFResponse")
				{
					Game1.activeClickableMenu = new CreatureDonationMenu();
					return;
				}
				if (answer.responseKey == "NIVSResponse")
				{
					Game1.activeClickableMenu = new ClickIntoCreatureInfoMenu();
				}
				if (answer.responseKey == "NIVRResponse")
				{
					Game1.activeClickableMenu = new ItemGrabMenu(NatureInTheValleyEntry.getRewardsForPlayer(Game1.player), false, true, new InventoryMenu.highlightThisItem(NatureInTheValleyEntry.HighlightCollectableRewards), null, "Rewards", new ItemGrabMenu.behaviorOnItemSelect(NatureInTheValleyEntry.OnRewardCollected), false, true, false, false, false, 0, null, -1, null, ItemExitBehavior.ReturnToPlayer, true);
				}
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002070 File Offset: 0x00000270
		public void Command(string common, string[] args)
		{
			this.Instantiate(args[0], Game1.player.Tile, Game1.player.currentLocation);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00004E1C File Offset: 0x0000301C
		public void OnLaunch(object sender, GameLaunchedEventArgs eventArgs)
		{
			this.helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore").RegisterSerializerType(typeof(NatInValeyNet));
			this.helper.ModRegistry.GetApi<ISpaceCoreAPI>("spacechase0.SpaceCore").RegisterSerializerType(typeof(NatInValeyGoldenNet));
			IGenericModConfigMenuApi api = base.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
			if (api == null)
			{
				return;
			}
			api.Register(base.ModManifest, delegate
			{
				this.config = new NatInValleyConfig();
			}, delegate
			{
				base.Helper.WriteConfig<NatInValleyConfig>(this.config);
			}, false);
			api.AddNumberOption(base.ModManifest, () => this.config.spawnRateMultiplier, delegate(float value)
			{
				this.config.spawnRateMultiplier = value;
			}, () => this.helper.Translation.Get("MenuingMult"), () => this.helper.Translation.Get("MenuingMultInfo"), new float?(0.1f), new float?(10f), new float?(0.1f), null, null);
			api.AddNumberOption(base.ModManifest, () => this.config.maxcreaturelLimitMultiplier, delegate(float value)
			{
				this.config.maxcreaturelLimitMultiplier = value;
			}, () => this.helper.Translation.Get("MenuingLim"), () => this.helper.Translation.Get("MenuingLimInfo"), new float?(0.1f), new float?(10f), new float?(0.1f), null, null);
			api.AddNumberOption(base.ModManifest, () => this.config.creaturePriceMultiplier, delegate(float value)
			{
				this.config.creaturePriceMultiplier = value;
			}, () => this.helper.Translation.Get("MenuingPrice"), () => this.helper.Translation.Get("MenuingPriceInfo"), new float?(0.1f), new float?(10f), new float?(0.1f), null, null);
			api.AddBoolOption(base.ModManifest, () => this.config.addCreaturesToShippingCollection, delegate(bool value)
			{
				this.config.addCreaturesToShippingCollection = value;
			}, () => this.helper.Translation.Get("MenuingShip"), () => this.helper.Translation.Get("MenuingShipInfo"), null);
			api.AddBoolOption(base.ModManifest, () => this.config.useOnlyContentPacks, delegate(bool value)
			{
				this.config.useOnlyContentPacks = value;
			}, () => this.helper.Translation.Get("MenuingPack"), () => "", null);
			api.AddNumberOption(base.ModManifest, () => this.config.catchingDifficultyMultiplier, delegate(float value)
			{
				this.config.catchingDifficultyMultiplier = value;
				NatureInTheValleyEntry.staticConfig.catchingDifficultyMultiplier = value;
			}, () => this.helper.Translation.Get("MenuingNet"), () => this.helper.Translation.Get("MenuingNetInfo"), new float?(0.7f), new float?(3f), new float?(0.1f), null, null);
			api.AddKeybindList(base.ModManifest, () => this.config.KeyForEncy, delegate(KeybindList value)
			{
				this.config.KeyForEncy = value;
			}, () => this.helper.Translation.Get("MenuingKey"), () => this.helper.Translation.Get("MenuingKeyInfo"), null);
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000050EC File Offset: 0x000032EC
		public static void TryCatch(Farmer farmer)
		{
			Vector2 value = default(Vector2);
			switch (farmer.FacingDirection)
			{
			case 0:
				value = farmer.Position + new Vector2(0f, -96f);
				break;
			case 1:
				value = farmer.Position + new Vector2(96f, 0f);
				break;
			case 2:
				value = farmer.Position + new Vector2(0f, 96f);
				break;
			case 3:
				value = farmer.Position + new Vector2(-96f, 0f);
				break;
			}
			for (int i = 0; i < NatureInTheValleyEntry.creatures.Count; i++)
			{
				if (NatureInTheValleyEntry.creatures[i].isStatic || farmer.currentLocation.Name == "NIVInnerInsec")
				{
					return;
				}
				if (NatureInTheValleyEntry.creatures[i].GetLocation().Name == farmer.currentLocation.Name && Vector2.Distance(NatureInTheValleyEntry.creatures[i].GetEffectivePosition() + new Vector2(16f, 16f) + (NatureInTheValleyEntry.creatures[i].IsGrounded ? Vector2.Zero : new Vector2(0f, -30f)), value) < (NatureInTheValleyEntry.creatures[i].IsGrounded ? 60f : 85f) * NatureInTheValleyEntry.staticConfig.catchingDifficultyMultiplier && !NatureInTheValleyEntry.creatures[i].isRunning)
				{
					Game1.player.addItemByMenuIfNecessary(ItemRegistry.Create("NatInValley.Creature." + NatureInTheValleyEntry.creatures[i].name, 1, 0, false), null, false);
					Game1.player.gainExperience(2, 3 + (int)Math.Pow(4.0, (double)NatureInTheValleyEntry.creatures[i].Rarity));
					NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<int>(i, "RemoveFromList", new string[]
					{
						"Nature.NatureInTheValley"
					}, null);
					NatureInTheValleyEntry.creatures.RemoveAt(i);
					i--;
				}
				else if (NatureInTheValleyEntry.creatures[i].GetLocation() == farmer.currentLocation && Vector2.Distance(NatureInTheValleyEntry.creatures[i].GetEffectivePosition() + new Vector2(16f, 16f) + (NatureInTheValleyEntry.creatures[i].IsGrounded ? Vector2.Zero : new Vector2(0f, -32f)), value) < 180f && !NatureInTheValleyEntry.creatures[i].isRunning && !NatureInTheValleyEntry.creatures[i].Dangerous)
				{
					NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<int>(i, "SetRun", new string[]
					{
						"Nature.NatureInTheValley"
					}, null);
					NatureInTheValleyEntry.creatures[i].isRunning = true;
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000208F File Offset: 0x0000028F
		private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
		{
			if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects", false))
			{
				e.Edit(delegate(IAssetData asset)
				{
					IDictionary<string, ObjectData> data = asset.AsDictionary<string, ObjectData>().Data;
					Dictionary<string, ObjectData> dictionary = new Dictionary<string, ObjectData>();
					foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
					{
						if (keyValuePair.Value.Count == 23)
						{
							ObjectData objectData = new ObjectData();
							objectData.Name = keyValuePair.Key;
							objectData.DisplayName = this.helper.Translation.Get(keyValuePair.Key + ".Name");
							objectData.Description = this.helper.Translation.Get("Rarity." + keyValuePair.Value[0]) + "\n\n" + this.helper.Translation.Get(keyValuePair.Key + ".Description");
							objectData.Type = "Basic";
							objectData.Category = -81;
							objectData.Price = (int)((float)int.Parse(keyValuePair.Value[19]) * 1f / 2.25f * this.config.creaturePriceMultiplier);
							objectData.Texture = "Mods\\NatureInTheValley\\Creatures\\Items";
							objectData.SpriteIndex = int.Parse(keyValuePair.Value[20]);
							objectData.CanBeGivenAsGift = false;
							objectData.ExcludeFromRandomSale = true;
							objectData.ExcludeFromShippingCollection = !this.config.addCreaturesToShippingCollection;
							dictionary.Add("NatInValley.Creature." + keyValuePair.Key, objectData);
						}
						else
						{
							ObjectData objectData2 = new ObjectData();
							objectData2.Name = keyValuePair.Key;
							objectData2.DisplayName = keyValuePair.Value[24];
							objectData2.Description = this.helper.Translation.Get("Rarity." + keyValuePair.Value[0]) + "\n\n" + keyValuePair.Value[25];
							objectData2.Type = "Basic";
							objectData2.Category = -81;
							objectData2.Price = (int)((float)int.Parse(keyValuePair.Value[19]) * 1f / 2.25f * this.config.creaturePriceMultiplier);
							objectData2.Texture = keyValuePair.Value[23];
							objectData2.SpriteIndex = int.Parse(keyValuePair.Value[20]);
							objectData2.CanBeGivenAsGift = false;
							objectData2.ExcludeFromRandomSale = true;
							objectData2.ExcludeFromShippingCollection = !this.config.addCreaturesToShippingCollection;
							dictionary.Add("NatInValley.Creature." + keyValuePair.Key, objectData2);
						}
					}
					data.TryAddMany(dictionary);
				}, AssetEditPriority.Default, null);
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00005400 File Offset: 0x00003600
		public void MenuChanged(object sender, MenuChangedEventArgs e)
		{
			IClickableMenu newMenu = e.NewMenu;
			if (!(newMenu is ShopMenu) || ((newMenu as ShopMenu).ShopId != "SeedShop" && (newMenu as ShopMenu).ShopId != "Joja"))
			{
				return;
			}
			Dictionary<ISalable, ItemStockInformation> itemPriceAndStock = (newMenu as ShopMenu).itemPriceAndStock;
			List<ISalable> forSale = (newMenu as ShopMenu).forSale;
			NatInValeyNet natInValeyNet = new NatInValeyNet();
			forSale.Insert(13, natInValeyNet);
			itemPriceAndStock.Add(natInValeyNet, new ItemStockInformation(250, int.MaxValue, null, null, LimitedStockMode.Global, null, null, null, null));
			(newMenu as ShopMenu).itemPriceAndStock = itemPriceAndStock;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000020B8 File Offset: 0x000002B8
		private bool ReturnHome(GameLocation location, string[] args, Farmer player, Point point)
		{
			Game1.warpFarmer("BusStop", 22, 11, 0);
			return true;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000054AC File Offset: 0x000036AC
		public bool OpenDonationWindow(GameLocation location, string[] args, Farmer player, Point point)
		{
			location.createQuestionDialogue(this.helper.Translation.Get("InsectariumDesk"), new Response[]
			{
				new Response("NIVFResponse", this.helper.Translation.Get("InsectariumC1")),
				new Response("NIVSResponse", this.helper.Translation.Get("InsectariumC2")),
				new Response("NIVRResponse", this.helper.Translation.Get("InsectariumC4")),
				new Response("NIVNResponse", this.helper.Translation.Get("InsectariumC3"))
			}, "NatDesk");
			return true;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00005584 File Offset: 0x00003784
		public void setUpInsectarium(GameLocation location)
		{
			int num = 0;
			if (this.CreaturesInLocationR(location) < 1)
			{
				if (Context.IsMainPlayer)
				{
					using (Dictionary<string, List<string>>.Enumerator enumerator = this.creatureData.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, List<string>> keyValuePair = enumerator.Current;
							string text;
							if (location.modData.TryGetValue("NatureInTheValley/Donated/" + keyValuePair.Key, out text))
							{
								num++;
								List<string> value = keyValuePair.Value;
								List<int> list = new List<int>();
								foreach (string s in value[11].Split(",", StringSplitOptions.None))
								{
									list.Add(int.Parse(s));
								}
								NatureInTheValleyEntry.creatures.Add(new NatCreature(new Vector2(float.Parse(value[21]), float.Parse(value[22])) * 64f, location, keyValuePair.Key, int.Parse(value[0]), value[1] == "true", float.Parse(value[2]), int.Parse(value[3]), float.Parse(value[4]), value[5] == "true", value[6] == "true", int.Parse(value[7]), value[8] == "true", new List<string>(value[9].Split(',', StringSplitOptions.None)), int.Parse(value[10]), list, int.Parse(value[12]), int.Parse(value[13]), int.Parse(value[14]), value[15], int.Parse(value[16]), int.Parse(value[17]), int.Parse(value[18]), true));
							}
						}
						goto IL_29C;
					}
				}
				foreach (KeyValuePair<string, List<string>> keyValuePair2 in this.creatureData)
				{
					string text2;
					if (location.modData.TryGetValue("NatureInTheValley/Donated/" + keyValuePair2.Key, out text2))
					{
						num++;
					}
				}
				NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<string>("a", "SetUpInsec", new string[]
				{
					"Nature.NatureInTheValley"
				}, null);
			}
			else
			{
				num = this.CreaturesInLocationR(location);
			}
			IL_29C:
			this.donatedCount = Math.Max(this.donatedCount, num);
			if (this.layersToSave.Count < 1 && location.map.GetLayer("AlwaysFront72") != null)
			{
				this.layersToSave = new List<Layer>
				{
					location.map.GetLayer("AlwaysFront72"),
					location.map.GetLayer("Front72"),
					location.map.GetLayer("Buildings72"),
					location.map.GetLayer("Front60"),
					location.map.GetLayer("Buildings601"),
					location.map.GetLayer("Buildings602"),
					location.map.GetLayer("AlwaysFront28"),
					location.map.GetLayer("Front48"),
					location.map.GetLayer("Buildings48"),
					location.map.GetLayer("Back48"),
					location.map.GetLayer("AlwaysFront24"),
					location.map.GetLayer("Front24"),
					location.map.GetLayer("Buildings24"),
					location.map.GetLayer("Back3"),
					location.map.GetLayer("Back4"),
					location.map.GetLayer("Front36")
				};
			}
			if (this.layersToSave.Count > 1)
			{
				foreach (Layer layer in this.layersToSave)
				{
					if (!location.map.Layers.Contains(layer))
					{
						try
						{
							location.map.InsertLayer(layer, 3);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			if (num == 84)
			{
				location.addButterflies(1.0, true);
			}
			if (num < 72)
			{
				location.map.RemoveLayer(location.map.GetLayer("AlwaysFront72"));
				location.map.RemoveLayer(location.map.GetLayer("Front72"));
				location.map.RemoveLayer(location.map.GetLayer("Buildings72"));
				location.removeTileProperty(38, 36, "Back", "Passable");
				location.removeTileProperty(42, 36, "Back", "Passable");
				location.removeTileProperty(43, 36, "Back", "Passable");
			}
			else
			{
				location.setTileProperty(38, 36, "Back", "Passable", "T");
				location.setTileProperty(42, 36, "Back", "Passable", "T");
				location.setTileProperty(43, 36, "Back", "Passable", "T");
			}
			if (num < 60)
			{
				location.map.RemoveLayer(location.map.GetLayer("Buildings601"));
				location.map.RemoveLayer(location.map.GetLayer("Front60"));
				location.map.RemoveLayer(location.map.GetLayer("Buildings602"));
				location.removeTileProperty(31, 46, "Back", "Passable");
				location.removeTileProperty(35, 46, "Back", "Passable");
				location.removeTileProperty(33, 44, "Back", "Passable");
				location.removeTileProperty(33, 48, "Back", "Passable");
				location.removeTileProperty(47, 47, "Back", "Passable");
				location.removeTileProperty(48, 48, "Back", "Passable");
				location.removeTileProperty(49, 46, "Back", "Passable");
				location.removeTileProperty(50, 47, "Back", "Passable");
				location.removeTileProperty(32, 37, "Back", "Passable");
				location.removeTileProperty(33, 37, "Back", "Passable");
				location.removeTileProperty(34, 37, "Back", "Passable");
				location.removeTileProperty(47, 37, "Back", "Passable");
				location.removeTileProperty(48, 37, "Back", "Passable");
				location.removeTileProperty(49, 37, "Back", "Passable");
				location.removeTileProperty(48, 46, "Back", "Passable");
				location.removeTileProperty(49, 46, "Back", "Passable");
				location.removeTileProperty(48, 47, "Back", "Passable");
				location.removeTileProperty(49, 47, "Back", "Passable");
				location.removeTileProperty(32, 44, "Back", "Passable");
				location.removeTileProperty(34, 44, "Back", "Passable");
				location.removeTileProperty(32, 45, "Back", "Passable");
				location.removeTileProperty(33, 45, "Back", "Passable");
				location.removeTileProperty(34, 45, "Back", "Passable");
				location.removeTileProperty(32, 46, "Back", "Passable");
				location.removeTileProperty(33, 46, "Back", "Passable");
				location.removeTileProperty(34, 46, "Back", "Passable");
				location.removeTileProperty(32, 47, "Back", "Passable");
				location.removeTileProperty(33, 47, "Back", "Passable");
				location.removeTileProperty(34, 47, "Back", "Passable");
			}
			else
			{
				location.setTileProperty(31, 46, "Back", "Passable", "T");
				location.setTileProperty(35, 46, "Back", "Passable", "T");
				location.setTileProperty(33, 44, "Back", "Passable", "T");
				location.setTileProperty(33, 48, "Back", "Passable", "T");
				location.setTileProperty(47, 47, "Back", "Passable", "T");
				location.setTileProperty(48, 48, "Back", "Passable", "T");
				location.setTileProperty(49, 46, "Back", "Passable", "T");
				location.setTileProperty(50, 47, "Back", "Passable", "T");
				location.setTileProperty(32, 37, "Back", "Passable", "T");
				location.setTileProperty(33, 37, "Back", "Passable", "T");
				location.setTileProperty(34, 37, "Back", "Passable", "T");
				location.setTileProperty(47, 37, "Back", "Passable", "T");
				location.setTileProperty(48, 37, "Back", "Passable", "T");
				location.setTileProperty(49, 37, "Back", "Passable", "T");
				location.setTileProperty(48, 46, "Back", "Passable", "T");
				location.setTileProperty(49, 46, "Back", "Passable", "T");
				location.setTileProperty(48, 47, "Back", "Passable", "T");
				location.setTileProperty(49, 47, "Back", "Passable", "T");
				location.setTileProperty(32, 44, "Back", "Passable", "T");
				location.setTileProperty(34, 44, "Back", "Passable", "T");
				location.setTileProperty(32, 45, "Back", "Passable", "T");
				location.setTileProperty(33, 45, "Back", "Passable", "T");
				location.setTileProperty(34, 45, "Back", "Passable", "T");
				location.setTileProperty(32, 46, "Back", "Passable", "T");
				location.setTileProperty(33, 46, "Back", "Passable", "T");
				location.setTileProperty(34, 46, "Back", "Passable", "T");
				location.setTileProperty(32, 47, "Back", "Passable", "T");
				location.setTileProperty(33, 47, "Back", "Passable", "T");
				location.setTileProperty(34, 47, "Back", "Passable", "T");
			}
			if (num < 48)
			{
				location.map.RemoveLayer(location.map.GetLayer("Back48"));
				location.map.RemoveLayer(location.map.GetLayer("AlwaysFront28"));
				location.map.RemoveLayer(location.map.GetLayer("Front48"));
				location.map.RemoveLayer(location.map.GetLayer("Buildings48"));
				location.removeTileProperty(42, 45, "Back", "Passable");
			}
			else
			{
				location.setTileProperty(42, 45, "Back", "Passable", "T");
			}
			if (num < 36)
			{
				location.map.RemoveLayer(location.map.GetLayer("Front36"));
			}
			if (num < 24)
			{
				location.map.RemoveLayer(location.map.GetLayer("AlwaysFront24"));
				location.map.RemoveLayer(location.map.GetLayer("Front24"));
				location.map.RemoveLayer(location.map.GetLayer("Buildings24"));
				location.removeTileProperty(51, 40, "Back", "Passable");
				location.removeTileProperty(51, 43, "Back", "Passable");
				location.removeTileProperty(43, 43, "Back", "Passable");
				location.removeTileProperty(38, 43, "Back", "Passable");
				location.removeTileProperty(30, 43, "Back", "Passable");
				location.removeTileProperty(30, 40, "Back", "Passable");
			}
			else
			{
				location.setTileProperty(51, 40, "Back", "Passable", "T");
				location.setTileProperty(51, 43, "Back", "Passable", "T");
				location.setTileProperty(43, 43, "Back", "Passable", "T");
				location.setTileProperty(38, 43, "Back", "Passable", "T");
				location.setTileProperty(30, 43, "Back", "Passable", "T");
				location.setTileProperty(30, 40, "Back", "Passable", "T");
			}
			if (num < 12)
			{
				location.map.RemoveLayer(location.map.GetLayer("Back3"));
				location.map.RemoveLayer(location.map.GetLayer("Back4"));
			}
			location.reloadMap();
			if (!Game1.player.hasOrWillReceiveMail("InsectariumEntryEvent"))
			{
				Game1.player.mailReceived.Add("InsectariumEntryEvent");
				Event evt = new Event(string.Concat(new string[]
				{
					"continue/-1000 -1000/farmer 41 50 0 IvyInsectarium 41 39 2/skippable/viewport 41 37 true/pause 650/viewport move 0 2 2000/move farmer 0 -5 0/pause 200/jump IvyInsectarium 6/pause 200/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv1"),
					"\"/pause 200/quickQuestion #",
					this.helper.Translation.Get("IvyEv2"),
					"#",
					this.helper.Translation.Get("IvyEv3"),
					"#",
					this.helper.Translation.Get("IvyEv4"),
					"(break)speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv5"),
					"\"(break)speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv6"),
					"\"(break)speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv7"),
					"\"/pause 300/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv8"),
					"\"/end position 41 45"
				}), Game1.player);
				location.startEvent(evt);
				return;
			}
			if (num == 84 && !Game1.player.hasOrWillReceiveMail("InsectariumFinalEvent"))
			{
				Game1.player.mailReceived.Add("InsectariumFinalEvent");
				Event evt2 = new Event(string.Concat(new string[]
				{
					"continue/-1000 -1000/farmer 46 18 0 IvyInsectarium 46 15 2/skippable/viewport 42 14 true/pause 650/viewport move 1 1 4000/pause 200/emote IvyInsectarium 56/pause 200/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv9"),
					"\"/pause 400/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv10"),
					"\"/pause 400/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv11"),
					"\"/pause 400/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv12"),
					"\"/pause 400/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv13"),
					"\"/pause 400/speak IvyInsectarium \"",
					this.helper.Translation.Get("IvyEv14"),
					"\"/viewport move 2 1 3000/pause 2500/end position 41 45"
				}), Game1.player);
				location.startEvent(evt2);
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00006638 File Offset: 0x00004838
		public void CommandTwo(string common, string[] args)
		{
			GameLocation locationFromName = Game1.getLocationFromName("NIVInnerInsec");
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
			{
				locationFromName.modData["NatureInTheValley/Donated/" + keyValuePair.Key] = "true";
			}
		}

		// Token: 0x06000023 RID: 35 RVA: 0x000066B0 File Offset: 0x000048B0
		public void CommandThree(string common, string[] args)
		{
			GameLocation locationFromName = Game1.getLocationFromName("NIVInnerInsec");
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
			{
				locationFromName.modData.Remove("NatureInTheValley/Donated/" + keyValuePair.Key);
			}
		}

		// Token: 0x06000024 RID: 36 RVA: 0x000020CA File Offset: 0x000002CA
		public static bool HighlightCollectableRewards(Item item)
		{
			return Game1.player.couldInventoryAcceptThisItem(item);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x00006724 File Offset: 0x00004924
		public static List<Item> getRewardsForPlayer(Farmer who)
		{
			List<Item> list = new List<Item>();
			int num = 0;
			foreach (KeyValuePair<string, List<string>> keyValuePair in NatureInTheValleyEntry.staticCreatureData)
			{
				string text;
				if (Game1.currentLocation.modData.TryGetValue("NatureInTheValley/Donated/" + keyValuePair.Key, out text))
				{
					num++;
				}
			}
			if (num >= 1 && !who.hasOrWillReceiveMail("InsecReward.SkillBook_2"))
			{
				list.Add(ItemRegistry.Create("SkillBook_2", 1, 0, false));
			}
			if (num >= 4 && !who.hasOrWillReceiveMail("InsecReward.PrizeTicket"))
			{
				list.Add(ItemRegistry.Create("PrizeTicket", 1, 0, false));
			}
			if (num >= 8 && !who.hasOrWillReceiveMail("InsecReward.TreasureTotem"))
			{
				list.Add(ItemRegistry.Create("TreasureTotem", 2, 0, false));
			}
			if (num >= 13 && !who.hasOrWillReceiveMail("InsecReward.StardropTea"))
			{
				list.Add(ItemRegistry.Create("StardropTea", 1, 0, false));
			}
			if (num >= 19 && !who.hasOrWillReceiveMail("InsecReward.GoldCoin"))
			{
				list.Add(ItemRegistry.Create("GoldCoin", 40, 0, false));
			}
			if (num >= 26 && !who.hasOrWillReceiveMail("InsecReward.745"))
			{
				list.Add(ItemRegistry.Create("745", 15, 0, false));
			}
			if (num >= 33 && !who.hasOrWillReceiveMail("InsecReward.724"))
			{
				list.Add(ItemRegistry.Create("724", 2, 0, false));
			}
			if (num >= 40 && !who.hasOrWillReceiveMail("InsecReward.725"))
			{
				list.Add(ItemRegistry.Create("725", 2, 0, false));
			}
			if (num >= 40 && !who.hasOrWillReceiveMail("InsecReward.726"))
			{
				list.Add(ItemRegistry.Create("726", 2, 0, false));
			}
			if (num >= 47 && !who.hasOrWillReceiveMail("InsecReward.681"))
			{
				list.Add(ItemRegistry.Create("681", 2, 0, false));
			}
			if (num >= 54 && !who.hasOrWillReceiveMail("InsecReward.805"))
			{
				list.Add(ItemRegistry.Create("805", 15, 0, false));
			}
			if (num >= 61 && !who.hasOrWillReceiveMail("InsecReward.907"))
			{
				list.Add(ItemRegistry.Create("907", 1, 0, false));
			}
			if (num >= 68 && !who.hasOrWillReceiveMail("InsecReward.688"))
			{
				list.Add(ItemRegistry.Create("688", 2, 0, false));
			}
			if (num >= 68 && !who.hasOrWillReceiveMail("InsecReward.689"))
			{
				list.Add(ItemRegistry.Create("689", 2, 0, false));
			}
			if (num >= 68 && !who.hasOrWillReceiveMail("InsecReward.690"))
			{
				list.Add(ItemRegistry.Create("690", 2, 0, false));
			}
			if (num >= 75 && !who.hasOrWillReceiveMail("InsecReward.NIVSurveyerShirt"))
			{
				list.Add(ItemRegistry.Create("NIVSurveyerShirt", 1, 0, false));
			}
			if (num >= 82 && !who.hasOrWillReceiveMail("InsecReward.NIVSurveyerhat"))
			{
				list.Add(ItemRegistry.Create("NIVSurveyerhat", 1, 0, false));
			}
			if (num >= 84 && !who.hasOrWillReceiveMail("InsecReward.NIVGoldNet"))
			{
				list.Add(new NatInValeyGoldenNet());
			}
			return list;
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000020D7 File Offset: 0x000002D7
		public static void OnRewardCollected(Item item, Farmer who)
		{
			if (item == null)
			{
				return;
			}
			if (who.hasOrWillReceiveMail("InsecReward." + item.ItemId))
			{
				return;
			}
			who.mailReceived.Add("InsecReward." + item.ItemId);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00006A34 File Offset: 0x00004C34
		public void DayStarted(object sender, DayStartedEventArgs eventArgs)
		{
			NatureInTheValleyEntry.creatures.Clear();
			this.dailyMod = this.possibleDailyMods[Game1.random.Next(this.possibleDailyMods.Count - 1)];
			int num = 0;
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
			{
				string text;
				if (Game1.getLocationFromName("NIVInnerInsec").modData.TryGetValue("NatureInTheValley/Donated/" + keyValuePair.Key, out text))
				{
					num++;
				}
			}
			this.donatedCount = num;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002112 File Offset: 0x00000312
		private double func(int x)
		{
			return Math.Max((double)x * Math.Sqrt((double)x) / (double)(45 * x), 1.0);
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00006AE8 File Offset: 0x00004CE8
		private void OnModMessagereceived(object sender, ModMessageReceivedEventArgs eventArgs)
		{
			if (eventArgs.FromModID == "Nature.NatureInTheValley" && !Context.IsSplitScreen)
			{
				string type = eventArgs.Type;
				if (type == "FixListLocations" && !Context.IsMainPlayer)
				{
					Model model = eventArgs.ReadAs<Model>();
					List<int> frames = model.frames;
					List<Vector2> positions = model.positions;
					if (frames.Count == NatureInTheValleyEntry.creatures.Count)
					{
						for (int i = 0; i < frames.Count; i++)
						{
							NatureInTheValleyEntry.creatures[i].Position = positions[i];
							NatureInTheValleyEntry.creatures[i].SetSprite(frames[i]);
						}
						return;
					}
					List<string> names = model.creatures;
					List<string> locations = model.locations;
					this.InstantiateFromLists(names, positions, locations, frames);
					return;
				}
				else
				{
					if (type == "AddToList")
					{
						if (Context.IsMainPlayer && Context.IsWorldReady)
						{
							this.SpawnCreatureInLocation(Game1.getLocationFromName(eventArgs.ReadAs<string>()));
						}
						return;
					}
					if (type == "RemoveFromList")
					{
						if (Context.IsMainPlayer)
						{
							NatureInTheValleyEntry.creatures.RemoveAt(Math.Min(eventArgs.ReadAs<int>(), NatureInTheValleyEntry.creatures.Count - 1));
						}
						return;
					}
					if (type == "SetRun")
					{
						if (Context.IsMainPlayer)
						{
							NatureInTheValleyEntry.creatures[Math.Min(eventArgs.ReadAs<int>(), NatureInTheValleyEntry.creatures.Count - 1)].isRunning = true;
						}
						return;
					}
					if (type == "SetUpInsec" && Context.IsMainPlayer && this.CreaturesInLocationR(Game1.getLocationFromName("NIVInnerInsec")) < 1)
					{
						GameLocation locationFromName = Game1.getLocationFromName("NIVInnerInsec");
						foreach (KeyValuePair<string, List<string>> keyValuePair in this.creatureData)
						{
							string text;
							if (locationFromName.modData.TryGetValue("NatureInTheValley/Donated/" + keyValuePair.Key, out text))
							{
								List<string> value = keyValuePair.Value;
								List<int> list = new List<int>();
								foreach (string s in value[11].Split(",", StringSplitOptions.None))
								{
									list.Add(int.Parse(s));
								}
								NatureInTheValleyEntry.creatures.Add(new NatCreature(new Vector2(float.Parse(value[21]), float.Parse(value[22])) * 64f, locationFromName, keyValuePair.Key, int.Parse(value[0]), value[1] == "true", float.Parse(value[2]), int.Parse(value[3]), float.Parse(value[4]), value[5] == "true", value[6] == "true", int.Parse(value[7]), value[8] == "true", new List<string>(value[9].Split(',', StringSplitOptions.None)), int.Parse(value[10]), list, int.Parse(value[12]), int.Parse(value[13]), int.Parse(value[14]), value[15], int.Parse(value[16]), int.Parse(value[17]), int.Parse(value[18]), true));
							}
						}
						return;
					}
				}
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00006EB4 File Offset: 0x000050B4
		public void CommandFour(string common, string[] args)
		{
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				base.Monitor.Log(natCreature.name, LogLevel.Warn);
				base.Monitor.Log(natCreature.Position.ToString(), LogLevel.Warn);
			}
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00006F30 File Offset: 0x00005130
		public void InstantiateFromLists(List<string> names, List<Vector2> pos, List<string> l, List<int> frames)
		{
			NatureInTheValleyEntry.creatures.Clear();
			for (int i = 0; i < names.Count; i++)
			{
				List<string> list = this.creatureData[names[i]];
				List<int> list2 = new List<int>();
				foreach (string s in list[11].Split(",", StringSplitOptions.None))
				{
					list2.Add(int.Parse(s));
				}
				NatureInTheValleyEntry.creatures.Add(new NatCreature(pos[i], Game1.getLocationFromName(l[i]), names[i], int.Parse(list[0]), list[1] == "true", float.Parse(list[2]), int.Parse(list[3]), float.Parse(list[4]), list[5] == "true", list[6] == "true", int.Parse(list[7]), list[8] == "true", new List<string>(list[9].Split(',', StringSplitOptions.None)), int.Parse(list[10]), list2, int.Parse(list[12]), int.Parse(list[13]), int.Parse(list[14]), list[15], int.Parse(list[16]), int.Parse(list[17]), int.Parse(list[18]), l[i] == "NIVInnerInsec"));
				NatureInTheValleyEntry.creatures[i].SetSprite(frames[i]);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000070FC File Offset: 0x000052FC
		public void OneSecond(object sender, OneSecondUpdateTickedEventArgs e)
		{
			if (Game1.currentLocation == null || Game1.currentLocation.Name == "NIVInnerInsec")
			{
				return;
			}
			if (NatureInTheValleyEntry.creatures.Count < this.locationCap && Game1.random.NextDouble() * this.mpCM < this.spawnChance)
			{
				if (Context.IsMainPlayer)
				{
					this.SpawnCreatureInLocation(Game1.currentLocation);
				}
				else
				{
					base.Helper.Multiplayer.SendMessage<string>(Game1.currentLocation.Name, "AddToList", new string[]
					{
						"Nature.NatureInTheValley"
					}, null);
				}
			}
			this.RemovedTerrain(Game1.currentLocation);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000071A0 File Offset: 0x000053A0
		public void RemovedLargeTerrain(object sender, LargeTerrainFeatureListChangedEventArgs e)
		{
			if (e.Removed == null || !(e.Removed is Bush) || !e.IsCurrentLocation)
			{
				return;
			}
			this.bushPos = this.GetBushes(Game1.currentLocation);
			List<Vector2> list = this.bushPos;
			foreach (NatCreature natCreature in NatureInTheValleyEntry.creatures)
			{
				if (natCreature.LocalLocationCode == 2 && !natCreature.isStatic && natCreature.GetLocation() == Game1.currentLocation)
				{
					Vector2 value = natCreature.Position / 64f;
					bool flag = false;
					foreach (Vector2 value2 in list)
					{
						if (Vector2.Distance(value, value2) < 2.5f)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						if (Context.IsMainPlayer)
						{
							natCreature.isRunning = true;
						}
						else
						{
							NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<int>(NatureInTheValleyEntry.creatures.IndexOf(natCreature), "SetRun", new string[]
							{
								"Nature.NatureInTheValley"
							}, null);
						}
					}
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002132 File Offset: 0x00000332
		public int CreaturesInLocation()
		{
			return NatureInTheValleyEntry.creatures.Count;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000072F0 File Offset: 0x000054F0
		public int CreaturesInLocationR(GameLocation l)
		{
			int num = 0;
			using (List<NatCreature>.Enumerator enumerator = NatureInTheValleyEntry.creatures.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetLocation().Name == l.Name)
					{
						num++;
					}
				}
			}
			return num;
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00007358 File Offset: 0x00005558
		public void RemovedTerrain(GameLocation l)
		{
			if (this.treePos == null)
			{
				this.treePos = new List<Vector2>();
			}
			List<Vector2> list = this.treePos;
			this.step++;
			bool flag = false;
			bool flag2 = this.step % 3 != 0;
			bool isMainPlayer = Context.IsMainPlayer;
			for (int i = 0; i < NatureInTheValleyEntry.creatures.Count; i++)
			{
				NatCreature natCreature = NatureInTheValleyEntry.creatures[i];
				if (isMainPlayer && natCreature.scale < 0.09f)
				{
					NatureInTheValleyEntry.creatures.RemoveAt(i);
					i--;
				}
				else if (!flag2 && natCreature.LocalLocationCode == 1 && natCreature.GetLocation() == l)
				{
					if (!flag)
					{
						List<Vector2> trees = this.GetTrees(l);
						if (trees.Count == list.Count)
						{
							flag2 = true;
							goto IL_152;
						}
						this.treePos = trees;
						list = this.treePos;
						flag = true;
					}
					Vector2 item = new Vector2((float)((int)(natCreature.Position.X / 64f + 0.25f)), (float)((int)(natCreature.Position.Y / 64f - 0.55f)));
					if (!list.Contains(item))
					{
						if (Context.IsMainPlayer)
						{
							natCreature.isRunning = true;
						}
						else
						{
							NatureInTheValleyEntry.staticHelper.Multiplayer.SendMessage<int>(NatureInTheValleyEntry.creatures.IndexOf(natCreature), "SetRun", new string[]
							{
								"Nature.NatureInTheValley"
							}, null);
						}
					}
				}
				IL_152:;
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000074D0 File Offset: 0x000056D0
		public static bool CoverCheckAction(BusStop __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, out bool __result)
		{
			if ((!NatureInTheValleyEntry.staticHelper.ModRegistry.IsLoaded("hootless.BusLocations") || !NatureInTheValleyEntry.staticHelper.ModRegistry.IsLoaded("Nature.NIVBL")) && tileLocation.X == 28 && tileLocation.Y == 11)
			{
				__instance.performAction("NatWarpInsect", who, tileLocation);
				__result = true;
				return false;
			}
			__result = false;
			return true;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00007538 File Offset: 0x00005738
		private bool ValidPosition(Vector2 position, GameLocation l)
		{
			new Location((int)position.X, (int)position.Y);
			return l.map != null && !l.isWaterTile((int)position.X, (int)position.Y) && !l.IsTileOccupiedBy(new Vector2(position.X, position.Y), ~(CollisionMask.Characters | CollisionMask.Farmers | CollisionMask.Flooring), CollisionMask.None, false) && l.isTilePassable(new Vector2(position.X, position.Y));
		}

		// Token: 0x06000033 RID: 51 RVA: 0x0000213E File Offset: 0x0000033E
		private void Pressed(object sender, ButtonPressedEventArgs eventArgs)
		{
			if (this.config.KeyForEncy.IsDown() && Context.IsPlayerFree)
			{
				Game1.activeClickableMenu = new ClickIntoCreatureInfoMenu();
			}
		}

		// Token: 0x04000001 RID: 1
		public IModHelper helper;

		// Token: 0x04000002 RID: 2
		public Dictionary<string, List<string>> creatureData;

		// Token: 0x04000003 RID: 3
		private int keepForLoop;

		// Token: 0x04000004 RID: 4
		private HashSet<string> desertLocationNames;

		// Token: 0x04000005 RID: 5
		private HashSet<string> forestLocationNames;

		// Token: 0x04000006 RID: 6
		private HashSet<string> waterLocationNames;

		// Token: 0x04000007 RID: 7
		private HashSet<string> underLocationNames;

		// Token: 0x04000008 RID: 8
		public static IModHelper staticHelper;

		// Token: 0x04000009 RID: 9
		private static Texture2D netTexture;

		// Token: 0x0400000A RID: 10
		public static Dictionary<string, List<string>> staticCreatureData;

		// Token: 0x0400000B RID: 11
		private int Tiles;

		// Token: 0x0400000C RID: 12
		private List<Vector2> treePos;

		// Token: 0x0400000D RID: 13
		private List<Vector2> waterPos;

		// Token: 0x0400000E RID: 14
		private List<Layer> layersToSave;

		// Token: 0x0400000F RID: 15
		public static List<NatCreature> creatures;

		// Token: 0x04000010 RID: 16
		public NatInValleyConfig config;

		// Token: 0x04000011 RID: 17
		private static PerScreen<int> AnimatedBump = new PerScreen<int>();

		// Token: 0x04000012 RID: 18
		private List<Vector2> bushPos;

		// Token: 0x04000013 RID: 19
		private List<Vector2> stumpPos;

		// Token: 0x04000014 RID: 20
		private int pfeatures;

		// Token: 0x04000015 RID: 21
		public static NatInValleyConfig staticConfig;

		// Token: 0x04000016 RID: 22
		private float dailyMod;

		// Token: 0x04000017 RID: 23
		private List<float> possibleDailyMods;

		// Token: 0x04000018 RID: 24
		private int locationCap;

		// Token: 0x04000019 RID: 25
		private int step;

		// Token: 0x0400001A RID: 26
		private double spawnChance;

		// Token: 0x0400001B RID: 27
		private int donatedCount;

		// Token: 0x0400001C RID: 28
		private double mpCM;

		// Token: 0x0400001D RID: 29
		private Dictionary<string, List<string>> locationalData = new Dictionary<string, List<string>>();
	}
}

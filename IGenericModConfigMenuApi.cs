using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace NatureInTheValley
{
	// Token: 0x02000012 RID: 18
	public interface IGenericModConfigMenuApi
	{
		// Token: 0x060000ED RID: 237
		void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

		// Token: 0x060000EE RID: 238
		void Unregister(IManifest mod);

		// Token: 0x060000EF RID: 239
		void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

		// Token: 0x060000F0 RID: 240
		void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

		// Token: 0x060000F1 RID: 241
		void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
	}
}

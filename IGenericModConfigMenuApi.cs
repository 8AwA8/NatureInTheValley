using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace NatureInTheValley
{
	// Token: 0x02000011 RID: 17
	public interface IGenericModConfigMenuApi
	{
		// Token: 0x060000BD RID: 189
		void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

		// Token: 0x060000BE RID: 190
		void Unregister(IManifest mod);

		// Token: 0x060000BF RID: 191
		void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

		// Token: 0x060000C0 RID: 192
		void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

		// Token: 0x060000C1 RID: 193
		void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
	}
}

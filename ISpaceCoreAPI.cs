using System;
using System.Collections.Generic;
using System.Reflection;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x02000009 RID: 9
	public interface ISpaceCoreAPI
	{
		// Token: 0x060000A3 RID: 163
		string[] GetCustomSkills();

		// Token: 0x060000A4 RID: 164
		int GetLevelForCustomSkill(Farmer farmer, string skill);

		// Token: 0x060000A5 RID: 165
		int GetExperienceForCustomSkill(Farmer farmer, string skill);

		// Token: 0x060000A6 RID: 166
		List<Tuple<string, int, int>> GetExperienceAndLevelsForCustomSkill(Farmer farmer);

		// Token: 0x060000A7 RID: 167
		void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);

		// Token: 0x060000A8 RID: 168
		int GetProfessionId(string skill, string profession);

		// Token: 0x060000A9 RID: 169
		void RegisterSerializerType(Type type);

		// Token: 0x060000AA RID: 170
		void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);
	}
}

using System;
using System.Collections.Generic;
using System.Reflection;
using StardewValley;

namespace NatureInTheValley
{
	// Token: 0x02000008 RID: 8
	public interface ISpaceCoreAPI
	{
		// Token: 0x06000075 RID: 117
		string[] GetCustomSkills();

		// Token: 0x06000076 RID: 118
		int GetLevelForCustomSkill(Farmer farmer, string skill);

		// Token: 0x06000077 RID: 119
		int GetExperienceForCustomSkill(Farmer farmer, string skill);

		// Token: 0x06000078 RID: 120
		List<Tuple<string, int, int>> GetExperienceAndLevelsForCustomSkill(Farmer farmer);

		// Token: 0x06000079 RID: 121
		void AddExperienceForCustomSkill(Farmer farmer, string skill, int amt);

		// Token: 0x0600007A RID: 122
		int GetProfessionId(string skill, string profession);

		// Token: 0x0600007B RID: 123
		void RegisterSerializerType(Type type);

		// Token: 0x0600007C RID: 124
		void RegisterCustomProperty(Type declaringType, string name, Type propType, MethodInfo getter, MethodInfo setter);
	}
}

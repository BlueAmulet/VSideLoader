using HarmonyLib;

namespace VSideLoader
{
	[HarmonyPatch(typeof(Console), "InputText")]
	internal static class ConsoleUtils
	{
		public static void Postfix(ref Console __instance)
		{
			string text = __instance.m_input.text;
			string[] array = text.Split(' ');
			if (array.Length > 0)
			{
				if (array[0] == "reloadtextures")
				{
					if (Settings.textureLoad.Value)
					{
						TextureReplacement.HandleTextures(false);
						__instance.Print("Reloaded textures");
					}
					else
					{
						__instance.Print("Texture loading not enabled in config");
					}
				}
			}
		}
	}
}

using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VSideLoader
{
	internal static class Settings
	{
		private static ConfigFile configFile;
		private static Dictionary<Type, string> typeStr = new Dictionary<Type, string>();

		internal static ConfigEntry<bool> betterLighting;
		internal static ConfigEntry<bool> textureDump;
		internal static ConfigEntry<bool> textureLoad;
		internal static ConfigEntry<bool> detectCollision;
		internal static ConfigEntry<bool> smokeFix;
		internal static ConfigEntry<FilterMode> textureFilter;
		internal static ConfigEntry<bool> useTextureName;
		internal static string[] blackList = {
			"Font Texture",
			"Heightmap_basematerial_ClearedMaskTex",
			"Color Grading Log LUT",
			"Hidden/BlitCopy_MainTex",
			"UnityWhite"
		};
		internal static string[] ignoreName = {
			"beehive_n",
			"cloudberry_d",
			"diffuse",
			"Pine_tree_log_texture",
			"silver_ore_d",
			"silver_ore_m",
			"silver_ore_n"
		};

		static Settings()
		{
			typeStr.Add(typeof(bool), "bool");
			typeStr.Add(typeof(int), "int");
			typeStr.Add(typeof(float), "float");
			typeStr.Add(typeof(KeyboardShortcut), "keybind");
		}

		public static void InitConfig()
		{
			var mOriginal = AccessTools.Method(typeof(ConfigEntryBase), "WriteDescription");
			var mPrefix = AccessTools.Method(typeof(Settings), "MyWriteDescription");
			VSideLoader.harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));

			configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, VSideLoader.PluginName + ".cfg"), true);
			ConfigEntry<int> version = configFile.Bind("Loader", "Version", 1, "Configuration Version");

			betterLighting = configFile.Bind("Shader", "BetterLighting", true, "Removes banding in scene lighting");

			textureDump = configFile.Bind("Texture", "Dump", false, "Dump textures to disk");
			textureLoad = configFile.Bind("Texture", "Load", true, "Load textures from disk");
			detectCollision = configFile.Bind("Texture", "DetectCollision", false, "Detect textures with same name but different contents");
			smokeFix = configFile.Bind("Texture", "SmokeFix", true, "Rename smoke/fog texture from 'dirt' to 'smoke'");
			textureFilter = configFile.Bind("Texture", "TextureFilter", FilterMode.Trilinear, "Texture filtering mode");
			useTextureName = configFile.Bind("Texture", "UseTextureName", true, "Use texture names instead of material name");
			blackList = configFile.Bind("Texture", "BlackList", String.Join(",", blackList), "Textures not to dump").Value.Split(',');
			ignoreName = configFile.Bind("Texture", "IgnoreName", String.Join(",", ignoreName), "Texture names to ignore").Value.Split(',');
		}

		public static bool MyWriteDescription(ref ConfigEntryBase __instance, ref StreamWriter writer)
		{
			if (__instance.ConfigFile == configFile)
			{
				if (!typeStr.TryGetValue(__instance.SettingType, out string TypeName))
				{
					TypeName = __instance.SettingType.Name;
				}
				string TypeDescription = TypeName + ", default: " + TomlTypeConverter.ConvertToString(__instance.DefaultValue, __instance.SettingType);
				if (!string.IsNullOrEmpty(__instance.Description.Description))
				{
					writer.WriteLine("# " + __instance.Description.Description.Replace("\n", "\n# ") + " (" + TypeDescription + ")");
				}
				else
				{
					writer.WriteLine("# " + TypeDescription);
				}
				if (__instance.Description.AcceptableValues != null)
				{
					writer.WriteLine(__instance.Description.AcceptableValues.ToDescriptionString());
				}
				else if (__instance.SettingType.IsEnum)
				{
					writer.WriteLine("# Acceptable values: " + string.Join(", ", Enum.GetNames(__instance.SettingType)));
					if (__instance.SettingType.GetCustomAttributes(typeof(FlagsAttribute), inherit: true).Any())
					{
						writer.WriteLine("# Multiple values can be set at the same time by separating them with , (e.g. Debug, Warning)");
					}
				}
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}

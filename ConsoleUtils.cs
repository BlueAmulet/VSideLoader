using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
					TextureReplacement.HandleTextures(false);
					__instance.Print("Reloaded textures");
				}
			}
		}
	}
}

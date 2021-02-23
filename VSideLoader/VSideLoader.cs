using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;

namespace VSideLoader
{
	[BepInPlugin(GUID, PluginName, Version)]
	public class VSideLoader : BaseUnityPlugin
	{
		public const string GUID = "dev.blueamulet.valheim.loader";
		public const string PluginName = "VSideLoader";
		public const string Version = "1.0.0";

		internal static new ManualLogSource Logger;
		internal static readonly Harmony harmony = new Harmony(GUID);

		public void Awake()
		{
			Logger = base.Logger;
			Settings.InitConfig();
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			int patchedMethods = 0;
			foreach (MethodBase method in harmony.GetPatchedMethods())
			{
				base.Logger.LogInfo("Patched " + method.DeclaringType.Name + "." + method.Name);
				patchedMethods++;
			}
			CleanShader.LoadShader();
			base.Logger.LogInfo(patchedMethods + " patches applied\n");

			if (Settings.textureDump.Value || Settings.textureLoad.Value)
			{
				SceneManager.sceneLoaded += TextureReplacement.OnSceneLoaded;
				//TextureReplacement.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single); // Trigger for current scene as well
			}
		}
	}
}

using UnityEngine;
using UnityEngine.Rendering;

namespace VSideLoader
{
	internal static class CleanShader
	{
		public static void LoadShader()
		{
			if (Settings.betterLighting.Value)
			{
				//string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "deferredshader");
				//AssetBundle bundle = AssetBundle.LoadFromFile(path);
				AssetBundle bundle = AssetBundle.LoadFromMemory(DeferredShader.shader);
				DeferredShader.shader = null;
				if (bundle == null)
				{
					VSideLoader.Logger.LogInfo("Loading bundle failed");
				}
				else
				{
					Shader shader = bundle.LoadAsset("assets/internal-deferredshading.shader") as Shader;
					if (shader != null)
					{
						VSideLoader.Logger.LogInfo("Setting shader");
						GraphicsSettings.SetCustomShader(BuiltinShaderType.DeferredShading, shader);
					}
					else
					{
						VSideLoader.Logger.LogInfo("Could not find shader");
					}
				}
			}
		}
	}
}

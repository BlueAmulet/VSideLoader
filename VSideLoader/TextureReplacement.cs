using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VSideLoader
{
	internal static class TextureReplacement
	{
		private static readonly Dictionary<string, Texture> texSet = new Dictionary<string, Texture>();
		private static readonly Dictionary<string, TextureInfo> loadedTextures = new Dictionary<string, TextureInfo>();

		private static readonly string[] smokeNames = {
			"aoe_smoke_MainTex",
			"build_fog_lowres_MainTex",
			"darklands_groundfog_MainTex",
			"dev_smoke_MainTex",
			"evileyes_particles_MainTex",
			"fog_MainTex",
			"forest_groundmist_MainTex",
			"ghost_smoke_MainTex",
			"godray_MainTex",
			"green_smoke_MainTex",
			"ooze_smoke_MainTex",
			"SmokeBlob_MainTex",
			"swamp_mist_low_MainTex"
		};

		internal static string[] normalMap =
		{
			"_BumpMap",
			"_ChestBumpMap",
			"_LegsBumpMap",
			"_SkinBumpMap",
			"_Normal",
			"_NormalTex",
			"_DetailNormalMap",
			"_CliffNormal",
			"_CultivatedNormal",
			"_DirtNormal",
			"_ForestNormal",
			"_GrassNormal",
			"_PavedNormal",
			"_RainNormal",
			"_RefractionNormal",
			"_RockNormal",
			"_SnowNormal"
		};

		internal static void HandleTextures(bool allowDump)
		{
			string basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Textures");
			string dumpPath = Path.Combine(basePath, "Dump");
			string loadPath = Path.Combine(basePath, "Load");

			Directory.CreateDirectory(basePath);
			Directory.CreateDirectory(dumpPath);
			Directory.CreateDirectory(loadPath);

			Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
			VSideLoader.Logger.LogInfo("Found " + materials.Length + " materials");
			foreach (Material material in materials)
			{
				string[] propertyNames = material.GetTexturePropertyNames();
				foreach (string propertyName in propertyNames)
				{
					string matTexName = material.name + (propertyName.StartsWith("_") ? "" : "-") + propertyName;
					string texName = null;
					Texture texture = material.GetTexture(propertyName);
					if (texture != null)
					{
						bool dumpTexture = allowDump;
						if (Settings.useTextureName.Value && !Settings.ignoreName.Contains(texture.name))
						{
							texName = texture.name;
						}
						if (Settings.useTextureName.Value && Settings.smokeFix.Value && texture.name == "dirt" && smokeNames.Contains(matTexName))
						{
							texName = "smoke";
						}/*
						if (Settings.ignoreName.Contains(texture.name))
						{
							VSideLoader.Logger.LogInfo("Ignore: " + texture.name + " is " + matTexName);
						}//*/
						if (String.IsNullOrWhiteSpace(texName))
						{
							texName = matTexName;
						}
						if (Settings.blackList.Contains(texName) || loadedTextures.Any(pair => pair.Value.tex == texture))
						{
							// ignore texture
							dumpTexture = false;
						}
						else if (!texSet.ContainsKey(texName))
						{
							if (Settings.detectCollision.Value)
							{
								texSet.Add(texName, texture);
							}
						}
						else if (texSet[texName] != texture)
						{
							VSideLoader.Logger.LogWarning("Duplicate texture name: " + texName);
							//texName += "_DupeTex";
						}
						if (Settings.textureDump.Value && dumpTexture)
						{
							if (texture is Texture2D tex2d)
							{
								Texture2D newTexture = DuplicateTexture(tex2d);
								if (Settings.normalFix.Contains(propertyName))
								{
									Color[] pixels = newTexture.GetPixels();
									for (int i = 0; i < pixels.Length; i++)
									{
										pixels[i].r *= pixels[i].a;
										float x = (pixels[i].r * 2f) - 1f;
										float y = (pixels[i].g * 2f) - 1f;
										pixels[i].b = (Mathf.Sqrt(1f - (x * x) - (y * y)) + 1f) / 2f;
										pixels[i].a = 1f;
									}
									newTexture.SetPixels(pixels);
								}
								File.WriteAllBytes(Path.Combine(dumpPath, texName + ".png"), newTexture.EncodeToPNG());
							}
							else
							{
								VSideLoader.Logger.LogWarning("Don't know how to handle texture of type " + texture.GetType().Name + " (" + texName + ")");
							}
						}
					}
					if (Settings.textureLoad.Value)
					{
						string texPath = Path.Combine(loadPath, matTexName + ".png");
						if (!File.Exists(texPath) && !String.IsNullOrWhiteSpace(texName) && texName != matTexName)
						{
							texPath = Path.Combine(loadPath, texName + ".png");
						}
						if (File.Exists(texPath))
						{
							Texture2D tex;
							bool needsLoad = false;
							DateTime time = File.GetLastWriteTime(texPath);
							if (loadedTextures.ContainsKey(texPath))
							{
								tex = loadedTextures[texPath].tex; // Reuse existing texture
								if (!loadedTextures[texPath].time.Equals(time))
								{
									needsLoad = true;
								}
							}
							else
							{
								tex = new Texture2D(2, 2, TextureFormat.RGBA32, true, normalMap.Contains(propertyName));
								loadedTextures.Add(texPath, new TextureInfo(tex, new DateTime()));
								needsLoad = true;
							}
							if (texture != null)
							{
								tex.name = texture.name;
							}
							tex.filterMode = Settings.textureFilter.Value;
							if (needsLoad)
							{
								if (tex.LoadImage(File.ReadAllBytes(texPath), !Settings.normalFix.Contains(propertyName)))
								{
									if (Settings.normalFix.Contains(propertyName) && tex.isReadable)
									{
										Color[] pixels = tex.GetPixels();
										for (int i = 0; i < pixels.Length; i++)
										{
											pixels[i].a = pixels[i].r;
											pixels[i].r = 1f;
											pixels[i].b = pixels[i].g;
										}
										tex.SetPixels(pixels);
									}
									VSideLoader.Logger.LogInfo("Loaded " + Path.GetFileName(texPath) + " (" + tex.width + "x" + tex.height + ") for " + material.name + "." + propertyName);
									material.SetTexture(propertyName, tex);
									loadedTextures[texPath].time = time;
								}
								else
								{
									VSideLoader.Logger.LogError("Failed to load " + Path.GetFileName(texPath));
								}
							}
							else
							{
								VSideLoader.Logger.LogInfo("Reusing loaded texture " + Path.GetFileName(texPath) + " for " + material.name + "." + propertyName);
								material.SetTexture(propertyName, tex);
							}
						}
					}
				}
			}
			if (Settings.detectCollision.Value)
			{
				VSideLoader.Logger.LogInfo("Found " + texSet.Count + " textures");
			}
		}

		internal static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			HandleTextures(true);
		}

		private static Texture2D DuplicateTexture(Texture2D source)
		{
			// TODO: This is causing very slight 1 bit corruption
			RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, source.graphicsFormat.ToString().EndsWith("_SRGB") ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
			Graphics.Blit(source, renderTex);
			Texture2D texture = RenderToTexture(renderTex);
			RenderTexture.ReleaseTemporary(renderTex);
			return texture;
		}

		private static Texture2D RenderToTexture(RenderTexture renderTex)
		{
			// TODO: This is also possibly causing 1 bit corruption
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D texture = new Texture2D(renderTex.width, renderTex.height, TextureFormat.ARGB32, false, false);
			texture.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			texture.Apply();
			RenderTexture.active = previous;
			return texture;
		}
	}
}

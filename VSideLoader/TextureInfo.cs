using System;
using UnityEngine;

namespace VSideLoader
{
	internal class TextureInfo
	{
		internal Texture2D tex;
		internal DateTime time;

		internal TextureInfo(Texture2D tex, DateTime time)
		{
			this.tex = tex;
			this.time = time;
		}
	}
}

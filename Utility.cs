using StbImageSharp;
using System.IO;
using UnityEngine;

namespace ModMogul
{
	internal class Utility
	{
		public static Sprite ImportSprite(string path, bool flipVertically = true)
		{
			// Correct order: check null/empty BEFORE File.Exists
			if (string.IsNullOrEmpty(path))
				return GenerateFallbackSprite();

			if (!File.Exists(path))
			{
				Debug.LogError("File not found: " + path);
				return GenerateFallbackSprite();
			}

			byte[] data = File.ReadAllBytes(path);

			var img = ImageResult.FromMemory(data, ColorComponents.RedGreenBlueAlpha);

			var tex = new Texture2D(img.Width, img.Height, TextureFormat.RGBA32, mipChain: false, linear: false);
			tex.filterMode = FilterMode.Point;
			tex.wrapMode = TextureWrapMode.Clamp;

			// Convert RGBA bytes -> Color32[]
			var pixels = new Color32[img.Width * img.Height];
			var src = img.Data;
			for (int i = 0, p = 0; i < pixels.Length; i++, p += 4)
				pixels[i] = new Color32(src[p], src[p + 1], src[p + 2], src[p + 3]);

			if (flipVertically)
				FlipVertically(pixels, img.Width, img.Height);

			tex.SetPixels32(pixels);
			tex.Apply(false, false);

			return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
		}

		static Sprite GenerateFallbackSprite()
		{
			Texture2D texture = new Texture2D(4, 4, TextureFormat.RGBA32, false, false);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;

			Color[] pixels = new Color[4 * 4];
			for (int y = 0; y < 4; y++)
				for (int x = 0; x < 4; x++)
					pixels[y * 4 + x] = ((x % 2 == 0) ^ (y % 2 == 0)) ? Color.purple : Color.black;

			texture.SetPixels(pixels);
			texture.Apply(false, false);

			return Sprite.Create(texture, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 100f);
		}

		static void FlipVertically(Color32[] pixels, int width, int height)
		{
			int row = width;
			for (int y = 0; y < height / 2; y++)
			{
				int top = y * row;
				int bottom = (height - 1 - y) * row;
				for (int x = 0; x < row; x++)
				{
					var tmp = pixels[top + x];
					pixels[top + x] = pixels[bottom + x];
					pixels[bottom + x] = tmp;
				}
			}
		}
	}
}

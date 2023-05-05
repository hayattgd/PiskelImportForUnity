using System;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace PiskelImporter
{
    [ScriptedImporter(version: 13, ext: "piskel")]
    public class Importer : ScriptedImporter
    {
        public FilterMode filterMode = FilterMode.Point;
        public SpriteMeshType spriteMeshType;
        [Min(0.01f)]
        public float pixelsPerUnit = 16f;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            string assettext = File.ReadAllText(ctx.assetPath);
            var file = JsonConvert.DeserializeObject<PiskelFile>(assettext);

            if (file != null)
            {
                for (int l = 0; l < file.piskel.layers.Length; l++)
                {
                    var layer = JsonConvert.DeserializeObject<Layer>(file.piskel.layers[l]);
                    if (layer != null)
                    {
                        for (int c = 0; c < layer.chunks.Length; c++)
                        {
                            string base64data = layer.chunks[c].base64PNG.Split(',')[1];

                            Sprite base64encoded = Base64ToSprite(base64data);
                            base64encoded.texture.filterMode = filterMode;
                            base64encoded.texture.alphaIsTransparency = true;
                            base64encoded.name = layer.name;

                            ctx.AddObjectToAsset($"Original_{layer.name}", base64encoded.texture);
                            ctx.SetMainObject(base64encoded.texture);

                            List<Sprite> frames = SplitSprite(base64encoded, layer.frameCount, pixelsPerUnit, spriteMeshType);
                            for (int f = 0; f < frames.Count; f++)
                            {
                                frames[f].name = file.piskel.name + "_" + layer.name + "_" + (f+1);
                                ctx.AddObjectToAsset($"{l}{f + 1}", frames[f]);
                            }
                        }
                    }
                    else
                    {
                        ctx.LogImportError($"Failed to Import Layer {l} (39)");
                    }
                }
            }
            else
            {
                ctx.LogImportError("Unable to load file as Piskel file");
            }
        }

        public static List<Sprite> SplitSprite(Sprite original, int frames, float ppu, SpriteMeshType smt)
        {
            List<Sprite> final = new List<Sprite> { };
            float width = original.texture.width / frames;

            for (int i = 0; i < frames; i++)
            {
                Vector2 pos = new Vector2(i * width, 0);

                Rect framerect = new Rect(pos.x, pos.y, width, original.texture.height);
                Sprite result = Sprite.Create(original.texture, framerect, Vector2.one * 0.5f, ppu, 0, smt);

                final.Add(result);
            }

            return final;
        }

        public static Sprite Base64ToSprite(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        }
    }
}
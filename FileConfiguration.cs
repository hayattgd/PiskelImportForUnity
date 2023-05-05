using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiskelImporter
{
    [Serializable]
    public class PiskelFile
    {
        public int modelVersion;
        public PiskelSprite piskel;
    }

    [Serializable]
    public class PiskelSprite
    {
        public string name;
        public string description;
        public int fps;
        public int height;
        public int width;
        public string[] layers;
    }

    [Serializable]
    public class Layer
    {
        public string name;
        public float opacity;
        public int frameCount;
        public Chunk[] chunks;
    }

    [Serializable]
    public class Chunk
    {
        public string base64PNG;
    }
}

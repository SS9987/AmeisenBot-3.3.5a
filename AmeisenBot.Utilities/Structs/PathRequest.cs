﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotUtilities.Structs
{
    public struct PathRequest
    {
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public int MapId { get; set; }

        public PathRequest(Vector3 a, Vector3 b, int mapId)
        {
            A = a;
            B = b;
            MapId = mapId;
        }
    }
}

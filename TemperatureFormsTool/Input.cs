﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TemperatureFormsTool
{
    public class Input
    {
        public string Name { get; }

        public string Id { get; }

        public Input(string name,string id) {
            Name = name;
            Id = id;
        }
    }
}

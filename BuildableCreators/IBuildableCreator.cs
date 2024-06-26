﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AirportCEOCustomBuildables;

public interface IBuildableCreator
{
    List<GameObject> buildables { get; set; }

    void SetUp();

    void ClearBuildables();

    void CreateBuildables();
}

﻿using NoMansGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoMansGUI.Docking.Interfaces
{
    public interface ILayoutItemStatePersister
    {
        bool SaveState(IShell shell, IShellView shellView, string filename);
        bool LoadState(IShell shell, IShellView shellView, string filename);
    }
}

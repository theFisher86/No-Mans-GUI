using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMGUIFramework.Module
{
    public interface IModule
    {
        void PreInitialize();
        void Initialize();
        void PostInitialize();
    }
}

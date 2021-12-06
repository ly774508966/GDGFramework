using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.Utils;
namespace GDG.Async
{
    public interface IYieldInstruction
    {
        bool CanMoveNext { get; }
    }
}
﻿using System;

namespace Assets.CourseGame.Develop.Utils.Conditions
{
    public interface ICompositeCondition : ICondition
    {
        ICompositeCondition Add(ICondition condition, Func<bool, bool, bool> logicOperation = null);

        ICompositeCondition Remove(ICondition condition);   
    }
}

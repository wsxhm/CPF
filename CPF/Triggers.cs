using CPF.Animation;
using CPF.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPF
{
    public class Triggers : Collection<Trigger>
    {
        public Trigger Add(string Property, Relation TargetRelation, Func<object, bool> PropertyConditions = default, params (string property, object value)[] setters)
        {
            var t = new Trigger(Property, TargetRelation, PropertyConditions);
            if (setters != null && setters.Length > 0)
            {
                foreach (var item in setters)
                {
                    t.Setters.Add(item.property, item.value);
                }
            }
            Add(t);
            return t;
        }
        //public Trigger Add(string Property, Relation TargetRelation, Func<object, bool> PropertyConditions = default, Storyboard storyboard = null, params (string property, object value)[] setters)
        //{
        //    var t = new Trigger(Property, TargetRelation, PropertyConditions);
        //    t.Animation = storyboard;
        //    Add(t);
        //    if (setters != null && setters.Length > 0)
        //    {
        //        foreach (var item in setters)
        //        {
        //            t.Setters.Add(item.property, item.value);
        //        }
        //    }
        //    return t;
        //}
    }
}

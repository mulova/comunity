using System;
using System.Collections.Generic;
using System.Reflection;

namespace commons
{
    public class DepInjector<T> : Loggable where T:class
    {
        private static AttributeRegistry attrReg = new AttributeRegistry();
        private Dictionary<Type, T> sourceDic = new Dictionary<Type, T>();
        private static MultiMap<Type, FieldInfo> targetDic;

        public bool RegisterIf(T instance, Type attrType)
        {
            if (attrReg.Contains(instance.GetType(), attrType))
            {
                sourceDic[instance.GetType()] = instance;
                return true;
            } else
            {
                return false;
            }
        }

        public T GetSource(Type type)
        {
            return sourceDic.Get(type);
        }

        public List<T> GetSources()
        {
            return new List<T>(sourceDic.Values);
        }

        public static void CollectDiTypes()
        {
            if (targetDic != null)
            {
                return;
            }
            targetDic = new MultiMap<Type, FieldInfo>();
            List<Type> targetList = ReflectionUtil.FindClassesWithAttribute<InjectAttribute>();
            foreach (Type t in targetList)
            {
                foreach (var f in t.GetFields(ReflectionUtil.INSTANCE_FLAGS))
                {
                    if (attrReg.Contains(f.FieldType, typeof(InjectionSourceAttribute)))
                    {
                        targetDic.Add(t, f);
                    }
                }
            }
        }

        public bool Resolve(T target)
        {
            bool changed = false;
            CollectDiTypes();
            List<FieldInfo> list = targetDic.GetSlot(target.GetType());
            foreach (FieldInfo f in list)
            {
                object src = sourceDic.Get(f.FieldType);
                if (f.GetValue(target) != src)
                {
                    f.SetValue(target, src);
                    log.Info("Inject dependency: {0}.{1} = {2}", target, f.Name, src);
                    changed = true;
                }
            }
            return changed;
        }
    }
}


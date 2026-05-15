using highspeed.framework.Common;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;

namespace highspeed.framework
{
    public static class ObjectCopy
    {
        /// <summary>
        /// 从父级类对象复制属性，返回子类对象
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static TChild CopyFromParent<TParent, TChild>(this TParent parent) where TChild : TParent, new()
        {
            TChild child = new TChild();
            var ParentType = typeof(TParent);
            var Properties = ParentType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    try
                    {
                        //进行属性拷贝
                        Propertie.SetValue(child, Propertie.GetValue(parent, null), null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("ObjectCopy.CopyFromParent", $"{child.GetType().Name}.{Propertie.Name} 赋值失败: {ex.Message}");
                    }
                }
            }
            return child;
        }

        /// <summary>
        /// 复制对象（属性复制）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T Copy<T>(this T source) where T : new()
        {
            T target = new T();
            var TType = typeof(T);
            var Properties = TType.GetProperties();
            foreach (var Propertie in Properties)
            {
                //循环遍历属性
                if (Propertie.CanRead && Propertie.CanWrite)
                {
                    try
                    {
                        //进行属性拷贝
                        Propertie.SetValue(target, Propertie.GetValue(source, null), null);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn("ObjectCopy.Copy<T>", $"{TType.Name}.{Propertie.Name} 赋值失败: {ex.Message}");
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// 复制集合（属性复制）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IList<T> CopyList<T>(this IList<T> source) where T : new()
        {
            return source.Select(item => (T)item.Copy()).ToList();
        }

        /// <summary>
        /// 复制对象属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static TTarget CopyProperties<TSource, TTarget>(this TSource source, params string[] ignoreProperties) where TTarget : new()
        {
            TTarget target = new TTarget();
            return CopyProperties(source, target, ignoreProperties);
        }

        /// <summary>
        /// 复制对象属性
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static TTarget CopyProperties<TSource, TTarget>(this TSource source, TTarget target, params string[] ignoreProperties)
        {
            if (source == null) return target;
            if (target == null) target = (TTarget)Activator.CreateInstance(typeof(TTarget));

            var SType = typeof(TSource);
            var TType = typeof(TTarget);
            var SProperties = SType.GetProperties();
            var TProperties = TType.GetProperties();
            foreach (var sp in SProperties)
                if (sp.CanRead && (ignoreProperties == null || !ignoreProperties.Contains(sp.Name)))
                    foreach (var tp in TProperties)
                        if (tp.Name == sp.Name && tp.PropertyType == sp.PropertyType)
                        {
                            if (tp.CanWrite)
                                try
                                {
                                    tp.SetValue(target, sp.GetValue(source, null), null);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Warn("ObjectCopy.CopyProperties<TSource, TTarget>", $"{TType.Name}.{tp.Name} 赋值失败: {ex.Message}");
                                }
                            break;
                        }
            return target;
        }

        /// <summary>
        /// MemoryStream对象克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T MemoClone<T>(this T source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)bf.Deserialize(ms);
            }
        }

        /// <summary>
        /// MemoryStream集合克隆
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<T> ListClone<T>(this List<T> source)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (List<T>)bf.Deserialize(ms);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Cache
{
    internal class EntityMappingCache
    {
        private IDictionary<Type, IDictionary<string, int>> _Cache = new Dictionary<Type, IDictionary<string, int>>();
        private static EntityMappingCache _Instance = new EntityMappingCache();

        public static EntityMappingCache Instance
        {
            get { return _Instance; }
        }

        public int GetDatabaseFieldIndex(Type type, string propertyName)
        {
            int result = -1;
            if (this._Cache.ContainsKey(type))
            {
                IDictionary<string, int> dict = this._Cache[type];
                if (dict.ContainsKey(propertyName))
                {
                    result = dict[propertyName];
                }
            }
            return result;
        }

        public void SetDatabaseFieldIndex(Type type, string propertyName, int index)
        {
            IDictionary<string, int> dict = null;
            if (this._Cache.ContainsKey(type))
            {
                dict = this._Cache[type];
            }
            else
            {
                dict = new Dictionary<string, int>();
                this._Cache.Add(type, dict);
            }
            if (dict.ContainsKey(propertyName))
            {
                dict[propertyName] = index;
            }
            else
            {
                dict.Add(propertyName, index);
            }
        }
    }
}

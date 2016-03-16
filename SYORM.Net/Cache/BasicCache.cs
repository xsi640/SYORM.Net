using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Cache
{
    public class BasicCache<TKey, TOjbect>
    {
        private IDictionary<TKey, TOjbect> _Cache = new Dictionary<TKey, TOjbect>();
        private static BasicCache<TKey, TOjbect> _Instance = new BasicCache<TKey, TOjbect>();

        public static BasicCache<TKey, TOjbect> Instance
        {
            get { return _Instance; }
        }

        public TOjbect Get(TKey key)
        {
            TOjbect result = default(TOjbect);
            if (this._Cache.ContainsKey(key))
            {
                result = this._Cache[key];
            }
            return result;
        }

        public void Set(TKey key, TOjbect obj)
        {
            if (this._Cache.ContainsKey(key))
            {
                this._Cache[key] = obj;
            }
            else
            {
                this._Cache.Add(key, obj);
            }
        }
    }
}

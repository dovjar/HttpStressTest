namespace HttpStressTest
{
    public static class DictionaryHelpers
    {
        public static Dictionary<string,string> Upsert(this Dictionary<string,string> obj, string key, string value)
        {
            if (obj.Keys.Contains(key))
            {
                obj[key] = value;
            }
            else
            {
                obj.Add(key, value);
            }
            return obj;
        }
        public static Dictionary<string,string> Upsert(this Dictionary<string,string> obj,Dictionary<string,string> values)
        {
            foreach(var key in values.Keys)
            {
                obj.Upsert(key, values[key]);
            }
            return obj;

        }
    }
}
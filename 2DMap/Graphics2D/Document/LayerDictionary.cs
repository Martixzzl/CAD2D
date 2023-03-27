namespace Graphics2D
{
    /// <summary>
    /// 图层字典类
    /// </summary>
    public class LayerDictionary : PersistableDictionaryWithDefault<Layer>
    {
        public LayerDictionary() : base("0", Layer.Default)
        {
            ;
        }
    }
}

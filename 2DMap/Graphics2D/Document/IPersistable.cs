namespace Graphics2D
{
    /// <summary>
    /// 文件接口
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="reader"></param>
        void Load(DocumentReader reader);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="writer"></param>
        void Save(DocumentWriter writer);
    }
}

namespace Graphics2D
{
    public class TextStyleDictionary : PersistableDictionaryWithDefault<TextStyle>
    {
        public TextStyleDictionary() : base("0", TextStyle.Default)
        {
            ;
        }
    }
}

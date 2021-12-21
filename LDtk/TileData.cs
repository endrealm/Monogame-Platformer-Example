namespace LDtk
{
    public class TileData
    {
        public TileData(EnumData enumData)
        {
            EnumData = enumData;
        }

        public EnumData EnumData { get; }
    }

    public class EnumData
    {
        public string EnumName { get; internal set; }
        public long EnumUid { get; internal set; }
        public string[] Values { get; internal set; }
    }
}
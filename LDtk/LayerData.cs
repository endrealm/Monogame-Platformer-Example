namespace LDtk
{
    public class LayerData
    {
        /// <summary>
        /// the cell size
        /// </summary>
        public float GridCellSize { get; internal set; }
        public TileData[][] TileData { get; internal set; }
    }
}
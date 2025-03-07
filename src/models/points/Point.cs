namespace FastReset.Models {
    /**
     * <summary>
     * A class which is used for temporary points.
     * </summary>
     * <see cref="Config.SavedPoint">See Config.SavedPoint for a permanently saved point</see>
     */
    public class Point : BasePoint {
        // Position
        public override float posX { get; set; }
        public override float posY { get; set; }
        public override float posZ { get; set; }

        // Rotation x
        public override float rotY { get; set; }
        public override float rotW { get; set; }

        // Rotation y
        public override float rotationY { get; set; }
    }
}

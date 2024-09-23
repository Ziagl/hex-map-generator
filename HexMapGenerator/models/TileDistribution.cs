namespace HexMapGenerator.models;

internal class TileDistribution
{
    public readonly float polar;
    public readonly float temperate;
    public readonly float dry;
    public readonly float tropical;

    // approx. size of each climate zone (based on read earth data)
    public static readonly float[] climateZoneSizes = { 0.18f, 0.38f, 0.14f, 0.3f };

    public TileDistribution()
    {
        this.polar = this.temperate = this.dry = this.tropical = 0.25f;
    }

    public TileDistribution(float polar, float temperate, float dry, float tropical)
    {
        float max = polar + temperate + dry + tropical;
        if (max <= 0)
        {
            this.polar = this.temperate = this.dry = this.tropical = 0.25f;
        }
        else
        {
            this.polar = polar / max;
            this.temperate = temperate / max;
            this.dry = dry / max;
            this.tropical = tropical / max;
        }
    }
}

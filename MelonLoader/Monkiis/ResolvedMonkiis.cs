namespace MonkiiLoader
{
    public sealed class ResolvedMonkiis // This class only exists because I can't use Tuples
    {
        public readonly MonkiiBase[] loadedMonkiis;
        public readonly RottenMonkii[] rottenMonkiis;

        public ResolvedMonkiis(MonkiiBase[] loadedMonkiis, RottenMonkii[] rottenMonkiis)
        {
            this.loadedMonkiis = loadedMonkiis ?? new MonkiiBase[0];
            this.rottenMonkiis = rottenMonkiis ?? new RottenMonkii[0];
        }
    }
}
